namespace Evolution.StepStone.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>Search request to send to the /Search endpoint.</summary>
    public sealed class SearchRequest
    {
        /// <summary>How to sort the results.</summary>
        public SearchRequestSort Sort { get; set; } = new SearchRequestSort();

        /// <summary>How many records and what page number.</summary>
        public SearchRequestPage Page { get; set; } = new SearchRequestPage();

        /// <summary>Date of last candidate activity. Either date or facet string.</summary>
        public DateCriteria LastActivityDate { get; set; } = DateTime.Now.AddDays(-90);

        /// <summary>Allowed values: "JobTitle", "ProfileAndCV".</summary>
        public SearchType SearchType { get; set; } = SearchType.ProfileAndCV;

        /// <summary>Allowed values: "SmartSearch" (default) or "ExactMatch".</summary>
        public SearchOption SearchOption { get; set; } = SearchOption.SmartSearch;

        /// <summary>The boolean search text.</summary>
        public string SearchText { get; set; }

        /// <summary>Optional salary limits.</summary>
        public SearchRequestSalary Salary { get; set; }

        /// <summary>Facet parameter, use one of values returned in "SalaryHour" facet.</summary>
        public IEnumerable<string> SalaryHour { get; set; }

        /// <summary>Facet parameter, use one of values returned in "SalaryDay" facet.</summary>
        public IEnumerable<string> SalaryDay { get; set; }

        /// <summary>Facet parameter, use one of values returned in "SalaryAnnual" facet.</summary>
        public IEnumerable<string> SalaryAnnual { get; set; }

        /// <summary>Facet parameter, use one of values returned in "JD" facet.</summary>
        public IEnumerable<string> JobDescription { get; set; }

        /// <summary>Facet parameter, use one of values returned in "Discipline" facet.</summary>
        public IEnumerable<string> Discipline { get; set; }

        /// <summary>Allowed values : /Dictionary/Languages</summary>
        public IEnumerable<string> Languages { get; set; }

        /// <summary>Allowed values : /Dictionary/EducationLevel or "HighestLevelOfEducation" Facet values.</summary>
        public IEnumerable<string> HighestLevelOfEducation { get; set; }

        /// <summary>Allowed values : /Dictionary/Countries or "Countries" facet value</summary>
        public IEnumerable<string> Countries { get; set; }

        /// <summary>Allowed values : /Dictionary/JobHours or "JobHours" facet values.</summary>
        public IEnumerable<string> JobHours { get; set; }

        /// <summary>Allowed values : /Dictionary/DesiredLocations
        /// Important note: cannot be used together with "PostcodeDistance" filter</summary>
        public IEnumerable<string> DesiredLocation { get; set; }

        /// <summary>Not available when searching using DesiredLocations.
        /// Facet parameter, use one of values returned in "Towns" facet.</summary>
        public IEnumerable<string> Towns { get; set; }

        /// <summary>Allowed values : /Dictionary/WorkEligibility</summary>
        public IEnumerable<string> Eligibility { get; set; }

        /// <summary>Allowed values: /Dictionary/SearchableLocations or properly formatted full postcodes or postcode out codes
        /// <para>Important note: Should be used together with <see cref="Radius"/> or <see cref="TravelTime"/>.</para></summary>
        public string CurrentLocation { get; set; }

        /// <summary>Distance expressed in miles.
        /// <para>Important note: Should be used with <see cref="CurrentLocation"/>.</para></summary>
        public int? Radius { get; set; }

        /// <summary>Distance expressed in minutes. 
        /// <para>Important notes:
        /// - Allowed range - &lt;5, 180&gt;
        /// - Should be used with <see cref="CurrentLocation"/></para></summary>
        public int? TravelTime { get; set; }

        /// <summary>Include candidates who have not specified a desired location and are therefore assumed that they are willing to relocate.
        /// <para>Important note: Should be used with <see cref="CurrentLocation"/>.</para></summary>
        public bool? WillingToRelocate { get; set; }

        /// <summary>Allowed values : /Dictionary/HideCandidatesViewed</summary>
        public string HideCandidatesViewedSince { get; set; }

        /// <summary>Whether the candidate holds a driving licence.
        /// Facet parameter, use one of values returned in "DrivingLicence" facet.</summary>
        public IEnumerable<string> DrivingLicence { get; set; }

        /// <summary>Candidates graduation year. Facet parameter, use one of values returned in "GraduationYear" facet.</summary>
        public IEnumerable<string> GraduationYear { get; set; }

        /// <summary>Candidates required notice period. Facet parameter, use one of values returned in "NoticePeriod" facet.</summary>
        public IEnumerable<string> NoticePeriod { get; set; }

        /// <summary></summary>
        public SearchRequestTarget TargetedSearch { get; set; }

        /// <summary>The role types candidates are looking for, Allowed values : /Dictionary/JobTypes</summary>
        public IEnumerable<string> JobTypeProfile { get; set; }
    }
}