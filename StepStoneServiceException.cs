namespace EvoApi.Services.StepStone
{
    using EvoApi.Services.StepStone.Models;
    using System;

    /// <summary>Exception thrown by the remote StepStone service.</summary>
    public sealed class StepStoneServiceException :
        ApplicationException
    {
        public StepStoneServiceException(string message) : base(message) { }

        public StepStoneServiceException(ErrorService detail) : base(detail?.Error + ": " + detail?.ErrorDescription) {
            this.Detail = detail;
        }

        public ErrorService Detail { get; set; }
    }
}
