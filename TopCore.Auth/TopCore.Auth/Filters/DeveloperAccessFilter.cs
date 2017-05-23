using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;

namespace TopCore.Auth.Filters
{
    public class DeveloperAccessFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!IsValidDeveloperRequest(context.HttpContext))
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.HttpContext.Response.Headers.Clear();
                context.Result = new EmptyResult();
                return;
            }
            base.OnActionExecuting(context);
        }

        private static bool IsValidDeveloperRequest(HttpContext context)
        {
            try
            {
                var developerKey = ConfigHelper.GetValue("appsettings.json", "Developers:AccessKey");
                string developerKeyParam = context.Request.Query["key"];
                var isValid = string.IsNullOrWhiteSpace(developerKey) || developerKey == developerKeyParam;
                // Not setup developer enable or it is false then return invalid
                return isValid;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}