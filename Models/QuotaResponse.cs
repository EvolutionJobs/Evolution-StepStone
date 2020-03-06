namespace Evolution.StepStone.Models
{
    using System;

    public sealed class QuotaResponse
    {
        public bool CompanyUsesCredit { get; set; }

        public int? CandidatesViewed { get; set; }

        public int? CandidatesRemaining { get; set; }

        public DateTime? QuotaRefreshDate { get; set; }
    }
}
