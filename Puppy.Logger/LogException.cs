#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> LogException.cs </Name>
//         <Created> 11/08/17 11:55:38 PM </Created>
//         <Key> 5965a811-ef13-47ed-8308-78d11b0e41f1 </Key>
//     </File>
//     <Summary>
//         LogException.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Hangfire;
using Microsoft.AspNetCore.Mvc.Filters;
using Puppy.Logger.Core;
using Puppy.Logger.Core.Models;
using System;
using System.Runtime.CompilerServices;

namespace Puppy.Logger
{
    public partial class Log
    {
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
            var id = JobLogException(LogLevel.Error, ex, callerMemberName, callerFilePath, callerLineNumber);
            return id;
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
            var id = JobLogExceptionContext(LogLevel.Error, context, callerMemberName, callerFilePath, callerLineNumber);
            return id;
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
            var id = JobLogException(LogLevel.Fatal, ex, callerMemberName, callerFilePath, callerLineNumber);
            return id;
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
            var id = JobLogExceptionContext(LogLevel.Fatal, context, callerMemberName, callerFilePath, callerLineNumber);
            return id;
        }

        private static string JobLogException(LogLevel logLevel, Exception ex, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            var loggerException = new LoggerException(ex, logLevel);
            UpdateLoggerException(loggerException, callerMemberName, callerFilePath, callerLineNumber);
            BackgroundJob.Enqueue(() => Write(logLevel, loggerException.ToString()));
            return loggerException.Id;
        }

        private static string JobLogExceptionContext(LogLevel logLevel, ExceptionContext context, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            var loggerException = new LoggerException(context, logLevel);
            UpdateLoggerException(context, loggerException, callerMemberName, callerFilePath, callerLineNumber);
            BackgroundJob.Enqueue(() => Write(logLevel, loggerException.ToString()));
            return loggerException.Id;
        }
    }
}