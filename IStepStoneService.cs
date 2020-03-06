namespace EvoApi.Services.StepStone
{
    using EvoApi.Services.StepStone.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>Dependency injected proxy for the StepStone Federated Candidate Search API.
    /// <para></para>To inject this call <see cref="Microsoft.Extensions.DependencyInjection.StepStoneExtensions.AddStepStoneService"/> in startup.</para></summary>
    public interface IStepStoneService
    {
        /// <summary>Get authentication tokens for all configured StepStone brands.
        /// </summary>
        /// <param name="username">Unique username to identify this user.</param>
        /// <param name="session">ID for this session, used to troubleshoot issues.</param>
        /// <returns>A token wrapper to pass to every subsequent request.</returns>
        /// <exception cref="StepStoneAuthenticationException">Thrown if the details are not authorised by the service.</exception>
        Task<StepStoneToken> Authenticate(string username, string session);

        /// <summary>Run a candidate search.</summary>
        /// <param name="source">The name of the configured source to use.</param>
        /// <param name="search">The search to send.</param>
        /// <param name="token">Session token aquired from <see cref="Authenticate"/>.</param>
        /// <param name="includeFacets">If set to true, enables Facets in result set.</param>
        /// <param name="includeCandidatesActivity">If set to true, enables Candidate’s application summary data available in search results.</param>
        /// <returns>Search results.</returns>
        /// <exception cref="StepStoneAuthenticationException">Thrown if the user credentials have expired and the service returns a 401</exception>
        /// <exception cref="StepStoneServiceException">Thrown for service exceptions or bad security headers.</exception>
        /// <exception cref="StepStoneSearchException">Thrown for invalid search requests.</exception>
        Task<SearchResponse> Search(
            StepStoneToken token, 
            string source, 
            SearchRequest search,
            bool includeFacets = false,
            bool includeCandidatesActivity = false);

        /// <summary>Get the candidate detail from a provider.
        /// <para>Calling this will use your quota or bill for the candidate.</para></summary>
        /// <param name="source">The name of the configured source to use.</param>
        /// <param name="id">The ID of the candidate.</param>
        /// <param name="token">Session token aquired from <see cref="Authenticate"/>.</param>
        /// <returns>The candidate details including contact information.</returns>
        /// <exception cref="StepStoneAuthenticationException">Thrown if the user credentials have expired and the service returns a 401</exception>
        /// <exception cref="StepStoneServiceException">Thrown for service exceptions or bad security headers.</exception>
        /// <exception cref="StepStoneSearchException">Thrown for invalid search requests.</exception>
        Task<FoundCandidate> Candidate(StepStoneToken token, string source, long id);

        /// <summary>Get a set of candidate CVs
        /// <para>Calling this will use your quota or bill for the candidates.</para></summary>
        /// <param name="source">The name of the configured source to use.</param>
        /// <param name="ids">The IDs of the candidates.</param>
        /// <param name="token">Session token aquired from <see cref="Authenticate"/>.</param>
        /// <returns>A collection of the candidate CV files.</returns>
        /// <exception cref="StepStoneAuthenticationException">Thrown if the user credentials have expired and the service returns a 401</exception>
        /// <exception cref="StepStoneServiceException">Thrown for service exceptions or bad security headers.</exception>
        /// <exception cref="StepStoneSearchException">Thrown for invalid search requests.</exception>
        Task<IEnumerable<StepStoneCV>> CV(StepStoneToken token, string source, long[] ids);

        /// <summary>Get a single candidate CV.
        /// <para>Calling this will use your quota or bill for the candidate.</para></summary>
        /// <param name="source">The name of the configured source to use.</param>
        /// <param name="id">The ID of the candidate.</param>
        /// <param name="token">Session token aquired from <see cref="Authenticate"/>.</param>
        /// <returns>The candidate's CV file.</returns>
        /// <exception cref="StepStoneAuthenticationException">Thrown if the user credentials have expired and the service returns a 401</exception>
        /// <exception cref="StepStoneServiceException">Thrown for service exceptions or bad security headers.</exception>
        /// <exception cref="StepStoneSearchException">Thrown for invalid search requests.</exception>
        Task<StepStoneCV> CV(StepStoneToken token, string source, long id);

        /// <summary>Get the quota's available to the given token.</summary>
        /// <param name="token">Session token aquired from <see cref="Authenticate"/>.</param>
        /// <param name="source">The name of the configured source to use.</param>
        /// <returns>Quota for each configured source.</returns>
        /// <exception cref="StepStoneAuthenticationException">Thrown if the user credentials have expired and the service returns a 401</exception>
        /// <exception cref="StepStoneServiceException">Thrown for service exceptions or bad security headers.</exception>
        /// <exception cref="StepStoneSearchException">Thrown for invalid search requests.</exception>
        Task<QuotaResponse> Quota(StepStoneToken token, string source);

        /// <summary>Get a single dictionary</summary>
        /// <param name="source">The name of the configured source to use.</param>
        /// <param name="dictionary">StepStone dictionary.</param>
        /// <param name="token">Session token aquired from <see cref="Authenticate"/>.</param>
        /// <returns>The dictionary values.</returns>
        /// <exception cref="StepStoneAuthenticationException">Thrown if the user credentials have expired and the service returns a 401</exception>
        /// <exception cref="StepStoneServiceException">Thrown for service exceptions or bad security headers.</exception>
        /// <exception cref="StepStoneSearchException">Thrown for invalid search requests.</exception>
        Task<IEnumerable<string>> Dictionary(StepStoneToken token, string source, StepStoneDictionary dictionary);
    }
}