namespace EvoApi.Services.StepStone.Models
{
    using System.Collections.Generic;
    
    public sealed class Facet
    {
        /// <summary>Unique facet type strong name.</summary>
        public string Type { get; set; }

        /// <summary>Aggregated counts in this facet.</summary>
        public IEnumerable<FacetItem> Items { get; set; }
    }
}
