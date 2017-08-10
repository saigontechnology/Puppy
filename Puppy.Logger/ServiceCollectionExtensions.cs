#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Puppy.Core.EnvironmentUtils;
using Puppy.Logger.Core;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Puppy.Logger
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLogger(this IServiceCollection services, IConfiguration configuration, string configSection = Constant.DefaultConfigSection)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            configuration.BuildLoggerConfig();
            return services;
        }

        /// <summary>
        ///     Use Puppy Logger, remember to <see cref="AddLogger" /> before <c> UseLogger </c> 
        /// </summary>
        /// <param name="app">          </param>
        /// <param name="loggerFactory"></param>
        /// <param name="appLifetime">   Ensure any buffered events are sent at shutdown </param>
        /// <returns></returns>
        /// <remarks>
        ///     The file will be written using the UTF-8 encoding without a byte-order mark.
        /// </remarks>
        public static IApplicationBuilder UseLogger(this IApplicationBuilder app, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            // Build the config for Logger
            Log.BuildLogger();

            // Add Logger for microsoft logger factory
            loggerFactory.AddSerilog();

            // Ensure any buffered events are sent at shutdown
            appLifetime.ApplicationStopped.Register(Serilog.Log.CloseAndFlush);

            return app;
        }

        public class LoggerMiddleware
        {
            private readonly RequestDelegate _next;

            public LoggerMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public Task Invoke(HttpContext context)
            {
                // Server
                if (context.Request.Headers.ContainsKey("Id"))
                {
                    context.Request.Headers.Remove("Id");
                }

                var id = Guid.NewGuid().ToString("N");
                context.Request.Headers.Add(nameof(LoggerException.Id), id);

                return _next.Invoke(context);
            }
        }

        /// <summary>
        ///     Update LoggerConfig by <see cref="configuration" /> 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="configSection"></param>
        public static void BuildLoggerConfig(this IConfiguration configuration, string configSection = Constant.DefaultConfigSection)
        {
            var isHaveConfig = configuration.GetChildren().Any(x => x.Key == configSection);
            if (isHaveConfig)
            {
                LoggerConfig.PathFormat = configuration.GetValue<string>($"{configSection}:{nameof(LoggerConfig.PathFormat)}");
                LoggerConfig.RetainedFileCountLimit = configuration.GetValue<int>($"{configSection}:{nameof(LoggerConfig.RetainedFileCountLimit)}");
                LoggerConfig.FileSizeLimitBytes = configuration.GetValue<int>($"{configSection}:{nameof(LoggerConfig.FileSizeLimitBytes)}");
            }

            if (!EnvironmentHelper.IsDevelopment()) return;

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Logger Rolling File Path: {LoggerConfig.PathFormat}, Max File Size: {LoggerConfig.FileSizeLimitBytes} (bytes), Max File Count: {LoggerConfig.RetainedFileCountLimit}");
            Console.ResetColor();
        }
    }
}