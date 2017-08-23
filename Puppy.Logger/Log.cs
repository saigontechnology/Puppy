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

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Puppy.Core.DateTimeUtils;
using Puppy.Core.EnvironmentUtils;
using Puppy.Logger.Core;
using Puppy.Logger.Core.Models;
using Puppy.Logger.RollingFile;
using Puppy.Logger.SQLite;
using Puppy.Web;
using Puppy.Web.Models.Api;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;

namespace Puppy.Logger
{
    public partial class Log
    {
        internal static void BuildLogger()
        {
            var loggerConfiguration =
                new LoggerConfiguration().WriteTo.SQLite();

            // Enable rolling file log by config
            if (LoggerConfig.IsEnableRollingFileLog)
            {
                loggerConfiguration
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

                loggerConfiguration =
                    loggerConfiguration
                        .WriteTo
                        .ColoredConsole(LoggerConfig.ConsoleLogMinimumLevelEnum, Constant.ConsoleTemplate,
                            levelSwitch: consoleLogLevelSwitch);
            }

            // Add Logger to Serilog
            Serilog.Log.Logger = loggerConfiguration.CreateLogger();
        }

        public static void Write(LogLevel logLevel, string message)
        {
            LogEventLevel logEventLevel = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), logLevel.ToString());
            Serilog.Log.Write(logEventLevel, message);
        }

        private static void UpdateLogInfo(ActionContext context, LogEntity logEntity, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            UpdateLogInfo(logEntity, callerMemberName, callerFilePath, callerLineNumber);

            // Priority to use Header Id instead of self generate Id
            if (context.HttpContext.Request.Headers.ContainsKey(nameof(logEntity.Id)))
                logEntity.Id = context.HttpContext.Request.Headers[nameof(logEntity.Id)];

            // Get Request Time from Header
            if (context.HttpContext.Request.Headers.ContainsKey(nameof(HttpContextInfo.RequestTime)))
            {
                string requestTimeStr = context.HttpContext.Request.Headers[nameof(HttpContextInfo.RequestTime)];
                DateTime requestTime;
                var isCanGetRequestTime =
                    DateTimeHelper.TryParse(requestTimeStr, Core.Constant.DateTimeOffSetFormat, out requestTime);
                logEntity.HttpContext.RequestTime =
                    isCanGetRequestTime ? (DateTimeOffset?)requestTime : null;
            }
        }

        private static void UpdateLogInfo(LogEntity logEntity, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            logEntity.CallerMemberName = callerMemberName;
            logEntity.CallerFilePath = callerFilePath;
            logEntity.CallerRelativePath = GetCallerRelativePath(logEntity.CallerFilePath);
            logEntity.CallerLineNumber = callerLineNumber;
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