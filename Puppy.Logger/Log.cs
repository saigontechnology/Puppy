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
using Puppy.Logger.RollingFile;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.IO;
using Puppy.Logger.SQLite;

namespace Puppy.Logger
{
    public partial class Log
    {
        internal static void BuildLogger()
        {
            var fileLogLevelSwitch = new LoggingLevelSwitch
            {
                MinimumLevel = LoggerConfig.FileLogMinimumLevelEnum
            };

            var loggerConfig =
                new LoggerConfiguration()
                    .MinimumLevel
                    .ControlledBy(fileLogLevelSwitch)
                    .WriteTo.SQLite(LoggerConfig.SQLiteConnectionString, LoggerConfig.SQLiteLogTableName,
                        LoggerConfig.SQLiteLogMinimumLevelEnum, storeTimestampInUtc: true);

            // Enable rolling file log by config
            if (LoggerConfig.IsEnableRollingFileLog)
            {
                loggerConfig
                    .WriteTo.RollingFile(
                    pathFormat: LoggerConfig.PathFormat,
                    fileSizeLimitBytes: LoggerConfig.FileSizeLimitBytes,
                    retainedFileCountLimit: LoggerConfig.RetainedFileCountLimit,
                    levelSwitch: fileLogLevelSwitch,
                    formatter: new LoggerTextFormatter(), // Custom Formatter for LoggerException
                    shared: true
                );
            }

            // Only enable console log in Development
            if (EnvironmentHelper.IsDevelopment())
            {
                var consoleLogLevelSwitch = new LoggingLevelSwitch
                {
                    MinimumLevel = LoggerConfig.ConsoleLogMinimumLevelEnum
                };

                loggerConfig =
                    loggerConfig
                        .WriteTo
                        .ColoredConsole(LoggerConfig.ConsoleLogMinimumLevelEnum, Constant.ConsoleTemplate,
                            levelSwitch: consoleLogLevelSwitch);
            }

            // Add Logger to Serilog
            Serilog.Log.Logger = loggerConfig.CreateLogger();
        }

        public static void Write(LogLevel logLevel, string message)
        {
            LogEventLevel logEventLevel = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), logLevel.ToString());
            Serilog.Log.Write(logEventLevel, message);
        }

        private static void UpdateLoggerException(ExceptionContext context, LoggerException loggerException, string callerMemberName, string callerFilePath, int callerLineNumber)
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

        private static void UpdateLoggerException(LoggerException loggerException, string callerMemberName, string callerFilePath, int callerLineNumber)
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