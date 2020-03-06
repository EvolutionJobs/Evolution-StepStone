namespace EvoApi.Services.StepStone.Models
{
    using System;

    public sealed class FoundCandidateViewInfo
    {
        /// <summary>Date when currently logged in recruiter or recruiter from the same company viewed candidate’s profile</summary>
        public DateTime? ViewDate { get; set; }

        /// <summary>Available values: “None”, “Me”, “OtherRecruiter” (from the same company)</summary>
        public ViewedBySource ViewedBy { get; set; }

        /// <summary>Candidate profile has been updated since ViewDate</summary>
        public bool UpdatedSinceViewed { get; set; }
    }
}
