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
        public const string LogTypeException = "Exception";

        /// <summary>
        ///     Write Log with Error Level and Return Global ID as Guid String of Log Entry 
        /// </summary>
        /// <param name="ex">              </param>
        /// <param name="type">            </param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath">  </param>
        /// <param name="callerLineNumber"></param>
        /// <returns> Log Id </returns>
        public static string Error(Exception ex, string type = LogTypeException, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            var id = JobLogException(ex, LogLevel.Error, type, callerMemberName, callerFilePath, callerLineNumber);
            return id;
        }

        /// <summary>
        ///     Write Log with Error Level and Return Global ID as Guid String of Log Entry 
        /// </summary>
        /// <param name="context">         </param>
        /// <param name="type">            </param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath">  </param>
        /// <param name="callerLineNumber"></param>
        /// <returns> Log Id </returns>
        /// <remarks> Priority to use Header Id instead of self generate Id </remarks>
        public static string Error(ExceptionContext context, string type = LogTypeException, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            var id = JobLogException(context, LogLevel.Error, type, callerMemberName, callerFilePath, callerLineNumber);
            return id;
        }

        /// <summary>
        ///     Write Log with Fatal Level and Return Global ID as Guid String of Log Entry 
        /// </summary>
        /// <param name="ex">              </param>
        /// <param name="type">            </param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath">  </param>
        /// <param name="callerLineNumber"></param>
        /// <returns> Log Id </returns>
        public static string Fatal(Exception ex, string type = LogTypeException, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            var id = JobLogException(ex, LogLevel.Fatal, type, callerMemberName, callerFilePath, callerLineNumber);
            return id;
        }

        /// <summary>
        ///     Write Log with Fatal Level and Return Global ID as Guid String of Log Entry 
        /// </summary>
        /// <param name="context">         </param>
        /// <param name="type">            </param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath">  </param>
        /// <param name="callerLineNumber"></param>
        /// <returns> Log Id </returns>
        /// <remarks> Priority to use Header Id instead of self generate Id </remarks>
        public static string Fatal(ExceptionContext context, string type = LogTypeException, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            var id = JobLogException(context, LogLevel.Fatal, type, callerMemberName, callerFilePath, callerLineNumber);
            return id;
        }

        private static string JobLogException(Exception ex, LogLevel logLevel, string type, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            var logEntity = new LogEntity(ex, logLevel, type);
            UpdateLogInfo(logEntity, callerMemberName, callerFilePath, callerLineNumber);
            BackgroundJob.Enqueue(() => Write(logLevel, logEntity.ToString()));
            return logEntity.Id;
        }

        private static string JobLogException(ExceptionContext context, LogLevel logLevel, string type, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            var logEntity = new LogEntity(context, logLevel, type);
            UpdateLogInfo(context, logEntity, callerMemberName, callerFilePath, callerLineNumber);
            BackgroundJob.Enqueue(() => Write(logLevel, logEntity.ToString()));
            return logEntity.Id;
        }
    }
}