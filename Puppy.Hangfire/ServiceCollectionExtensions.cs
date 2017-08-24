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
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Hangfire.Server;
using Hangfire.States;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Puppy.Core.EnvironmentUtils;
using Puppy.Core.ServiceCollectionUtils;
using System;

namespace Puppy.Hangfire
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     [Background Job] Hangfire, auto override existing added hangfire service. 
        /// </summary>
        /// <param name="services">                </param>
        /// <param name="databaseConnectionString"></param>
        /// <param name="configuration">           </param>
        /// <param name="configSection">           </param>
        /// <remarks>
        ///     <see cref="databaseConnectionString" /> is null or empty for store job in memory,
        ///     else in Sql Server. Default is in memory
        /// </remarks>
        public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration, string databaseConnectionString = null, string configSection = Constant.DefaultConfigSection)
        {
            // Build Config
            configuration.BuildHangfireConfig(configSection);

            // Check if already have hangfire service before, let remove to override purpose.
            services.Removes(typeof(JobStorage),
                typeof(IGlobalConfiguration),
                typeof(JobActivator),
                typeof(RouteCollection),
                typeof(IJobFilterProvider),
                typeof(IBackgroundJobFactory),
                typeof(IBackgroundJobStateChanger),
                typeof(IBackgroundJobPerformer),
                typeof(IBackgroundJobClient),
                typeof(IRecurringJobManager),
                typeof(Action<IGlobalConfiguration>));

            // Add Hangfire Service
            if (string.IsNullOrWhiteSpace(databaseConnectionString))
            {
                services.AddHangfire(config =>
                {
                    config.UseMemoryStorage();
                });
            }
            else
            {
                services.AddHangfire(config =>
                {
                    config.UseSqlServerStorage(databaseConnectionString);
                });
            }
            return services;
        }

        /// <summary>
        ///     [Background Job] Hangfire 
        /// </summary>
        /// <param name="app">                 
        ///     The path for the Back To Site link. Set to <see langword="null" /> in order to hide
        ///     the Back To Site link. Default is "/"
        /// </param>
        /// <param name="appPath">             </param>
        /// <param name="statsPollingInterval">
        ///     The interval the /stats endpoint should be polled with (milliseconds). Default is 2000
        /// </param>
        public static IApplicationBuilder UseHangfire(this IApplicationBuilder app, string appPath = "/", int statsPollingInterval = 2000)
        {
            if (!string.IsNullOrWhiteSpace(HangfireConfig.DashboardUrl))
            {
                app.UseHangfireDashboard(HangfireConfig.DashboardUrl, new DashboardOptions
                {
                    Authorization = new[] { new CustomAuthorizeFilter() },
                    AppPath = appPath,
                    StatsPollingInterval = statsPollingInterval
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