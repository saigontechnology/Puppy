#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ProcessingTimeExtensions.cs </Name>
//         <Created> 11/08/17 12:09:26 AM </Created>
//         <Key> 8415bfb1-ee92-411b-b8ae-fa63fd790a08 </Key>
//     </File>
//     <Summary>
//         ProcessingTimeExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Puppy.Web.Middlewares
{
    public static class ProcessingTimeExtensions
    {
        public const string ProcessingTimeResponseHeader = "X-Processing-Time-Milliseconds";

        /// <summary>
        ///     [Response] Information about executed time 
        /// </summary>
        /// <param name="app"></param>
        public static IApplicationBuilder UseProcessingTime(this IApplicationBuilder app)
        {
            app.UseMiddleware<ProcessingTimeMiddleware>();

            return app;
        }

        public class ProcessingTimeMiddleware
        {
            private readonly RequestDelegate _next;

            public ProcessingTimeMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public Task Invoke(HttpContext context)
            {
                var watch = new Stopwatch();
                context.Response.OnStarting(state =>
                {
                    var httpContext = (HttpContext)state;
                    watch.Stop();
                    var elapsedMilliseconds = watch.ElapsedMilliseconds.ToString();
                    httpContext.Response.Headers.Add(ProcessingTimeResponseHeader, elapsedMilliseconds);
                    return Task.CompletedTask;
                }, context);

                watch.Start();
                return _next(context);
            }
        }
    }
}