namespace Evolution.StepStone.Models
{
    using System;
    using System.Collections.Generic;

    public sealed class ErrorSearch
    {
        public string Message { get; set; }

        public IDictionary<string, IEnumerable<string>> ModelState { get; set; } = 
            new Dictionary<string, IEnumerable<string>>(StringComparer.OrdinalIgnoreCase);
    }
}