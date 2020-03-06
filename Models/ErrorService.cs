namespace Evolution.StepStone.Models
{
    public sealed class ErrorService
    {
        /// <summary>Error type, expected to be:
        /// <para>"invalid_request" - something required (client_id, client_secret, username or password) wasn't provided.</para>
        /// <para>"invalid_grant" - recruiter account has been blocked due to exceeded number of incorrect logins, 
        /// or recruiter does not have privileges to access requested data, 
        /// or name or password is incorrect.</para>
        /// <para>"unsupported_grant_type" - grant_type has not been provided or has incorrect value.</para>
        /// <para>"invalid_scope" - scope has not been provided or has incorrect value.</para>
        /// <para>"invalid_client" - client_id or client_secret is incorrect.</para>
        /// <para>"server_error" - response contains additional property "error_id" that can be used to troubleshoot error by TJG.</para>
        /// </summary>
        public string Error { get; set; }

        /// <summary>Long description of the error.</summary>
        public string ErrorDescription { get; set; }
    }
}