#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> CorsExtensions.cs </Name>
//         <Created> 11/08/17 1:10:36 AM </Created>
//         <Key> cc7ed707-a978-4de8-b170-19c3f50cf9a3 </Key>
//     </File>
//     <Summary>
//         CorsExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Puppy.Core.EnvironmentUtils;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Puppy.Web.Middlewares.Cros
{
    public static class CrosExtensions
    {
        public const string DefaultConfigSection = "Cros";

        public static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration, string configSection = DefaultConfigSection)
        {
            configuration.BuildCrosConfig(configSection);

            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyOrigin();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowCredentials();

            services.AddCors(options =>
            {
                options.AddPolicy(CrosConfig.PolicyAllowAllName, corsBuilder.Build());
            });

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory(CrosConfig.PolicyAllowAllName));
            });

            return services;
        }

        public static IApplicationBuilder UseCors(this IApplicationBuilder app)
        {
            app.UseCors(CrosConfig.PolicyAllowAllName);
            app.UseMiddleware<CorsMiddlewareMonkey>();

            return app;
        }

        /// <summary>
        ///     This middleware for hot fix current issue of AspNetCore Cors 
        /// </summary>
        public class CorsMiddlewareMonkey
        {
            private readonly RequestDelegate _next;

            public CorsMiddlewareMonkey(RequestDelegate next)
            {
                _next = next;
            }

            public Task Invoke(HttpContext context)
            {
                context.Response.OnStarting(state =>
                {
                    var httpContext = (HttpContext)state;
                    if (!httpContext.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
                        httpContext.Response.Headers.Add("Access-Control-Allow-Origin", CrosConfig.AccessControlAllowOrigin);

                    if (!httpContext.Response.Headers.ContainsKey("Access-Control-Allow-Headers"))
                        httpContext.Response.Headers.Add("Access-Control-Allow-Headers", CrosConfig.AccessControlAllowHeaders);

                    if (!httpContext.Response.Headers.ContainsKey("Access-Control-Allow-Methods"))
                        httpContext.Response.Headers.Add("Access-Control-Allow-Methods", CrosConfig.AccessControlAllowMethods);

                    if (httpContext.Request.Method.Equals("OPTIONS", StringComparison.Ordinal))
                        httpContext.Response.StatusCode = (int)HttpStatusCode.NoContent;

                    return Task.CompletedTask;
                }, context);

                return _next(context);
            }
        }

        public static void BuildCrosConfig(this IConfiguration configuration, string configSection = DefaultConfigSection)
        {
            var isHaveConfig = configuration.GetChildren().Any(x => x.Key == configSection);
            if (isHaveConfig)
            {
                CrosConfig.PolicyAllowAllName = configuration.GetValue($"{configSection}:{nameof(CrosConfig.PolicyAllowAllName)}", CrosConfig.PolicyAllowAllName);

                CrosConfig.AccessControlAllowOrigin = configuration.GetValue($"{configSection}:{nameof(CrosConfig.AccessControlAllowOrigin)}", CrosConfig.AccessControlAllowOrigin);

                CrosConfig.AccessControlAllowHeaders = configuration.GetValue($"{configSection}:{nameof(CrosConfig.AccessControlAllowHeaders)}", CrosConfig.AccessControlAllowHeaders);

                CrosConfig.AccessControlAllowMethods = configuration.GetValue($"{configSection}:{nameof(CrosConfig.AccessControlAllowMethods)}", CrosConfig.AccessControlAllowMethods);
            }

            if (!EnvironmentHelper.IsDevelopment()) return;

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(
                $"API Cros Config: {nameof(CrosConfig.PolicyAllowAllName)}: {CrosConfig.PolicyAllowAllName}," +
                $" {nameof(CrosConfig.AccessControlAllowOrigin)}: {CrosConfig.AccessControlAllowOrigin}," +
                $" {nameof(CrosConfig.AccessControlAllowHeaders)}: {CrosConfig.AccessControlAllowHeaders}," +
                $" {nameof(CrosConfig.AccessControlAllowMethods)}: {CrosConfig.AccessControlAllowMethods}");
            Console.ResetColor();
        }
    }
}