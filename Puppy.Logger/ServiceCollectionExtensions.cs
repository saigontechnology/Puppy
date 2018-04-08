#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <DisplayUrl> http://topnguyen.net/ </DisplayUrl>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ServiceCollectionExtensions.cs </Name>
//         <Created> 10/08/17 10:59:13 AM </Created>
//         <Key> 490f84b2-5411-4ea1-9c46-eda3bd8cf0b5 </Key>
//     </File>
//     <Summary>
//         ServiceCollectionExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Puppy.Core.EnvironmentUtils;
using Puppy.Logger.Core.Models;
using Puppy.Web.HttpUtils;
using Puppy.Web.Models;
using Serilog;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puppy.Logger
{
    public static class ServiceCollectionExtensions
    {
        private static IConfiguration _configuration;
        private static string _configSection;

        /// <summary>
        ///     Add Logger Service with Hangfire on Memory if it not added by another service before.
        /// </summary>
        /// <param name="services">     </param>
        /// <param name="configuration"></param>
        /// <param name="configSection"></param>
        /// <returns></returns>
        public static IServiceCollection AddLogger(this IServiceCollection services, IConfiguration configuration, string configSection = Constant.DefaultConfigSection)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _configSection = configSection;

            // Check if don't have hangfire service, let add to run background job.
            if (services.All(x => x.ServiceType != typeof(JobStorage)))
            {
                services.AddHangfire(x => x.UseMemoryStorage());
            }

            return services;
        }

        /// <summary>
        ///     <para>
        ///         Use Puppy Logger, remember to <see cref="AddLogger" /> before <c> UseLogger </c>.
        ///         In Configure of Application Builder, you need inject "IApplicationLifetime
        ///         appLifetime" to use Logger.
        ///     </para>
        ///     <para>
        ///         You can access log via URL config by <see cref="LoggerConfig.ViewLogUrl" /> and
        ///         Search for <see cref="LogEntity.Id" />, <see cref="LogEntity.Message" />,
        ///         <see cref="LogEntity.Level" />, <see cref="LogEntity.CreatedTime" /> (with string
        ///         format is <c> "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK" </c>, ex: "2017-08-24T00:56:29.6271125+07:00")
        ///     </para>
        /// </summary>
        /// <param name="app">          </param>
        /// <param name="loggerFactory"></param>
        /// <param name="appLifetime">   Ensure any buffered events are sent at shutdown </param>
        /// <returns></returns>
        /// <remarks>
        ///     <para>
        ///         Auto add request header <c> "Id" </c>, <c> "RequestTime" </c> and <c>
        ///         EnableRewind </c> for Request to get <c> Request Body </c> when logging.
        ///     </para>
        ///     <para>
        ///         The file will be written using the <c> UTF-8 encoding </c> without a byte-order mark.
        ///     </para>
        ///     <para> Auto reload config when the appsettings.json changed </para>
        /// </remarks>
        public static IApplicationBuilder UseLogger(this IApplicationBuilder app, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            // Re-Build the config for Logger - parameters
            _configuration.BuildLoggerConfig(_configSection);

            // Build the config for Logger - methods
            Log.BuildLogger();

            // Add Logger for microsoft logger factory
            loggerFactory.AddSerilog();

            // Ensure any buffered events are sent at shutdown
            appLifetime.ApplicationStopped.Register(Serilog.Log.CloseAndFlush);

            // Add middleware to inject Request Id
            app.UseMiddleware<LoggerMiddleware>();

            ChangeToken.OnChange(_configuration.GetReloadToken, () =>
            {
                // Re-Build the config for Logger - parameters
                _configuration.BuildLoggerConfig();

                // Re-Build the config for Logger - methods
                Log.BuildLogger();

                Log.Warning("Puppy Logger Changed Configuration");
            });

            return app;
        }

        public class LoggerMiddleware
        {
            private readonly RequestDelegate _next;

            public LoggerMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task Invoke(HttpContext context)
            {
                // Add Request Id if not have already.
                if (!context.Request.Headers.ContainsKey(nameof(LogEntity.Id)))
                {
                    var id = Guid.NewGuid().ToString("N");
                    context.Request.Headers.Add(nameof(LogEntity.Id), id);
                }

                if (!context.Request.Headers.ContainsKey(nameof(HttpContextInfoModel.RequestTime)))
                {
                    var requestTime = DateTimeOffset.Now.ToString(Puppy.Core.Constants.StandardFormat.DateTimeOffSetFormat);
                    context.Request.Headers.Add(nameof(HttpContextInfoModel.RequestTime), requestTime);
                }

                // Allows using several time the stream in ASP.Net Core. Enable Rewind for Request to
                // get Request Body when logging
                context.Request.EnableRewind();

                if (!context.Request.IsRequestFor(LoggerConfig.ViewLogUrl))
                {
                    await _next.Invoke(context).ConfigureAwait(true);
                    return;
                }

                if (!IsCanAccessLogViaUrl(context))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.Headers.Clear();
                    await context.Response.WriteAsync(LoggerConfig.UnAuthorizeMessage).ConfigureAwait(true);
                    return;
                }

                var logsContentResult = Log.GetLogsContentResult(context);
                context.Response.ContentType = logsContentResult.ContentType;
                context.Response.StatusCode = logsContentResult.StatusCode ?? StatusCodes.Status200OK;
                await context.Response.WriteAsync(logsContentResult.Content, Encoding.UTF8).ConfigureAwait(true);
            }
        }

        internal static bool IsCanAccessLogViaUrl(HttpContext httpContext)
        {
            if (string.IsNullOrWhiteSpace(LoggerConfig.AccessKeyQueryParam))
            {
                return true;
            }

            string requestKey = httpContext.Request.Query[LoggerConfig.AccessKeyQueryParam];
            var isCanAccess = string.IsNullOrWhiteSpace(LoggerConfig.AccessKey) || LoggerConfig.AccessKey == requestKey;
            return isCanAccess;
        }

        /// <summary>
        ///     Update LoggerConfig by <see cref="configuration" />
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="configSection"></param>
        internal static void BuildLoggerConfig(this IConfiguration configuration, string configSection = Constant.DefaultConfigSection)
        {
            var isHaveConfig = configuration.GetChildren().Any(x => x.Key == configSection);

            if (isHaveConfig)
            {
                // Rolling File
                LoggerConfig.IsEnableRollingFileLog = configuration.GetValue($"{configSection}:{nameof(LoggerConfig.IsEnableRollingFileLog)}", LoggerConfig.IsEnableRollingFileLog);

                // Path Format
                LoggerConfig.PathFormat = configuration.GetValue($"{configSection}:{nameof(LoggerConfig.PathFormat)}", LoggerConfig.PathFormat);

                // Date Time Format
                LoggerConfig.DateFormat = configuration.GetValue($"{configSection}:{nameof(LoggerConfig.DateFormat)}", LoggerConfig.DateFormat);
                LoggerConfig.HourFormat = configuration.GetValue($"{configSection}:{nameof(LoggerConfig.HourFormat)}", LoggerConfig.HourFormat);
                LoggerConfig.HalfHourFormat = configuration.GetValue($"{configSection}:{nameof(LoggerConfig.HalfHourFormat)}", LoggerConfig.HalfHourFormat);

                LoggerConfig.RetainedFileCountLimit = configuration.GetValue($"{configSection}:{nameof(LoggerConfig.RetainedFileCountLimit)}", LoggerConfig.RetainedFileCountLimit);
                LoggerConfig.FileSizeLimitBytes = configuration.GetValue($"{configSection}:{nameof(LoggerConfig.FileSizeLimitBytes)}", LoggerConfig.FileSizeLimitBytes);
                LoggerConfig.FileLogMinimumLevel = configuration.GetValue($"{configSection}:{nameof(LoggerConfig.FileLogMinimumLevel)}", LoggerConfig.FileLogMinimumLevel);

                // Console
                LoggerConfig.ConsoleLogMinimumLevel = configuration.GetValue($"{configSection}:{nameof(LoggerConfig.ConsoleLogMinimumLevel)}", LoggerConfig.ConsoleLogMinimumLevel);

                // Database
                LoggerConfig.SQLiteFilePath = configuration.GetValue($"{configSection}:{nameof(LoggerConfig.SQLiteFilePath)}", LoggerConfig.SQLiteFilePath);
                LoggerConfig.SQLiteLogMinimumLevel = configuration.GetValue($"{configSection}:{nameof(LoggerConfig.SQLiteLogMinimumLevel)}", LoggerConfig.SQLiteLogMinimumLevel);

                LoggerConfig.ViewLogUrl = configuration.GetValue($"{configSection}:{nameof(LoggerConfig.ViewLogUrl)}", LoggerConfig.ViewLogUrl);
                LoggerConfig.AccessKey = configuration.GetValue($"{configSection}:{nameof(LoggerConfig.AccessKey)}", LoggerConfig.AccessKey);
                LoggerConfig.AccessKeyQueryParam = configuration.GetValue($"{configSection}:{nameof(LoggerConfig.AccessKeyQueryParam)}", LoggerConfig.AccessKeyQueryParam);
                LoggerConfig.UnAuthorizeMessage = configuration.GetValue($"{configSection}:{nameof(LoggerConfig.UnAuthorizeMessage)}", LoggerConfig.UnAuthorizeMessage);

                if (!LoggerConfig.ViewLogUrl.StartsWith("/"))
                {
                    throw new ArgumentException($"{nameof(LoggerConfig.ViewLogUrl)} must start by /", nameof(LoggerConfig.ViewLogUrl));
                }
            }
        }
    }
}