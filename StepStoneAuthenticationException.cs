namespace Evolution.StepStone
{
    using Evolution.StepStone.Models;
    using System;

    /// <summary>Exception thrown when unable to log in.</summary>
    public sealed class StepStoneAuthenticationException :
        ApplicationException
    {
        public StepStoneAuthenticationException(string message) : base(message) { }

        public StepStoneAuthenticationException(ErrorService detail) : base(detail?.Error + ": " + detail?.ErrorDescription) {
            this.Detail = detail;
        }

        public ErrorService Detail { get; set; }
    }
}
