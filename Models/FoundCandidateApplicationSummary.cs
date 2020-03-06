namespace Evolution.StepStone.Models
{
    using System.Collections.Generic;

    public sealed class FoundCandidateApplicationSummary
    {
        /// <summary>Aggregated salaries</summary>
        public string Salary { get; set; }

        /// <summary>Aggregated locations</summary>
        public IEnumerable<string> Locations { get; set; }

        /// <summary></summary>
        public string NormalisedJobTitle { get; set; }

        /// <summary>Number of applications for given NormalisedJobtitle</summary>
        public int Quantity { get; set; }
    }
}
