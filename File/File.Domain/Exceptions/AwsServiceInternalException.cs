using System.Net;
using File.Domain.Enums;

namespace File.Domain.Exceptions
{
    public class AwsServiceInternalException : AwsFileException
    {
        private const string MessageTemplate = "Internal Service Error.";

        public AwsServiceInternalException()
            : base(ErrorCode.InternalServiceError, HttpStatusCode.InternalServerError, MessageTemplate)
        {
        }
    }
}
