#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey </Project>
//     <File>
//         <Name> ServiceCollectionExtensions.cs </Name>
//         <Created> 27/07/17 1:26:59 AM </Created>
//         <Key> 18da2c44-f356-4e52-b761-b9f453d13e90 </Key>
//     </File>
//     <Summary>
//         ServiceCollectionExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Hangfire;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Puppy.Core.EnvironmentUtils;
using System;

namespace Puppy.Hangfire
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     [Background Job] Hangfire store job information in database 
        /// </summary>
        /// <param name="services">                </param>
        /// <param name="databaseConnectionString"></param>
        /// <param name="configuration">           </param>
        /// <param name="configSection">           </param>
        public static IServiceCollection AddHangfire(this IServiceCollection services, string databaseConnectionString, IConfiguration configuration, string configSection = Constant.DefaultConfigSection)
        {
            // Build Config
            configuration.BuildHangfireConfig(configSection);
            services.AddHangfire(config => config.UseSqlServerStorage(databaseConnectionString));
            return services;
        }

        /// <summary>
        ///     [Background Job] Hangfire 
        /// </summary>
        /// <param name="app"></param>
        public static IApplicationBuilder UseHangfire(this IApplicationBuilder app)
        {
            if (string.IsNullOrWhiteSpace(HangfireConfig.DashboardUrl))
            {
                app.UseHangfireDashboard(HangfireConfig.DashboardUrl, new DashboardOptions
                {
                    Authorization = new[] { new CustomAuthorizeFilter() }
                });
            }

            app.UseHangfireServer();

            return app;
        }

        public class CustomAuthorizeFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize([NotNull] DashboardContext context)
            {
                var httpContext = context.GetHttpContext();
                Helper.IsCanAccessHangfireDashboard(httpContext);
                return true;
            }
        }

        public static void BuildHangfireConfig(this IConfiguration configuration, string configSection = Constant.DefaultConfigSection)
        {
            HangfireConfig.DashboardUrl = configuration.GetValue<string>($"{configSection}:{nameof(HangfireConfig.DashboardUrl)}");
            HangfireConfig.AccessKey = configuration.GetValue<string>($"{configSection}:{nameof(HangfireConfig.AccessKey)}");
            HangfireConfig.AccessKeyQueryParam = configuration.GetValue<string>($"{configSection}:{nameof(HangfireConfig.AccessKeyQueryParam)}");

            if (!EnvironmentHelper.IsDevelopment()) return;

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(!string.IsNullOrWhiteSpace(HangfireConfig.DashboardUrl)
                ? $"Hangfire Access Dashboard via Url: {HangfireConfig.DashboardUrl}?{HangfireConfig.AccessKeyQueryParam}={HangfireConfig.AccessKey}"
                : "Hangfire Setup without Dashboard");
            Console.ResetColor();
        }
    }
}