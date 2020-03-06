namespace Evolution.StepStone.Models
{
    public sealed class FacetItem
    {
        /// <summary>Total number of candidates that have given value selected in their profile.</summary>
        public int Count { get; set; }

        /// <summary>Value for given facet.</summary>
        public string Value { get; set; }

        /// <summary>Flag that indicates if given facet value was used in request model.</summary>
        public bool IsSelected { get; set; }
    }
}
