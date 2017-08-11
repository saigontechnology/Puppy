#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> RequestRewindExtensions.cs </Name>
//         <Created> 11/08/17 6:50:15 PM </Created>
//         <Key> 737e48b8-760d-4edb-b88d-7d451156feb5 </Key>
//     </File>
//     <Summary>
//         RequestRewindExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System.Threading.Tasks;

namespace Puppy.Web.Middlewares
{
    public static class RequestRewindExtensions
    {
        public static IApplicationBuilder UseRequestRewind(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestRewindMiddleware>();
        }

        public class RequestRewindMiddleware
        {
            private readonly RequestDelegate _next;

            public RequestRewindMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public Task Invoke(HttpContext context)
            {
                // Allows using several time the stream in ASP.Net Core. Enable Rewind help to get
                // Request Body content.
                context.Request.EnableRewind();

                return _next(context);
            }
        }
    }
}