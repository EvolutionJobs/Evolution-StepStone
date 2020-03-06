namespace Evolution.StepStone
{
    using System;
    using System.Collections.Generic;

    /// <summary>Authentication token to pass to StepStone services.</summary>
    public sealed class StepStoneToken
    {
        /// <summary>The client's internal username the token was created for.</summary>
        public string User { get; set; }

        /// <summary>Unique key for this authentication session.</summary>
        public string Session { get; set; }

        /// <summary>Collection of tokens for each service we have login details for.</summary>
        public IDictionary<string, string> Tokens { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Time in seconds until the first token expires.</summary>
        public int Expires { get; set; }
    }
}
