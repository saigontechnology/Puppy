#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> LoggerConfig.cs </Name>
//         <Created> 10/08/17 12:31:28 PM </Created>
//         <Key> 645e0f00-7753-42d7-8f8d-8e2029b3e5ca </Key>
//     </File>
//     <Summary>
//         LoggerConfig.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;
using Puppy.Logger.Core;
using Serilog.Events;
using System;
using System.IO;
using System.Linq;

namespace Puppy.Logger.Models
{
    /// <summary>
    ///     Logger Configuration 
    /// </summary>
    /// <remarks>
    ///     <c> Not allow config format/template </c> of rolling file because it must use Puppy
    ///     Logger format for <c> analyze purpose </c>.
    /// </remarks>
    public static class LoggerConfig
    {
        /// <summary>
        ///     Path format to write log, default is <c> Logs\\LOG_{Date}.json </c> 
        /// </summary>
        public static string PathFormat { get; set; } = "Logs\\LOG_{Date}.json";

        public static string FullPath { get; } = Path.Combine(Directory.GetCurrentDirectory(), PathFormat);

        public static string FolderFullPath { get; } = Path.Combine(Directory.GetCurrentDirectory(), Path.GetDirectoryName(PathFormat));

        /// <summary>
        ///     Maximum retained file, default is <c> 365 </c> 
        /// </summary>
        public static int RetainedFileCountLimit { get; set; } = 365;

        /// <summary>
        ///     Maximum file size, default is <c> 1048576 (bytes) ~ 1 GB </c> 
        /// </summary>
        public static long? FileSizeLimitBytes { get; set; } = 1048576;

        [JsonIgnore]
        internal static LogEventLevel FileLogMinimumLevelEnum = LogEventLevel.Warning;

        /// <summary>
        ///     <para> Minimum level to log by file. </para>
        ///     <para>
        ///         Have 5 levels: <c> Verbose </c>, <c> Debug </c>, <c> Information </c>, <c>
        ///         Warning </c>, <c> Error </c>, <c> Fatal </c>
        ///     </para>
        /// </summary>
        /// <remarks> Default is <c> Warning </c> </remarks>
        public static LogLevel FileLogMinimumLevel
        {
            get => (LogLevel)Enum.Parse(typeof(LogLevel), FileLogMinimumLevelEnum.ToString());
            set
            {
                if (!Enum.IsDefined(typeof(LogLevel), value))
                {
                    throw new ArgumentOutOfRangeException(nameof(FileLogMinimumLevel));
                }
                FileLogMinimumLevelEnum = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), value.ToString());
            }
        }

        [JsonIgnore]
        internal static LogEventLevel ConsoleLogMinimumLevelEnum = LogEventLevel.Information;

        /// <summary>
        ///     <para> Minimum level to log by console. </para>
        ///     <para>
        ///         Have 5 levels: <c> Verbose </c>, <c> Debug </c>, <c> Information </c>, <c>
        ///         Warning </c>, <c> Error </c>, <c> Fatal </c>
        ///     </para>
        /// </summary>
        /// <remarks> Default is <c> Information </c> </remarks>
        public static string ConsoleLogMinimumLevel
        {
            get => ConsoleLogMinimumLevelEnum.ToString();
            set
            {
                if (!Constant.LogLevels.Contains(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(ConsoleLogMinimumLevel));
                }
                ConsoleLogMinimumLevelEnum = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), value);
            }
        }
    }
}