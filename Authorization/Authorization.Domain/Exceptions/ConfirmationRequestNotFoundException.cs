using System.Globalization;
using System.Net;
using Authorization.Domain.Models;

namespace Authorization.Domain.Exceptions
{
    public class ConfirmationRequestNotFoundException : ApplicationException
    {
        private const string MessageTemplate = "Confirmation request not found.";

        public ConfirmationRequestNotFoundException()
            : base(ErrorCode.ConfirmationRequestNotFound, HttpStatusCode.NotFound, string.Format(CultureInfo.InvariantCulture, MessageTemplate))
        {
        }
    }
}
