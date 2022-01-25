using System;
using System.Net;
using File.Domain.Enums;

namespace File.Domain.Exceptions
{
    public abstract class AwsFileException : Exception
    {
        protected AwsFileException(ErrorCode errorCode, HttpStatusCode statusCode, string roleName)
            : base(roleName)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }

        public ErrorCode ErrorCode { get; }
        public HttpStatusCode StatusCode { get; }
    }
}
