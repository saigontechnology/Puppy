using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace TopCore.Auth.Filters
{
    public class DeveloperAccessFilter : ActionFilterAttribute
    {
        private readonly IConfigurationRoot _configurationRoot;

        public DeveloperAccessFilter(IConfigurationRoot configurationRoot)
        {
            _configurationRoot = configurationRoot;
        }

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

        private bool IsValidDeveloperRequest(HttpContext context)
        {
            try
            {
                var developerKey = _configurationRoot.GetValue<string>("Developers:AccessKey");
                string developerKeyParam = context.Request.Query["key"];
                var isValid = string.IsNullOrWhiteSpace(developerKey) || developerKey == developerKeyParam;
                return isValid;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}