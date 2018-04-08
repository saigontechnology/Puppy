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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Puppy.Core.ServiceCollectionUtils;
using Puppy.Web.HttpUtils;
using System;
using System.Threading.Tasks;

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

        public static IApplicationBuilder UseHangfire(this IApplicationBuilder app)
        {
            if (!string.IsNullOrWhiteSpace(HangfireConfig.DashboardUrl))
            {
                app.UseMiddleware<HangfireDashboardAccessMiddleware>();

                app.UseHangfireDashboard(HangfireConfig.DashboardUrl, new DashboardOptions
                {
                    Authorization = new[] { new CustomAuthorizeFilter() },
                    AppPath = HangfireConfig.BackToSiteUrl,
                    StatsPollingInterval = HangfireConfig.StatsPollingInterval
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
                return Helper.IsCanAccessHangfireDashboard(httpContext);
            }
        }

        public class HangfireDashboardAccessMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly RouteCollection _routeCollection;

            public HangfireDashboardAccessMiddleware(RequestDelegate next, RouteCollection routeCollection)
            {
                _next = next;
                _routeCollection = routeCollection;
            }

            public async Task Invoke(HttpContext context)
            {
                // Check is request to Job Dashboard
                var route = _routeCollection.FindDispatcher(context.Request.Path.Value.Replace(HangfireConfig.DashboardUrl, string.Empty));

                var dashboardRequestUrl = route == null ? HangfireConfig.DashboardUrl : $@"{HangfireConfig.DashboardUrl}/{route.Item2.Value.Trim('/')}";

                var isRequestToHangfireDashboard = context.Request.IsRequestFor(dashboardRequestUrl);

                if (route == null || !isRequestToHangfireDashboard)
                {
                    await _next.Invoke(context).ConfigureAwait(true);

                    return;
                }

                // Set cookie if need
                string requestAccessKey = context.Request.Query[HangfireConfig.AccessKeyQueryParam];

                if (!string.IsNullOrWhiteSpace(requestAccessKey) && context.Request.Cookies[HangfireConfig.AccessKeyQueryParam] != requestAccessKey)
                {
                    SetCookie(context, Helper.CookieAccessKeyName, requestAccessKey);
                }

                // Check Permission
                bool isCanAccess = Helper.IsCanAccessHangfireDashboard(context);

                if (!isCanAccess)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                    await context.Response.WriteAsync(HangfireConfig.UnAuthorizeMessage).ConfigureAwait(true);

                    return;
                }

                await _next.Invoke(context).ConfigureAwait(true);
            }

            private static void SetCookie(HttpContext context, string key, string value)
            {
                context.Response.Cookies.Append(key, value, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false // allow transmit via http and https
                });
            }
        }

        public static void BuildHangfireConfig(this IConfiguration configuration, string configSection = Constant.DefaultConfigSection)
        {
            HangfireConfig.DashboardUrl = configuration.GetValue($"{configSection}:{nameof(HangfireConfig.DashboardUrl)}", HangfireConfig.DashboardUrl);
            if (!HangfireConfig.DashboardUrl.StartsWith("/"))
            {
                throw new ArgumentException($"{nameof(HangfireConfig.DashboardUrl)} must start by /", nameof(HangfireConfig.DashboardUrl));
            }

            HangfireConfig.AccessKey = configuration.GetValue($"{configSection}:{nameof(HangfireConfig.AccessKey)}", HangfireConfig.AccessKey);
            HangfireConfig.AccessKeyQueryParam = configuration.GetValue($"{configSection}:{nameof(HangfireConfig.AccessKeyQueryParam)}", HangfireConfig.AccessKeyQueryParam);
            HangfireConfig.UnAuthorizeMessage = configuration.GetValue($"{configSection}:{nameof(HangfireConfig.UnAuthorizeMessage)}", HangfireConfig.UnAuthorizeMessage);
            HangfireConfig.BackToSiteUrl = configuration.GetValue($"{configSection}:{nameof(HangfireConfig.BackToSiteUrl)}", HangfireConfig.BackToSiteUrl);
            HangfireConfig.StatsPollingInterval = configuration.GetValue($"{configSection}:{nameof(HangfireConfig.StatsPollingInterval)}", HangfireConfig.StatsPollingInterval);
        }
    }
}