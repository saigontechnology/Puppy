using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using TopCore.Auth.Domain.Exceptions;
using TopCore.WebAPI.Domain.Exceptions;
using TopCore.WebAPI.Domain.ViewModels;

namespace TopCore.WebAPI.Filters
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            ApiErrorViewModel apiErrorViewModel;
            var exception = context.Exception as TopCoreException;

            if (exception != null)
            {
                var ex = exception;
                context.Exception = null;
                apiErrorViewModel = new ApiErrorViewModel(ex.Code, ex.Message);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
            else if (context.Exception is UnauthorizedAccessException)
            {
                apiErrorViewModel = new ApiErrorViewModel(ErrorCode.Unauthorized, "Unauthorized Access");
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

                apiErrorViewModel = new ApiErrorViewModel(ErrorCode.Unknown, msg);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            Log.Logger.Error(context.Exception, apiErrorViewModel.Message);

            // always return a JSON result
            context.Result = new JsonResult(apiErrorViewModel);
            base.OnException(context);
        }
    }
}