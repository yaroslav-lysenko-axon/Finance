using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using File.Application.ApiError;
using File.Application.Models.Responses;
using File.Domain.Exceptions;
using File.Infrastructure.Logging.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json;
using Serilog;

namespace Test1.Middleware
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
            catch (AwsFileException e)
            {
                var errorCodeDisplayName = e.ErrorCode.GetDisplayName();
                Logger.Error("AwsFileException: {@ErrorCode}", errorCodeDisplayName);

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

                var statusCode = HttpStatusCode.InternalServerError;
                var errorCode = "error";
                var message = "unhandled exception";

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
