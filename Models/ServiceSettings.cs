namespace EvoApi.Services.StepStone.Models
{
    public sealed class ServiceSettings
    {
        /// <summary>The name of the service</summary>
        public string Name { get; set; }

        /// <summary>URL to connect to</summary>
        public string Url { get; set; }

        /// <summary>The aggregator's ID provided by STST UK</summary>
        public string ClientID { get; set; }

        /// <summary>The aggregator's secret provided by STST UK</summary>
        public string ClientSecret { get; set; }

        /// <summary>The recruiter's username with the search provider.</summary>
        public string RecruiterUsername { get; set; }

        /// <summary>The recruiter's password with the search provider.</summary>
        public string RecruiterPassword { get; set; }
    }
}