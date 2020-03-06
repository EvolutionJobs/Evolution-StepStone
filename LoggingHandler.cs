namespace EvoApi.Services.StepStone
{
    using Microsoft.Extensions.Logging;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Diagnostics;

    /// <summary>HTTP request wrapper that logs request/response body.
    /// Use this for debugging issues with third party services.</summary>
    class LoggingHandler : DelegatingHandler
    {
        readonly ILogger logger;

        public LoggingHandler(HttpMessageHandler innerHandler, ILogger logger)
            : base(innerHandler)
        {
            this.logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (this.logger?.IsEnabled(LogLevel.Trace) ?? false)
            {
                if (request.Content == null)
                    this.logger.LogInformation(@"Request {Method} {URI} 
Headers:{Headers}", request.Method, request.RequestUri, ReadableHeaders(request.Headers));
                else
                    this.logger.LogInformation(@"Request {Method} {URI} 
Headers:{Headers}
{Content}", request.Method, request.RequestUri, ReadableHeaders(request.Headers), await request.Content.ReadAsStringAsync());
            }
            else
            {
                if (request.Content == null)
                    this.logger.LogInformation("Request {Method} {URI}", request.Method, request.RequestUri);
                else
                    this.logger.LogInformation(@"Request {Method} {URI} 
{Content}", request.Method, request.RequestUri, await request.Content.ReadAsStringAsync());
            }

            var watch = new Stopwatch();
            watch.Start();
            var response = await base.SendAsync(request, cancellationToken);
            watch.Stop();
            long duration = watch.ElapsedMilliseconds;

            if (this.logger?.IsEnabled(LogLevel.Trace) ?? false)
            {
                if (response.Content == null)
                    this.logger.LogInformation(@"Response ({Duration}ms) {Code} {Status} 
Headers:{Headers}", duration, (int)response.StatusCode, response.ReasonPhrase, ReadableHeaders(response.Headers));
                else
                    this.logger.LogInformation(@"Response ({Duration}ms) {Code} {Status} 
Headers:{Headers}
{Content}", duration, (int)response.StatusCode, response.ReasonPhrase, ReadableHeaders(response.Headers), await ReadableResponse(response));
            }
            else if (this.logger?.IsEnabled(LogLevel.Information) ?? false)
            {
                if (response.Content == null)
                    this.logger.LogInformation("Response ({Duration}ms) {Code} {Status}", duration, (int)response.StatusCode, response.ReasonPhrase);
                else
                    this.logger.LogInformation(@"Response ({Duration}ms) {Code} {Status}
{Content}", duration, (int)response.StatusCode, response.ReasonPhrase, await ReadableResponse(response));
            }
            else 
                this.logger.LogInformation(@"Response ({Duration}ms) {Code} {Status}",
                    duration, (int)response.StatusCode, response.ReasonPhrase);

            return response;
        }

        static async Task<string> ReadableResponse(HttpResponseMessage response)
        {
            var contentType = response.Content.Headers.GetValues("Content-Type").FirstOrDefault();
            if (contentType == "application/zip")
                return $"ZIP file {response.Content.Headers.GetValues("Content-Length").FirstOrDefault()} bytes";

            return await response.Content.ReadAsStringAsync();
        }

        static string ReadableHeaders(HttpHeaders headers) =>
            string.Join("\r\n",
                from h in headers
                select $"\t\"{h.Key}\": {string.Join(' ', h.Value)}");
    }
}