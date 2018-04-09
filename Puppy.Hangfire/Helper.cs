#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> Helper.cs </Name>
//         <Created> 09/08/17 1:43:25 PM </Created>
//         <Key> 9145254e-b134-4f0a-986e-320c95189abd </Key>
//     </File>
//     <Summary>
//         Helper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Http;

namespace Puppy.Hangfire
{
    public static class Helper
    {
        internal const string CookieAccessKeyName = "Hangfire_AccessKey";

        public static bool IsCanAccessHangfireDashboard(HttpContext httpContext)
        {
            if (string.IsNullOrWhiteSpace(HangfireConfig.AccessKey))
            {
                return true;
            }

            string requestKey = httpContext.Request.Query[HangfireConfig.AccessKeyQueryParam];

            requestKey = string.IsNullOrWhiteSpace(requestKey) ? httpContext.Request.Cookies[CookieAccessKeyName] : requestKey;

            var isCanAccess = HangfireConfig.AccessKey == requestKey;

            return isCanAccess;
        }
    }
}