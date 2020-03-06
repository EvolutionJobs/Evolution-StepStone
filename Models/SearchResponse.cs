namespace Evolution.StepStone.Models
{
    using System.Collections.Generic;

    public sealed class SearchResponse
    {
        /// <summary></summary>
        public IEnumerable<FoundCandidate> Candidates { get; set; }

        /// <summary></summary>
        public int TotalResultsCount { get; set; }

        /// <summary></summary>
        public IEnumerable<Facet> Facets { get; set; }
    }
}
