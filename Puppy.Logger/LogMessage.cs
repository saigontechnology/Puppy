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
        public static void Verbose(string message, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            JobLogMessage(LogLevel.Verbose, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Debug(string message, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            JobLogMessage(LogLevel.Debug, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Information(string message, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            JobLogMessage(LogLevel.Information, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Warning(string message, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            JobLogMessage(LogLevel.Warning, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Error(string message, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            JobLogMessage(LogLevel.Error, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        public static void Fatal(string message, [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = -1)
        {
            JobLogMessage(LogLevel.Fatal, message, callerMemberName, callerFilePath, callerLineNumber);
        }

        private static void JobLogMessage(LogLevel logLevel, string message, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            var logInfo = new LogInfo(message, logLevel);
            UpdateLogInfo(logInfo, callerMemberName, callerFilePath, callerLineNumber);
            BackgroundJob.Enqueue(() => Write(logLevel, logInfo.ToString()));
        }
    }
}