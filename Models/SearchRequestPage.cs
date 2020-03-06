namespace EvoApi.Services.StepStone.Models
{
    public sealed class SearchRequestPage
    {
        /// <summary>Required, Max number of candidates to be returned.
        /// Must be between 1 and 50.</summary>
        public int MaxRecords { get; set; } = 50;

        /// <summary>Required, 1-indexed, results page to be returned</summary>
        public int Page { get; set; } = 1;
    }
}