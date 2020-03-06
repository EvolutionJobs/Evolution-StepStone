namespace EvoApi.Services.StepStone.Models
{
    using System;
    using System.Collections.Generic;

    public class FoundCandidate
    {
        /// <summary></summary>
        public long Id { get; set; }

        /// <summary></summary>
        public int Relevancy { get; set; }

        /// <summary></summary>
        public string ForeName { get; set; }

        /// <summary></summary>
        public string Surname { get; set; }

        /// <summary></summary>
        public string DesiredJobTitle { get; set; }

        /// <summary></summary>
        public string CurrentJobTitle { get; set; }

        /// <summary></summary>
        public string WorkExperience { get; set; }

        /// <summary></summary>
        public string Title { get; set; }

        /// <summary></summary>
        public string PersonalSummary { get; set; }

        /// <summary></summary>
        public string CvSnippet { get; set; }

        /// <summary></summary>
        public string KeySkills { get; set; }

        /// <summary></summary>
        public string LandlinePhone { get; set; }

        /// <summary></summary>
        public string MobilePhone { get; set; }

        /// <summary></summary>
        public string Email { get; set; }

        /// <summary>This flag informs that candidate profile is anonymised. Information like FirstName, LastName, Phone and Email are not being returned. 
        /// "CV preview" and "Download CV" options are not available for such candidates.</summary>
        public bool IsAnonymous { get; set; }

        public IEnumerable<WebsiteLink> WebsiteLinks { get; set; }

        /// <summary>Candidate's current location</summary>
        public string CurrentLocation { get; set; }

        /// <summary></summary>
        public IEnumerable<string> DesiredLocations { get; set; }

        /// <summary>Distance from postcode in search criteria, if postcode not defined distance always is equal 0</summary>
        public double Distance { get; set; } = 0;

        /// <summary>Candidate's current salary</summary>
        public FoundCandidateSalary CurrentSalary { get; set; }

        /// <summary>Candidate's desired salary</summary>
        public FoundCandidateSalary DesiredSalary { get; set; }

        /// <summary>If the candidate has requested their application data be hidden then the endpoint will return no data</summary>
        public FoundCandidateApplications ApplicationsData { get; set; }

        /// <summary>Details of when this candidate was last viewed by us.</summary>
        public FoundCandidateViewInfo ViewInfo { get; set; }

        /// <summary>Summary of candidate’s applications grouped by normalised job title.
        /// If the candidate has requested their application data be hidden then the endpoint will return no data
        /// </summary>
        public IEnumerable<FoundCandidateApplicationSummary> ApplicationsSummary { get; set; }

        /// <summary>Minutes required for candidate to reach provided <see cref="SearchRequest.CurrentLocation"/> by car. 
        /// (Only for TravelTime search, otherwise value is always 0)</summary>
        public int TravelTimeByCar { get; set; } = 0;

        /// <summary>Minutes required for candidate to reach provided <see cref="SearchRequest.CurrentLocation"/> by public transport. 
        /// (Only for TravelTime search, otherwise value is always 0)</summary>
        public int TravelTimeByPublicTransport { get; set; } = 0;

        /// <summary>The date of their last application or profile update date, whichever is the more recent.</summary>
        public DateTime? LastActiveDate { get; set; }

        /// <summary>Candidates requested job type as defined by the values outlined by the dictionary endpoint /Dictionary/JobTypes</summary>
        public IEnumerable<string> JobType { get; set; }

        /// <summary>When the candidate registered their profile.</summary>
        public DateTime? Registered { get; set; }
    }
}
