namespace Evolution.StepStone.Models
{
    public sealed class SearchRequestTarget
    {
        /// <summary>Allowed values : /Dictionary/JobTitleType</summary>
        public string JobTitleType { get; set; } = "Current";

        /// <summary></summary>
        public string SearchJobTitles { get; set; }

        /// <summary></summary>
        public string SearchSkills { get; set; }

        /// <summary></summary>
        public string SearchCvProfile { get; set; }

        /// <summary>Allowed values : /Dictionary/CvProfileType</summary>
        public string CvProfileType { get; set; } = "Both";
    }
}