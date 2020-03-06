namespace EvoApi.Services.StepStone.Models
{
    public sealed class TokenResponse
    {
        /// <summary>Bearer token to send with every request.</summary>
        public string AccessToken { get; set; }

        /// <summary>Token type, should always be "bearer".</summary>
        public string TokenType { get; set; }

        /// <summary>When the token expires in seconds.</summary>
        public int ExpiresIn { get; set; }
    }
}