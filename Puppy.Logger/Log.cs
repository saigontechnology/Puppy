#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> Log.cs </Name>
//         <Created> 10/08/17 11:08:05 AM </Created>
//         <Key> c7f5efdf-645e-4b7d-8c03-580f83d02994 </Key>
//     </File>
//     <Summary>
//         Log.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Mvc.Filters;
using Puppy.Core.EnvironmentUtils;
using Puppy.Logger.Core;
using Serilog;
using Serilog.Core;
using System;

namespace Puppy.Logger
{
    public class Log
    {
        public static void Verbose(string message)
        {
            LoggerException loggerException = new LoggerException(message, LogLevel.Verbose);
            Serilog.Log.Verbose(loggerException.ToString());
        }

        public static void Debug(string message)
        {
            LoggerException loggerException = new LoggerException(message, LogLevel.Debug);
            Serilog.Log.Debug(loggerException.ToString());
        }

        public static void Information(string message)
        {
            LoggerException loggerException = new LoggerException(message, LogLevel.Information);
            Serilog.Log.Information(loggerException.ToString());
        }

        public static void Warning(string message)
        {
            LoggerException loggerException = new LoggerException(message, LogLevel.Warning);
            Serilog.Log.Warning(loggerException.ToString());
        }

        public static void Error(string message)
        {
            LoggerException loggerException = new LoggerException(message, LogLevel.Error);
            Serilog.Log.Error(loggerException.ToString());
        }

        /// <summary>
        ///     Write Log with Error Level and Return Global ID as Guid String of Log Entry 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string Error(Exception ex)
        {
            LoggerException loggerException = new LoggerException(ex, LogLevel.Error);
            Serilog.Log.Error(loggerException.ToString());
            return loggerException.Id;
        }

        /// <summary>
        ///     Write Log with Error Level and Return Global ID as Guid String of Log Entry 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <remarks> Priority to use Header Id instead of self generate Id </remarks>
        public static string Error(ExceptionContext context)
        {
            // TODO Log Request Information: Endpoint with Param values

            LoggerException loggerException = new LoggerException(context, LogLevel.Error);

            // Priority to use Header Id instead of self generate Id
            if (context.HttpContext.Request.Headers.ContainsKey(nameof(LoggerException.Id)))
            {
                loggerException.Id = context.HttpContext.Request.Headers[nameof(LoggerException.Id)];
            }

            Serilog.Log.Error(loggerException.ToString());
            return loggerException.Id;
        }

        public static void Fatal(string message)
        {
            LoggerException loggerException = new LoggerException(message, LogLevel.Fatal);
            Serilog.Log.Fatal(loggerException.ToString());
        }

        /// <summary>
        ///     Write Log with Fatal Level and Return Global ID as Guid String of Log Entry 
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string Fatal(Exception ex)
        {
            LoggerException loggerException = new LoggerException(ex, LogLevel.Fatal);
            Serilog.Log.Fatal(loggerException.ToString());
            return loggerException.Id;
        }

        /// <summary>
        ///     Write Log with Fatal Level and Return Global ID as Guid String of Log Entry 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <remarks> Priority to use Header Id instead of self generate Id </remarks>
        public static string Fatal(ExceptionContext context)
        {
            // TODO Log Request Information: Endpoint with Param values

            LoggerException loggerException = new LoggerException(context, LogLevel.Fatal);

            // Priority to use Header Id instead of self generate Id
            if (context.HttpContext.Request.Headers.ContainsKey(nameof(LoggerException.Id)))
            {
                loggerException.Id = context.HttpContext.Request.Headers[nameof(LoggerException.Id)];
            }

            Serilog.Log.Fatal(loggerException.ToString());
            return loggerException.Id;
        }

        internal static void BuildLogger()
        {
            var levelSwitch = new LoggingLevelSwitch
            {
                MinimumLevel = LoggerConfig.FileLogMinimumLevelEnum
            };

            var loggerConfig =
                new LoggerConfiguration()
                    .MinimumLevel.ControlledBy(levelSwitch)
                    .WriteTo.RollingFile(
                        pathFormat: LoggerConfig.PathFormat,
                        fileSizeLimitBytes: LoggerConfig.FileSizeLimitBytes,
                        retainedFileCountLimit: LoggerConfig.RetainedFileCountLimit,
                        levelSwitch: levelSwitch,
                        formatter: new LoggerFormatter() // Custom Formatter for ErrorModel
                    );

            if (EnvironmentHelper.IsDevelopment())
            {
                // Add Write to Console with Colored
                loggerConfig = loggerConfig.WriteTo.ColoredConsole(LoggerConfig.ConsoleLogMinimumLevelEnum, Constant.ConsoleTemplate);
            }

            // Add Logger to Serilog
            Serilog.Log.Logger = loggerConfig.CreateLogger();
        }
    }
}