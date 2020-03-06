namespace Evolution.StepStone
{
    using Evolution.StepStone.Models;
    using System;
    using System.Net;

    /// <summary>Exception thrown when unable to log in.</summary>
    public sealed class StepStoneSearchException :
        ApplicationException
    {
        public StepStoneSearchException(string message) : base(message) { }

        public StepStoneSearchException(string message, ErrorSearch detail) : base(message) {
            this.Detail = detail;

            if (detail.ModelState != null)
                foreach (var kvp in detail.ModelState)
                    this.Data[kvp.Key] = kvp.Value;
        }

        public ErrorSearch Detail { get; set; }

        public HttpStatusCode Code { get; set; }
    }
}
