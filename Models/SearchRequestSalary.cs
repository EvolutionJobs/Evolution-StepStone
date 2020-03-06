namespace EvoApi.Services.StepStone.Models
{
    public sealed class SearchRequestSalary
    {
        /// <summary>Required, allowed values : /Dictionary/SalaryRateType.</summary>
        public string SalaryRateType { get; set; } = "Annual Salary";

        /// <summary>Required, salary min range (£)</summary>
        public decimal From { get; set; } = 0;

        /// <summary>Required, salary max range (£)</summary>
        public decimal To { get; set; } = 999999;

        /// <summary>Optional property to include candidates with unspecified salaries. If not defined will default to False</summary>
        public bool? Unspecified { get; set; }
    }
}