using System;
using System.Net;
using Authorization.Domain.Models;

namespace Authorization.Domain.Exceptions
{
    public abstract class ApplicationException : Exception
    {
        protected ApplicationException(ErrorCode errorCode, HttpStatusCode statusCode, string roleName)
            : base(roleName)
        {
            ErrorCode = errorCode;
            StatusCode = statusCode;
        }

        public ErrorCode ErrorCode { get; }
        public HttpStatusCode StatusCode { get; }
    }
}
