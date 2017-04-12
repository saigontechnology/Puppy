using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;
using TopCore.Auth.Domain.Exceptions;
using TopCore.Auth.Domain.ViewModels;

namespace TopCore.Auth.Filters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            ApiErrorViewModel apiError;
            var exception = context.Exception as TopCoreException;

            if (exception != null)
            {
                var ex = exception;
                context.Exception = null;
                apiError = new ApiErrorViewModel(ex.Code, ex.Message);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                apiError = new ApiErrorViewModel(ErrorCode.Unauthorized, "Unauthorized Access");
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                // Unhandled errors
#if !DEBUG
                var msg = "An unhandled error occurred.";
#else
                var msg = context.Exception.GetBaseException().Message + Environment.NewLine + context.Exception.StackTrace;
#endif

                apiError = new ApiErrorViewModel(ErrorCode.Unknown, msg);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                context.HttpContext.Response.StatusCode = 500;
            }

            // handle logging here

            // always return a JSON result
            context.Result = new JsonResult(apiError);
            base.OnException(context);
        }
    }
}