using System.Globalization;
using System.Net;
using Authorization.Domain.Models;

namespace Authorization.Domain.Exceptions
{
    public class ConfirmationRequestExpiredException : ApplicationException
    {
        private const string MessageTemplate = "Confirmation request is expired.";

        public ConfirmationRequestExpiredException()
            : base(ErrorCode.ConfirmationRequestExpired, HttpStatusCode.Found, string.Format(CultureInfo.InvariantCulture, MessageTemplate))
        {
        }
    }
}
