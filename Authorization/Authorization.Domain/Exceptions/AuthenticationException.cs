using System.Net;
using Authorization.Domain.Models;

namespace Authorization.Domain.Exceptions
{
    public class AuthenticationException : ApplicationException
    {
        private const string MessageTemplate = "The refresh token has expired.";

        public AuthenticationException()
            : base(ErrorCode.ClientAuthorizationFailed, HttpStatusCode.Unauthorized, MessageTemplate)
        {
        }
    }
}
