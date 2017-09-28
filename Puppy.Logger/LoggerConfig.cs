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
using Puppy.Core.StringUtils;
using Serilog.Events;
using System;
using System.IO;
using System.Linq;

namespace Puppy.Logger
{
    /// <summary>
    ///     [Auto Reload] Logger Configuration 
    /// </summary>
    /// <remarks>
    ///     <c> Not allow config format/template </c> of rolling file because it must use Puppy
    ///     Logger format for <c> analyze purpose </c>.
    /// </remarks>
    public static class LoggerConfig
    {
        #region Rolling File

        /// <summary>
        ///     Default Puppy Logger always log in SQLite file and also in Rolling File with config,
        ///     so you can enable or disable rolling file option
        /// </summary>
        public static bool IsEnableRollingFileLog { get; set; } = true;

        /// <summary>
        ///     Path format to write log, default is <c> "Logs\\{Level}\\LOG_{Level}_{Date}.json" </c>
        /// </summary>
        public static string PathFormat { get; set; } = "Logs\\{Level}\\LOG_{Level}_{Date}.json";

        public static string DateFormat { get; set; } = "yyyy-MM-dd";

        public static string HourFormat { get; set; } = "yyyy-MM-dd HH";

        public static string HalfHourFormat { get; set; } = "yyyy-MM-dd HH_mm";

        public static string FullPath => PathFormat.GetFullPath();

        public static string FolderFullPath => Path.GetDirectoryName(PathFormat).GetFullPath();

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
        public static string FileLogMinimumLevel
        {
            get => FileLogMinimumLevelEnum.ToString();
            set
            {
                if (!Constant.LogEventLevels.Contains(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(FileLogMinimumLevel));
                }
                FileLogMinimumLevelEnum = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), value);
            }
        }

        #endregion

        #region Console

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
                if (!Constant.LogEventLevels.Contains(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(ConsoleLogMinimumLevel));
                }
                ConsoleLogMinimumLevelEnum = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), value);
            }
        }

        #endregion

        #region SQLite

        public static string SQLiteConnectionString { get; set; } = @"Logs\Puppy.Logger.db";

        [JsonIgnore]
        internal static LogEventLevel SQLiteLogMinimumLevelEnum = LogEventLevel.Warning;

        /// <summary>
        ///     <para> Minimum level to log by SQLite. </para>
        ///     <para>
        ///         Have 5 levels: <c> Verbose </c>, <c> Debug </c>, <c> Information </c>, <c>
        ///         Warning </c>, <c> Error </c>, <c> Fatal </c>
        ///     </para>
        /// </summary>
        /// <remarks> Default is <c> Information </c> </remarks>
        public static string SQLiteLogMinimumLevel
        {
            get => SQLiteLogMinimumLevelEnum.ToString();
            set
            {
                if (!Constant.LogEventLevels.Contains(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(SQLiteLogMinimumLevelEnum));
                }
                SQLiteLogMinimumLevelEnum = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), value);
            }
        }

        /// <summary>
        ///     <para> Log API endpoint, start by "/". Default is "/developers/logs" </para>
        ///     <para>
        ///         Use query string "skip", "take", "terms" (have 's', multiple search for 'Id',
        ///         'Message', 'Level' and 'DateTime' - format "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK")
        ///     </para>
        /// </summary>
        public static string ViewLogUrl { get; set; } = "/developers/logs";

        /// <summary>
        ///     Access Key read from URI, default is empty 
        /// </summary>
        /// <remarks> Empty is allow <c> anonymous </c> </remarks>
        public static string AccessKey { get; set; } = string.Empty;

        /// <summary>
        ///     Query parameter via http request, default is "key" 
        /// </summary>
        /// <remarks> Empty is allow <c> anonymous </c> </remarks>
        public static string AccessKeyQueryParam { get; set; } = "key";

        /// <summary>
        ///     Un-authorize message when user access api document with not correct key. Default is
        ///     "You don't have permission to view Logs, please contact your administrator."
        /// </summary>
        public static string UnAuthorizeMessage { get; set; } = "You don't have permission to view Logs, please contact your administrator.";

        #endregion
    }
}