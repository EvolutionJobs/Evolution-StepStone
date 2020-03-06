namespace Evolution.StepStone.Models
{
    using System;

    public sealed class FoundCandidateApplications
    {
        /// <summary>Number of applications since LastActivityDate in search criteria (limited to 3 months from now)</summary>
        public int Count { get; set; }

        /// <summary>Date used for calculating applications count</summary>
        public DateTime? Since { get; set; }

        /// <summary></summary>
        public FoundCandidateSalaryRate RateType { get; set; }
    }
}
