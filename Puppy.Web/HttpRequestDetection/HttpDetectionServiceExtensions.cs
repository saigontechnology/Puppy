#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> HttpDetectionServiceExtensions.cs </Name>
//         <Created> 09/09/17 3:57:26 PM </Created>
//         <Key> 6d993bd7-fb37-45f8-86d2-7067faab0146 </Key>
//     </File>
//     <Summary>
//         HttpDetectionServiceExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Puppy.Core.EnvironmentUtils;

namespace Puppy.Web.HttpRequestDetection
{
    public static class HttpDetectionServiceExtensions
    {
        /// <summary>
        ///     <para> [Http Detection] Ip Address detection enhance for local request </para>
        ///     <para>
        ///         When you try and test this on your local machine, your IP Address will resolve to
        ///         the loop back address(i.e. 127.0.0.1 or::1). A handy way in which you can fool
        ///         ASP.NET Core in thinking the request is coming from somewhere else is by using
        ///         the ForwardedHeadersMiddleware and passing along a X - Forwarded - For header
        ///         with each request.
        ///     </para>
        ///     <para>
        ///         First, register the ForwardedHeadersMiddleware when running in Development mode
        ///         by calling the UseForwardedHeaders extension method. You can pass along an
        ///         instance of ForwardedHeadersOptions for which your need to set the
        ///         ForwardedHeaders to look only for the X-Forwarded - For.
        ///     </para>
        ///     <para>
        ///         When running on IIS, you will also need to set the ForwardLimit to 2.By default
        ///         this is set to 1, but IIS already acts as a reverse proxy and will add a X -
        ///         Forwarded - For header to all requests.If the ForwardLimit is set to 1, then the
        ///         middleware will only pick up the value which was set by IIS, and not the value
        ///         you are passing in. So be sure to set ForwardLimit to 2 - it had me scratching my
        ///         head for a while.
        ///     </para>
        /// </summary>
        /// <param name="app"></param>
        public static IApplicationBuilder UseHttpDetection(this IApplicationBuilder app)
        {
            if (EnvironmentHelper.IsDevelopment())
            {
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.XForwardedFor,

                    // IIS is also tagging a X-Forwarded-For header on, so we need to increase this
                    // limit, otherwise the X-Forwarded-For we are passing along from the browser
                    // will be ignored
                    ForwardLimit = 2
                });
            }

            return app;
        }
    }
}