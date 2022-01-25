using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Authorization.Application.ApiError;
using Authorization.Application.Extensions;
using Authorization.Application.Models.Responses;
using Authorization.Infrastructure.Logging.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using ApplicationException = Authorization.Domain.Exceptions.ApplicationException;

namespace Authorization.Host.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private static readonly ILogger Logger = Log.ForContext<ExceptionHandlingMiddleware>();
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ApplicationException e)
            {
                var errorCodeDisplayName = e.ErrorCode.GetDisplayName();
                Logger.Error("ApplicationException: {@ErrorCode}", errorCodeDisplayName);

                if (context.Response.HasStarted)
                {
                    throw;
                }

                context.Response.Clear();

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)e.StatusCode;

                var errorResponse = new ErrorResponse
                {
                    Code = errorCodeDisplayName,
                    Message = e.Message,
                };

                var error = JsonConvert.SerializeObject(errorResponse);

                await context.Response.WriteAsync(error);
            }
            catch (Exception exception)
            {
                var exceptionInfo = ExceptionInfo.Create(exception);
                Logger.Error("Exception: {@ExceptionInfo}", exceptionInfo);

                if (context.Response.HasStarted)
                {
                    throw;
                }

                HttpStatusCode statusCode;
                string errorCode, message;

                switch (context.Response.StatusCode)
                {
                    case 401:
                        statusCode = HttpStatusCode.Unauthorized;
                        errorCode = "unauthorized_access";
                        message = "Not authorized";
                        break;
                    case 403:
                        statusCode = HttpStatusCode.Forbidden;
                        errorCode = "forbidden_access";
                        message = "Forbidden";
                        break;
                    default:
                        statusCode = HttpStatusCode.InternalServerError;
                        errorCode = "error";
                        message = "unhandled exception";
                        break;
                }

                var operationId = Activity.Current?.RootId;
                var apiError = new ApiError()
                {
                    OperationId = operationId,
                    Code = errorCode,
                    Message = message,
                };

                var response = new ApiErrorResponse() { Error = apiError };
                var payload = JsonConvert.SerializeObject(response);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;
                await context.Response.WriteAsync(payload);
            }
        }
    }
}
