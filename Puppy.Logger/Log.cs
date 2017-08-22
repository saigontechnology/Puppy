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
using Puppy.Logger.SQLite;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.IO;

namespace Puppy.Logger
{
    public partial class Log
    {
        internal static void BuildLogger()
        {
            var loggerConfig =
                new LoggerConfiguration()
                    .WriteTo
                    .SQLite(LoggerConfig.SQLiteConnectionString, LoggerConfig.SQLiteLogMinimumLevelEnum);

            // Enable rolling file log by config
            if (LoggerConfig.IsEnableRollingFileLog)
            {
                loggerConfig
                    .WriteTo.RollingFile(
                    pathFormat: LoggerConfig.PathFormat,
                    fileSizeLimitBytes: LoggerConfig.FileSizeLimitBytes,
                    retainedFileCountLimit: LoggerConfig.RetainedFileCountLimit,
                    restrictedToMinimumLevel: LoggerConfig.FileLogMinimumLevelEnum,
                    formatter: new LoggerTextFormatter(),
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

        private static void UpdateLogInfo(ExceptionContext context, LogInfo logInfo, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            UpdateLogInfo(logInfo, callerMemberName, callerFilePath, callerLineNumber);

            // Priority to use Header Id instead of self generate Id
            if (context.HttpContext.Request.Headers.ContainsKey(nameof(logInfo.Id)))
                logInfo.Id = context.HttpContext.Request.Headers[nameof(logInfo.Id)];

            // Get Request Time from Header
            if (context.HttpContext.Request.Headers.ContainsKey(nameof(HttpContextInfo.RequestTime)))
            {
                string requestTimeStr = context.HttpContext.Request.Headers[nameof(HttpContextInfo.RequestTime)];
                DateTime requestTime;
                var isCanGetRequestTime =
                    DateTimeHelper.TryParse(requestTimeStr, Core.Constant.DateTimeOffSetFormat, out requestTime);
                logInfo.HttpContextInfo.RequestTime =
                    isCanGetRequestTime ? (DateTimeOffset?)requestTime : null;
            }
        }

        private static void UpdateLogInfo(LogInfo logInfo, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            logInfo.CallerMemberName = callerMemberName;
            logInfo.CallerFilePath = callerFilePath;
            logInfo.CallerRelativePath = GetCallerRelativePath(logInfo.CallerFilePath);
            logInfo.CallerLineNumber = callerLineNumber;
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