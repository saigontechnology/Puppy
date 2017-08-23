#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ViewLogViaUrlAccessFilter.cs </Name>
//         <Created> 23/08/17 10:51:15 PM </Created>
//         <Key> e4973af5-48a0-4904-80cf-47b3a1355916 </Key>
//     </File>
//     <Summary>
//         ViewLogViaUrlAccessFilter.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Puppy.Logger.Filters
{
    public class ViewLogViaUrlAccessFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!Log.IsCanAccessLogViaUrl(context.HttpContext))
            {
                context.HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                context.HttpContext.Response.Headers.Clear();
                context.Result = new EmptyResult();
                return;
            }
            base.OnActionExecuting(context);
        }
    }
}