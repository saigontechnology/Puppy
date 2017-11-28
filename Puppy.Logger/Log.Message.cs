#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> LogMessage.cs </Name>
//         <Created> 11/08/17 11:54:22 PM </Created>
//         <Key> 9d29953a-3393-4626-b6d1-a0e88b181636 </Key>
//     </File>
//     <Summary>
//         LogMessage.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Hangfire;
using Puppy.Logger.Core;
using Puppy.Logger.Core.Models;
using System.Runtime.CompilerServices;

namespace Puppy.Logger
{
    public partial class Log
    {
        public const string LogTypeMessage = "Message";

        /// <summary>
        ///     Write Log with Verbose Level and Return Global ID as Guid String of Log Entry 
        /// </summary>
        /// <param name="message">         </param>
        /// <param name="type">            </param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath">  </param>
        /// <param name="callerLineNumber"></param>
        /// <returns> Log Id </returns>
        public static string Verbose(string message, string type = LogTypeMessage, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            return JobLogMessage(message, LogLevel.Verbose, type, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        ///     Write Log with Debug Level and Return Global ID as Guid String of Log Entry 
        /// </summary>
        /// <param name="message">         </param>
        /// <param name="type">            </param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath">  </param>
        /// <param name="callerLineNumber"></param>
        /// <returns> Log Id </returns>
        public static string Debug(string message, string type = LogTypeMessage, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            return JobLogMessage(message, LogLevel.Debug, type, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        ///     Write Log with Information Level and Return Global ID as Guid String of Log Entry 
        /// </summary>
        /// <param name="message">         </param>
        /// <param name="type">            </param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath">  </param>
        /// <param name="callerLineNumber"></param>
        /// <returns> Log Id </returns>
        public static string Information(string message, string type = LogTypeMessage, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            return JobLogMessage(message, LogLevel.Information, type, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        ///     Write Log with Warning Level and Return Global ID as Guid String of Log Entry 
        /// </summary>
        /// <param name="message">         </param>
        /// <param name="type">            </param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath">  </param>
        /// <param name="callerLineNumber"></param>
        /// <returns> Log Id </returns>
        public static string Warning(string message, string type = LogTypeMessage, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            return JobLogMessage(message, LogLevel.Warning, type, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        ///     Write Log with Error Level and Return Global ID as Guid String of Log Entry 
        /// </summary>
        /// <param name="message">         </param>
        /// <param name="type">            </param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath">  </param>
        /// <param name="callerLineNumber"></param>
        /// <returns> Log Id </returns>
        public static string Error(string message, string type = LogTypeMessage, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            return JobLogMessage(message, LogLevel.Error, type, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        ///     Write Log with Fatal Level and Return Global ID as Guid String of Log Entry 
        /// </summary>
        /// <param name="message">         </param>
        /// <param name="type">            </param>
        /// <param name="callerMemberName"></param>
        /// <param name="callerFilePath">  </param>
        /// <param name="callerLineNumber"></param>
        /// <returns> Log Id </returns>
        public static string Fatal(string message, string type = LogTypeMessage, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            return JobLogMessage(message, LogLevel.Fatal, type, callerMemberName, callerFilePath, callerLineNumber);
        }

        private static string JobLogMessage(string message, LogLevel level, string type, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            var logEntity = new LogEntity(message, level, type);
            UpdateLogInfo(logEntity, callerMemberName, callerFilePath, callerLineNumber);
            BackgroundJob.Enqueue(() => Write(level, logEntity.ToString()));
            return logEntity.Id;
        }
    }
}