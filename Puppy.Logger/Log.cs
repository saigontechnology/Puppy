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
using Serilog;
using Serilog.Core;
using System;

namespace Puppy.Logger
{
    public class Log
    {
        public static void Verbose(string message)
        {
            Serilog.Log.Verbose(message);
        }

        public static void Debug(string message)
        {
            Serilog.Log.Debug(message);
        }

        public static void Information(string message)
        {
            Serilog.Log.Information(message);
        }

        public static void Warning(string message)
        {
            Serilog.Log.Warning(message);
        }

        public static void Error(string message)
        {
            Serilog.Log.Error(message);
        }

        /// <summary>
        ///     Write Log with Error Level and Return Global ID as Guid of Log Entry 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Guid Error(Exception e)
        {
            LoggerModel loggerModel = new LoggerModel(e);
            Serilog.Log.Error(loggerModel.ToString());
            return loggerModel.Id;
        }

        /// <summary>
        ///     Write Log with Error Level and Return Global ID as Guid of Log Entry 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Guid Error(ExceptionContext e)
        {
            LoggerModel loggerModel = new LoggerModel(e.Exception);
            Serilog.Log.Error(loggerModel.ToString());
            return loggerModel.Id;
        }

        public static void Fatal(string message)
        {
            Serilog.Log.Fatal(message);
        }

        /// <summary>
        ///     Write Log with Fatal Level and Return Global ID as Guid of Log Entry 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Guid Fatal(Exception e)
        {
            LoggerModel loggerModel = new LoggerModel(e);
            Serilog.Log.Fatal(loggerModel.ToString());
            return loggerModel.Id;
        }

        /// <summary>
        ///     Write Log with Fatal Level and Return Global ID as Guid of Log Entry 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Guid Fatal(ExceptionContext e)
        {
            LoggerModel loggerModel = new LoggerModel(e.Exception);
            Serilog.Log.Fatal(loggerModel.ToString());
            return loggerModel.Id;
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
                    .Enrich.WithProcessId()
                    .Enrich.WithProcessName()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentUserName()
                    .Enrich.WithThreadId()
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