#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> RollingFileLoggerConfigurationExtensions.cs </Name>
//         <Created> 17/08/17 10:33:17 PM </Created>
//         <Key> c8423f7a-6d49-48d6-b054-b796b150464f </Key>
//     </File>
//     <Summary>
//         RollingFileLoggerConfigurationExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using Serilog.Sinks.File;
using System;

namespace Puppy.Logger.RollingFile
{
    /// <summary>
    ///     Extends <see cref="LoggerSinkConfiguration" /> with rolling file configuration methods. 
    /// </summary>
    public static class RollingFileLoggerConfigurationExtensions
    {
        private const int DefaultRetainedFileCountLimit = 31; // A long month of logs
        private const long DefaultFileSizeLimitBytes = 1L * 1024 * 1024 * 1024;
        private const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";

        /// <summary>
        ///     Write log events to a series of files. Each file will be named according to the date
        ///     of the first log entry written to it. Only simple date-based rolling is currently supported.
        /// </summary>
        /// <param name="sinkConfiguration">        Logger sink configuration. </param>
        /// <param name="pathFormat">              
        ///     String describing the location of the log files, with {Date} in the place of the file
        ///     date. E.g. "Logs\myapp-{Date}.log" will result in log files such as
        ///     "Logs\myapp-2013-10-20.log", "Logs\myapp-2013-10-21.log" and so on.
        /// </param>
        /// <param name="restrictedToMinimumLevel">
        ///     The minimum level for events passed through the sink. Ignored when
        ///     <paramref name="levelSwitch" /> is specified.
        /// </param>
        /// <param name="levelSwitch">             
        ///     A switch allowing the pass-through minimum level to be changed at runtime.
        /// </param>
        /// <param name="outputTemplate">          
        ///     A message template describing the format used to write to the sink. the default is
        ///     "{Timestamp} [{Level}] {Message}{NewLine}{Exception}".
        /// </param>
        /// <param name="formatProvider">          
        ///     Supplies culture-specific formatting information, or null.
        /// </param>
        /// <param name="fileSizeLimitBytes">      
        ///     The maximum size, in bytes, to which any single log file will be allowed to grow. For
        ///     unrestricted growth, pass null. The default is 1 GB.
        /// </param>
        /// <param name="retainedFileCountLimit">  
        ///     The maximum number of log files that will be retained, including the current log
        ///     file. For unlimited retention, pass null. The default is 31.
        /// </param>
        /// <param name="buffered">                
        ///     Indicates if flushing to the output file can be buffered or not. The default is false.
        /// </param>
        /// <param name="shared">                  
        ///     Allow the log files to be shared by multiple processes. The default is false.
        /// </param>
        /// <param name="flushToDiskInterval">     
        ///     If provided, a full disk flush will be performed periodically at the specified interval.
        /// </param>
        /// <returns> Configuration object allowing method chaining. </returns>
        /// <remarks>
        ///     The file will be written using the UTF-8 encoding without a byte-order mark.
        /// </remarks>
        public static LoggerConfiguration RollingFile(
            this LoggerSinkConfiguration sinkConfiguration,
            string pathFormat,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            string outputTemplate = DefaultOutputTemplate,
            IFormatProvider formatProvider = null,
            long? fileSizeLimitBytes = DefaultFileSizeLimitBytes,
            int? retainedFileCountLimit = DefaultRetainedFileCountLimit,
            LoggingLevelSwitch levelSwitch = null,
            bool buffered = false,
            bool shared = false,
            TimeSpan? flushToDiskInterval = null)
        {
            var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            return RollingFile(sinkConfiguration, formatter, pathFormat, restrictedToMinimumLevel, fileSizeLimitBytes,
                retainedFileCountLimit, levelSwitch, flushToDiskInterval);
        }

        /// <summary>
        ///     Write log events to a series of files. Each file will be named according to the date
        ///     of the first log entry written to it. Only simple date-based rolling is currently supported.
        /// </summary>
        /// <param name="sinkConfiguration">        Logger sink configuration. </param>
        /// <param name="formatter">               
        ///     Formatter to control how events are rendered into the file. To control plain text
        ///     formatting, use the overload that accepts an output template instead.
        /// </param>
        /// <param name="pathFormat">              
        ///     String describing the location of the log files, with {Date} in the place of the file
        ///     date. E.g. "Logs\myapp-{Date}.log" will result in log files such as
        ///     "Logs\myapp-2013-10-20.log", "Logs\myapp-2013-10-21.log" and so on.
        /// </param>
        /// <param name="restrictedToMinimumLevel">
        ///     The minimum level for events passed through the sink. Ignored when
        ///     <paramref name="levelSwitch" /> is specified.
        /// </param>
        /// <param name="levelSwitch">             
        ///     A switch allowing the pass-through minimum level to be changed at runtime.
        /// </param>
        /// <param name="fileSizeLimitBytes">      
        ///     The maximum size, in bytes, to which any single log file will be allowed to grow. For
        ///     unrestricted growth, pass null. The default is 1 GB.
        /// </param>
        /// <param name="retainedFileCountLimit">  
        ///     The maximum number of log files that will be retained, including the current log
        ///     file. For unlimited retention, pass null. The default is 31.
        /// </param>
        /// <param name="flushToDiskInterval">     
        ///     If provided, a full disk flush will be performed periodically at the specified interval.
        /// </param>
        /// <returns> Configuration object allowing method chaining. </returns>
        /// <remarks>
        ///     The file will be written using the UTF-8 encoding without a byte-order mark.
        /// </remarks>
        public static LoggerConfiguration RollingFile(
            this LoggerSinkConfiguration sinkConfiguration,
            ITextFormatter formatter,
            string pathFormat,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            long? fileSizeLimitBytes = DefaultFileSizeLimitBytes,
            int? retainedFileCountLimit = DefaultRetainedFileCountLimit,
            LoggingLevelSwitch levelSwitch = null,
            TimeSpan? flushToDiskInterval = null)
        {
            if (sinkConfiguration == null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (formatter == null) throw new ArgumentNullException(nameof(formatter));

            ILogEventSink sink = new RollingFileSink(pathFormat, formatter, fileSizeLimitBytes, retainedFileCountLimit);

            if (flushToDiskInterval.HasValue)
            {
                sink = new PeriodicFlushToDiskSink(sink, flushToDiskInterval.Value);
            }

            return sinkConfiguration.Sink(sink, restrictedToMinimumLevel, levelSwitch);
        }
    }
}