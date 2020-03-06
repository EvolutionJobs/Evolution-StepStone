namespace EvoApi.Services.StepStone
{
    using EvoApi.Services.Serialisation;
    using EvoApi.Services.StepStone.Models;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Formatting;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>Utility for accessing StepStone API.</summary>
    sealed class StepStoneService :
        IDisposable,
        IStepStoneService
    {
        #region set up static JSON serialisers

        /// <summary>Get the serialisation settings used by StepStone's search endpoint</summary>
        static JsonSerializerSettings JsonSettings { get; }

        static IEnumerable<JsonSerializerSettings> ErrorSerialisers { get; }

        /// <summary>Get the media formatter used by StepStone's search endpoint</summary>
        static IEnumerable<MediaTypeFormatter> SearchMediaFormatter { get; }

        /// <summary>Get the media formatter used by StepStone's authentication endpoint</summary>
        static IEnumerable<MediaTypeFormatter> AuthenticationMediaFormatter { get; }

        static StepStoneService()
        {
            // Search endpoint expects and returns JSON serialised objects with TitleCaseProperties
            JsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new TitleCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatString = "dd/MM/yyyy HH:mm:ss",
                DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
                MissingMemberHandling = MissingMemberHandling.Ignore,
            };

            JsonSettings.Converters.Add(new TitleCaseEnumConverter());

            SearchMediaFormatter = new MediaTypeFormatter[1] {
                new JsonMediaTypeFormatter { SerializerSettings = JsonSettings }
            };

            // Auth endpoint expects Form data and sends back JSON with snake_case_properties
            var authJsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new SnakeCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            authJsonSettings.Converters.Add(new SlugCaseEnumConverter());

            AuthenticationMediaFormatter = new MediaTypeFormatter[1] {
                new JsonMediaTypeFormatter { SerializerSettings = authJsonSettings }
            };

            ErrorSerialisers = new JsonSerializerSettings[2] { JsonSettings, authJsonSettings };
        }

        #endregion

        /// <summary>Dependency injected logging service.</summary>
        readonly ILogger logger;

        /// <summary>The StepStone brands configured.</summary>
        readonly ServiceSettings[] settings;

        /// <summary>Name for this application in requests made to StepStone.</summary>
        readonly string application;

        /// <summary>HTTP client for making requests.</summary>
        HttpClient client;

        /// <summary>Create a StepStone API utility.
        /// This should be a singleton as it will retain an HTTP client.</summary>
        /// <param name="settings">Collection of StepStone services</param>
        public StepStoneService(ILoggerFactory loggerFactory, string application, ServiceSettings[] settings)
        {
            this.application = application;

            this.logger = loggerFactory.CreateLogger<StepStoneService>();

            // If the logging is set to info or trace add a wrapper around every request that records exactly what is sent/recieved
            this.client = this.logger?.IsEnabled(LogLevel.Information) ?? false ?
                new HttpClient(new LoggingHandler(new HttpClientHandler(), this.logger)) { Timeout = new TimeSpan(0, 0, 30) } :
                new HttpClient() { Timeout = new TimeSpan(0, 0, 30) };

            this.settings = settings;
            if (this.settings?.Length > 0)
                this.logger.LogInformation("StepStone services: {StepStoneServices}", string.Join(", ", this.settings.Select(s => s.Name)));
        }

        void IDisposable.Dispose()
        {
            if (this.client == null)
                return;

            this.client.Dispose();
            this.client = null;
        }

        async Task<StepStoneToken> IStepStoneService.Authenticate(string username, string session)
        {
            if (!(this.settings?.Length > 0))
                return null;

            var tokens = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            int expires = 3600;

            foreach (var setting in this.settings)
            {
                string authUrl = setting.Url + "/authorisationserver/token";
                var tokenDictionary = new Dictionary<string, string> {
                    { "client_id", setting.ClientID },
                    { "client_secret", setting.ClientSecret },
                    { "client_user_id", username },
                    { "username", setting.RecruiterUsername },
                    { "password", setting.RecruiterPassword },
                    { "grant_type", "password" },
                    { "scope" , "CVDatabase" },
                    { "client_device_id", this.application + "+" + session }
                };

                using var token = new FormUrlEncodedContent(tokenDictionary);
                using var response = await this.client.PostAsync(authUrl, token);

                if (!response.IsSuccessStatusCode)
                    throw new StepStoneAuthenticationException(
                        await response.Content.ReadAsAsync<ErrorService>(AuthenticationMediaFormatter));

                var result = await response.Content.ReadAsAsync<TokenResponse>(AuthenticationMediaFormatter);
                tokens[setting.Name] = result.AccessToken;

                // Take the shortest expiration
                expires = Math.Min(expires, result.ExpiresIn);
            }

            return new StepStoneToken
            {
                User = username,
                Session = session,
                Tokens = tokens,
                Expires = expires
            };
        }

        /// <summary>We get error messages if we pass incompatible queries.
        /// Clean up request so that conflicting restrictions are not passed.</summary>
        /// <param name="request">Request to clean.</param>
        /// <returns>Cleaned request</returns>
        SearchRequest Clean(SearchRequest request)
        {
            if (request.Salary != null)
            {
                if (request.Salary.From > request.Salary.To)
                {
                    this.logger.LogWarning("From {From} must be lower than to {To}.", request.Salary.From, request.Salary.To);
                    request.Salary = null;
                }
                else if (request.SalaryAnnual?.Count() > 0)
                {
                    this.logger.LogWarning("Cannot pass both salary filter and SalaryAnnual facet.");
                    request.Salary = null;
                }
                else if (request.SalaryDay?.Count() > 0)
                {
                    this.logger.LogWarning("Cannot pass both salary filter and SalaryDay facet.");
                    request.Salary = null;
                }
                else if (request.SalaryHour?.Count() > 0)
                {
                    this.logger.LogWarning("Cannot pass both salary filter and SalaryHour facet.");
                    request.Salary = null;
                }
            }

            if (string.IsNullOrEmpty(request.CurrentLocation))
            {
                // We don't have a current location, radius and travel time should not be passed
                if (request.Radius != null)
                {
                    this.logger.LogWarning("Cannot pass radius of {Radius} if CurrentLocation is not set.", request.Radius);
                    request.Radius = null;
                }

                if (request.TravelTime != null)
                {
                    this.logger.LogWarning("Cannot pass travel time of {TravelTime} if CurrentLocation is not set.", request.TravelTime);
                    request.TravelTime = null;
                }
            }
            else
            {
                //  We have a location search, check radius or traveltime is valid
                if (request.Radius < 0)
                {
                    this.logger.LogWarning("Cannot pass negative radius of {Radius}.", request.Radius);
                    request.Radius = null;
                }

                if (request.TravelTime < 5 ||
                    request.TravelTime > 180)
                {
                    this.logger.LogWarning("Cannot pass travel time of less than 5 mins or more than 180 mins {TravelTime}.", request.TravelTime);
                    request.TravelTime = null;
                }

                // If both null then clear current location
                if (request.Radius == null &&
                    request.TravelTime == null)
                {
                    this.logger.LogWarning("Cannot pass either TravelTime or Radius to use CurrentLocation.");
                    request.CurrentLocation = null;
                }
                else
                {
                    // If both passed clear TravelTime
                    if (request.Radius > 0 &&
                        request.TravelTime > 5)
                    {
                        this.logger.LogWarning("Cannot pass both TravelTime {TravelTime} and Radius {Radius} with CurrentLocation, only Radius will be passed.", request.TravelTime, request.Radius);
                        request.TravelTime = null;
                    }

                    // Check facets
                    if (request.DesiredLocation?.Count() > 0)
                    {
                        this.logger.LogWarning("Cannot pass DesiredLocation facet with CurrentLocation.");
                        request.CurrentLocation = null;
                    }

                    if (request.Towns?.Count() > 0)
                    {
                        this.logger.LogWarning("Cannot pass Towns facet with CurrentLocation.");
                        request.CurrentLocation = null;
                    }
                }
            }

            return request;
        }

        Task<SearchResponse> IStepStoneService.Search(
            StepStoneToken token,
            string source,
            SearchRequest search,
            bool includeFacets,
            bool includeCandidatesActivity) =>
            this.RequestJson<SearchResponse>(
                source,
                HttpMethod.Post,
                includeFacets && includeCandidatesActivity ? "Search?includeFacets=true&includeCandidatesActivity=true" :
                includeFacets ? "Search?includeFacets=true" :
                includeCandidatesActivity ? "Search?includeCandidatesActivity=true" :
                "Search",
                token,
                this.Clean(search));

        Task<FoundCandidate> IStepStoneService.Candidate(StepStoneToken token, string source, long id) =>
            this.RequestJson<FoundCandidate>(
                source, HttpMethod.Get, $"Candidate/{id}", token);


        Task<IEnumerable<StepStoneCV>> IStepStoneService.CV(StepStoneToken token, string source, long[] ids) =>
            this.Request(
                source, HttpMethod.Get, $"cv/{string.Join(',', ids)}", token, ZipResponseUtility.ParseCV);

        async Task<StepStoneCV> IStepStoneService.CV(StepStoneToken token, string source, long id)
        {
            var cvs = await (this as IStepStoneService).CV(token, source, new long[1] { id });
            var cv = cvs.FirstOrDefault(c => c.ID == id); // Try for matching ID first
            if (cv != null) return cv;

            cv = cvs.FirstOrDefault();
            if (cv != null)
            {
                // If ID's mismatch assume that we failed to parse the file entry name, log warning and override
                this.logger.LogWarning("CV File ID not matched: {Expected}!={Actual} {FileName}", id, cv.ID, cv.Filename);
                cv.ID = id;
            }

            return cv;
        }

        Task<IEnumerable<string>> IStepStoneService.Dictionary(StepStoneToken token, string source, StepStoneDictionary dictionary) =>
            this.RequestJson<IEnumerable<string>>(
                source, HttpMethod.Get, $"Dictionary/{dictionary.ToString()}", token);

        Task<QuotaResponse> IStepStoneService.Quota(StepStoneToken token, string source) =>
            this.RequestJson<QuotaResponse>(source, HttpMethod.Get, "Usage/Quota", token);

        /// <summary>Request data from the StepStone API and parse the response as JSON.</summary>
        /// <typeparam name="R">The result type to expect.</typeparam>
        /// <param name="provider">The name of the source provider in the settings.</param>
        /// <param name="method">The HTTP verb to use.</param>
        /// <param name="path">The action to call (no "/" at start).</param>
        /// <param name="content">Optional content to send if POST</param>
        /// <param name="token">Security details for the StepStone session.</param>
        /// <returns>The response from the action.</returns>
        Task<R> RequestJson<R>(
            string provider, HttpMethod method, string path,
            StepStoneToken token,
            object content = null) =>
            this.Request(provider, method, path, token,
                r => r.Content.ReadAsAsync<R>(SearchMediaFormatter), content);

        async Task<R> Request<R>(
            string provider, HttpMethod method, string path,
            StepStoneToken token,
            Func<HttpResponseMessage, Task<R>> parse,
            object content = null)
        {
            var setting = this.settings.FirstOrDefault(s => string.Equals(s.Name, provider, StringComparison.OrdinalIgnoreCase));

            if (setting == null ||
                !token.Tokens.TryGetValue(setting.Name, out var auth))
            {
                this.logger.LogError("Stepstone token not found for {Key}, available: {Tokens}", setting.Name, string.Join(", ", token.Tokens.Keys));
                return default; // Provider not authenticated
            }

            string uri = $"{setting.Url}/CandidateSearchApi/{path}";

            using var request = this.AuthenticatedRequest(method, uri, content, token.User, auth, token.Session);
            using var response = await this.client.SendAsync(request);

            if (response.IsSuccessStatusCode)
                return await parse(response);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new StepStoneAuthenticationException("Token used for authorisation has expired, a new token should be requested.");

            string responseTxt = await response.Content.ReadAsStringAsync();

            if (responseTxt.Contains("\"error_description\""))
                throw new StepStoneServiceException(
                    JsonConvert.DeserializeObject<ErrorService>(responseTxt,
                    (AuthenticationMediaFormatter.First() as JsonMediaTypeFormatter).SerializerSettings));

            ErrorSearch error = null;
            try
            {
                // StepStone sometimes throws errors as { Message: "", ModelType: {} } and sometimes { message: "", model_type: {} }
                foreach (var s in ErrorSerialisers)
                {
                    var parsed = JsonConvert.DeserializeObject<ErrorSearch>(responseTxt, s);
                    error = parsed;
                    if (parsed.ModelState?.Count > 0)
                        break;
                }

                if (string.IsNullOrEmpty(error?.Message))
                    error = new ErrorSearch { Message = responseTxt };
            }
            catch (UnsupportedMediaTypeException umtx)
            {
                this.logger.LogError(umtx, "Unable to parse exception");
                error = new ErrorSearch { Message = responseTxt };
            }

            throw new StepStoneSearchException(error?.Message ?? response.ReasonPhrase, error) { Code = response.StatusCode };
        }

        /// <summary>Create an authenticated request in the format StepStone expects</summary>
        HttpRequestMessage AuthenticatedRequest(
            HttpMethod method, string uri,
            object content,
            string user, string token, string session) =>
            new HttpRequestMessage
            {
                Method = method,
                RequestUri = new Uri(uri),
                Headers = {
                    { "Authorization", $"bearer {token}" },
                    { "client_user_id", user },
                    { "client_device_id", this.application + "+" + session }
                },
                Content = content != null ? new StringContent(
                    JsonConvert.SerializeObject(content, JsonSettings),
                    Encoding.UTF8,
                    "application/json") : null
            };
    }
}