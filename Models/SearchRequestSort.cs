namespace Evolution.StepStone.Models
{
    public sealed class SearchRequestSort
    {
        /// <summary>Required, true if descending.</summary>
        public bool Descending { get; set; } = true;

        /// <summary>Allowed values: /Dictionary/SortColumns</summary>
        public string Column { get; set; } = "Relevancy";
    }
}