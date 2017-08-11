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
using Puppy.Core.DateTimeUtils;
using Puppy.Core.EnvironmentUtils;
using Puppy.Logger.Core;
using Puppy.Logger.Core.Models;
using Serilog;
using Serilog.Core;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Puppy.Logger
{
    public class Log
    {
        public static void Verbose(string message, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            var loggerException = new LoggerException(message, LogLevel.Verbose);
            UpdateLoggerException(loggerException, callerMemberName, callerFilePath, callerLineNumber);
            Serilog.Log.Verbose(loggerException.ToString());
        }

        public static void Debug(string message, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            var loggerException = new LoggerException(message, LogLevel.Debug);
            UpdateLoggerException(loggerException, callerMemberName, callerFilePath, callerLineNumber);
            Serilog.Log.Debug(loggerException.ToString());
        }

        public static void Information(string message, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            var loggerException = new LoggerException(message, LogLevel.Information);
            UpdateLoggerException(loggerException, callerMemberName, callerFilePath, callerLineNumber);
            Serilog.Log.Information(loggerException.ToString());
        }

        public static void Warning(string message, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            var loggerException = new LoggerException(message, LogLevel.Warning);
            UpdateLoggerException(loggerException, callerMemberName, callerFilePath, callerLineNumber);
            Serilog.Log.Warning(loggerException.ToString());
        }

        public static void Error(string message, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            var loggerException = new LoggerException(message, LogLevel.Error);
            UpdateLoggerException(loggerException, callerMemberName, callerFilePath, callerLineNumber);
            Serilog.Log.Error(loggerException.ToString());
        }

        /// <summary>
        ///     Write Log with Error Level and Return Global ID as Guid String of Log Entry 
        /// </summary>
        /// <param name="ex">              </param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath">  </param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        public static string Error(Exception ex, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            var loggerException = new LoggerException(ex, LogLevel.Error);
            UpdateLoggerException(loggerException, callerMemberName, callerFilePath, callerLineNumber);
            Serilog.Log.Error(loggerException.ToString());
            return loggerException.Id;
        }

        /// <summary>
        ///     Write Log with Error Level and Return Global ID as Guid String of Log Entry 
        /// </summary>
        /// <param name="context">         </param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath">  </param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        /// <remarks> Priority to use Header Id instead of self generate Id </remarks>
        public static string Error(ExceptionContext context, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            var loggerException = new LoggerException(context, LogLevel.Error);

            UpdateLoggerException(context, loggerException, callerMemberName, callerFilePath, callerLineNumber);

            Serilog.Log.Error(loggerException.ToString());
            return loggerException.Id;
        }

        public static void Fatal(string message, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            var loggerException = new LoggerException(message, LogLevel.Fatal);

            UpdateLoggerException(loggerException, callerMemberName, callerFilePath, callerLineNumber);

            Serilog.Log.Fatal(loggerException.ToString());
        }

        /// <summary>
        ///     Write Log with Fatal Level and Return Global ID as Guid String of Log Entry 
        /// </summary>
        /// <param name="ex">              </param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath">  </param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        public static string Fatal(Exception ex, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            var loggerException = new LoggerException(ex, LogLevel.Fatal);

            UpdateLoggerException(loggerException, callerMemberName, callerFilePath, callerLineNumber);

            Serilog.Log.Fatal(loggerException.ToString());
            return loggerException.Id;
        }

        /// <summary>
        ///     Write Log with Fatal Level and Return Global ID as Guid String of Log Entry 
        /// </summary>
        /// <param name="context">         </param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath">  </param>
        /// <param name="callerLineNumber"></param>
        /// <returns></returns>
        /// <remarks> Priority to use Header Id instead of self generate Id </remarks>
        public static string Fatal(ExceptionContext context, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            var loggerException = new LoggerException(context, LogLevel.Fatal);

            UpdateLoggerException(context, loggerException, callerMemberName, callerFilePath, callerLineNumber);

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
                        formatter: new LoggerFormatter() // Custom Formatter for LoggerException
                    );

            if (EnvironmentHelper.IsDevelopment())
                loggerConfig =
                    loggerConfig.WriteTo.ColoredConsole(LoggerConfig.ConsoleLogMinimumLevelEnum,
                        Constant.ConsoleTemplate);

            // Add Logger to Serilog
            Serilog.Log.Logger = loggerConfig.CreateLogger();
        }

        private static void UpdateLoggerException(ExceptionContext context, LoggerException loggerException,
            string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            UpdateLoggerException(loggerException, callerMemberName, callerFilePath, callerLineNumber);

            // Priority to use Header Id instead of self generate Id
            if (context.HttpContext.Request.Headers.ContainsKey(nameof(LoggerException.Id)))
                loggerException.Id = context.HttpContext.Request.Headers[nameof(LoggerException.Id)];

            // Get Request Time from Header
            if (context.HttpContext.Request.Headers.ContainsKey(nameof(HttpContextInfo.RequestTime)))
            {
                string requestTimeStr = context.HttpContext.Request.Headers[nameof(HttpContextInfo.RequestTime)];
                DateTime requestTime;
                var isCanGetRequestTime =
                    DateTimeHelper.TryParse(requestTimeStr, Core.Constant.DateTimeOffSetFormat, out requestTime);
                loggerException.HttpContextInfo.RequestTime =
                    isCanGetRequestTime ? (DateTimeOffset?)requestTime : null;
            }
        }

        private static void UpdateLoggerException(LoggerException loggerException, string callerMemberName,
            string callerFilePath, int callerLineNumber)
        {
            loggerException.CallerMemberName = callerMemberName;
            loggerException.CallerFilePath = callerFilePath;
            loggerException.CallerRelativePath = GetCallerRelativePath(loggerException.CallerFilePath);
            loggerException.CallerLineNumber = callerLineNumber;
        }

        private static string GetCallerRelativePath(string callerFilePath)
        {
            if (string.IsNullOrWhiteSpace(callerFilePath))
                throw new ArgumentNullException(nameof(callerFilePath));

            var workingDirectory = Directory.GetCurrentDirectory();
            DirectoryInfo workingDirectoryInfo = new DirectoryInfo(workingDirectory);
            var callerRelativePath = callerFilePath.Replace(workingDirectoryInfo.FullName, string.Empty).Trim('\\');
            callerRelativePath = Path.Combine(workingDirectoryInfo.Name, callerRelativePath);
            return callerRelativePath;
        }
    }
}