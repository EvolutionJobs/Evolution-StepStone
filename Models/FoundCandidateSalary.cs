namespace EvoApi.Services.StepStone.Models
{
    public sealed class FoundCandidateSalary
    {
        /// <summary></summary>
        public FoundCandidateSalaryRate RateType { get; set; }

        /// <summary></summary>
        public decimal LowerValue { get; set; }

        /// <summary></summary>
        public decimal UpperValue { get; set; }
    }
}