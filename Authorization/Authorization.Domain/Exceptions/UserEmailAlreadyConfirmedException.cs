using System.Globalization;
using System.Net;
using Authorization.Domain.Models;

namespace Authorization.Domain.Exceptions
{
    public class UserEmailAlreadyConfirmedException : ApplicationException
    {
        private const string MessageTemplate = "User email has already been confirmed.";

        public UserEmailAlreadyConfirmedException()
            : base(ErrorCode.UserEmailAlreadyConfirmed, HttpStatusCode.Found, string.Format(CultureInfo.InvariantCulture, MessageTemplate))
        {
        }
    }
}
