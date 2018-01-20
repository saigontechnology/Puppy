#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> HttpContextAccessorExtensions.cs </Name>
//         <Created> 29/08/17 12:56:32 AM </Created>
//         <Key> ecf93000-e938-462a-b3c6-59b669aaf009 </Key>
//     </File>
//     <Summary>
//         HttpContextAccessorExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Puppy.Core.ServiceCollectionUtils;

namespace System.Web
{
    public static class HttpContext
    {
        private static IHttpContextAccessor _contextAccessor;

        public static Microsoft.AspNetCore.Http.HttpContext Current => _contextAccessor?.HttpContext;

        internal static void Configure(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
    }
}

namespace Puppy.Web.Middlewares
{
    public static class HttpContextAccessorExtensions
    {
        public static IServiceCollection AddHttpContextAccessor(this IServiceCollection services)
        {
            services.AddSingletonIfNotExist<IHttpContextAccessor, HttpContextAccessor>();
            return services;
        }

        /// <summary>
        ///     [Response] Information about executed time 
        /// </summary>
        /// <param name="app"></param>
        public static IApplicationBuilder UseHttpContextAccessor(this IApplicationBuilder app)
        {
            var httpContextAccessor = app.ApplicationServices.GetService<IHttpContextAccessor>();

            System.Web.HttpContext.Configure(httpContextAccessor);

            return app;
        }
    }
}