#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> RollingFileSink.cs </Name>
//         <Created> 18/08/17 12:26:49 AM </Created>
//         <Key> 3e7b5d6a-e453-4e79-a14d-9f05a251c6b9 </Key>
//     </File>
//     <Summary>
//         RollingFileSink.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Sinks.File;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Puppy.Logger.RollingFile
{
    /// <summary>
    ///     Write log events to a series of files. Each file will be named according to the date of
    ///     the first log entry written to it. Only simple date-based rolling is currently supported.
    /// </summary>
    public sealed class RollingFileSink : ILogEventSink, IFlushableFileSink, IDisposable
    {
        private readonly bool _buffered;
        private readonly Encoding _encoding;
        private readonly long? _fileSizeLimitBytes;
        private readonly int? _retainedFileCountLimit;
        private readonly TemplatePathRoller _roller;
        private readonly bool _shared;
        private readonly object _syncRoot = new object();
        private readonly ITextFormatter _textFormatter;
        private ILogEventSink _currentFile;

        private bool _isDisposed;
        private DateTime? _nextCheckpoint;

        /// <summary>
        ///     Construct a <see cref="RollingFileSink" />. 
        /// </summary>
        /// <param name="pathFormat">            
        ///     String describing the location of the log files, with {Date} in the place of the file
        ///     date. E.g. "Logs\myapp-{Date}.log" will result in log files such as
        ///     "Logs\myapp-2013-10-20.log", "Logs\myapp-2013-10-21.log" and so on.
        /// </param>
        /// <param name="textFormatter">         
        ///     Formatter used to convert log events to text.
        /// </param>
        /// <param name="fileSizeLimitBytes">    
        ///     The maximum size, in bytes, to which a log file will be allowed to grow. For
        ///     unrestricted growth, pass null. The default is 1 GB.
        /// </param>
        /// <param name="retainedFileCountLimit">
        ///     The maximum number of log files that will be retained, including the current log
        ///     file. For unlimited retention, pass null. The default is 31.
        /// </param>
        /// <param name="encoding">              
        ///     Character encoding used to write the text file. The default is UTF-8 without BOM.
        /// </param>
        /// <param name="buffered">              
        ///     Indicates if flushing to the output file can be buffered or not. The default is false.
        /// </param>
        /// <param name="shared">                
        ///     Allow the log files to be shared by multiple processes. The default is false.
        /// </param>
        /// <returns> Configuration object allowing method chaining. </returns>
        /// <remarks> The file will be written using the UTF-8 character set. </remarks>
        public RollingFileSink(string pathFormat,
            ITextFormatter textFormatter,
            long? fileSizeLimitBytes,
            int? retainedFileCountLimit,
            Encoding encoding = null,
            bool buffered = false,
            bool shared = false)
        {
            if (pathFormat == null) throw new ArgumentNullException(nameof(pathFormat));
            if (fileSizeLimitBytes.HasValue && fileSizeLimitBytes < 0)
                throw new ArgumentException("Negative value provided; file size limit must be non-negative");
            if (retainedFileCountLimit.HasValue && retainedFileCountLimit < 1)
                throw new ArgumentException(
                    "Zero or negative value provided; retained file count limit must be at least 1");

            _roller = new TemplatePathRoller(pathFormat);
            _textFormatter = textFormatter;
            _fileSizeLimitBytes = fileSizeLimitBytes;
            _retainedFileCountLimit = retainedFileCountLimit;
            _encoding = encoding;
            _buffered = buffered;
            _shared = shared;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     un-managed resources.
        /// </summary>
        public void Dispose()
        {
            lock (_syncRoot)
            {
                if (_currentFile == null) return;
                CloseFile();
                _isDisposed = true;
            }
        }

        /// <inheritdoc />
        public void FlushToDisk()
        {
            lock (_syncRoot)
            {
                (_currentFile as IFlushableFileSink)?.FlushToDisk();
            }
        }

        /// <summary>
        ///     Emit the provided log event to the sink. 
        /// </summary>
        /// <param name="logEvent"> The log event to write. </param>
        /// <remarks>
        ///     Events that come in out-of-order (e.g. around the rollovers) may end up written to a
        ///     later file than their timestamp would indicate.
        /// </remarks>
        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));

            lock (_syncRoot)
            {
                if (_isDisposed) throw new ObjectDisposedException("The rolling log file has been disposed.");

                AlignCurrentFileTo(logEvent);

                // If the file was unable to be opened on the last attempt, it will remain null until
                // the next checkpoint passes, at which time another attempt will be made to open it.
                _currentFile?.Emit(logEvent);
            }
        }

        private void AlignCurrentFileTo(LogEvent logEvent)
        {
            UpdateLogFileDirectory(logEvent);

            if (_nextCheckpoint.HasValue && logEvent.Timestamp >= _nextCheckpoint.Value)
            {
                CloseFile();
            }

            OpenFile(logEvent);
        }

        /// <summary>
        ///     Update _roller directory by {Level} 
        /// </summary>
        /// <param name="logEvent"></param>
        private void UpdateLogFileDirectory(LogEvent logEvent)
        {
            _roller.LogFileDirectory = _roller.LogFileDirectory.Replace("{Level}", logEvent.Level.ToString());
            _roller.LogFileDirectory = _roller.LogFileDirectory.Replace(LogEventLevel.Verbose.ToString(), logEvent.Level.ToString());
            _roller.LogFileDirectory = _roller.LogFileDirectory.Replace(LogEventLevel.Debug.ToString(), logEvent.Level.ToString());
            _roller.LogFileDirectory = _roller.LogFileDirectory.Replace(LogEventLevel.Information.ToString(), logEvent.Level.ToString());
            _roller.LogFileDirectory = _roller.LogFileDirectory.Replace(LogEventLevel.Warning.ToString(), logEvent.Level.ToString());
            _roller.LogFileDirectory = _roller.LogFileDirectory.Replace(LogEventLevel.Error.ToString(), logEvent.Level.ToString());
            _roller.LogFileDirectory = _roller.LogFileDirectory.Replace(LogEventLevel.Fatal.ToString(), logEvent.Level.ToString());
        }

        private void OpenFile(LogEvent logEvent)
        {
            var currentCheckpoint = _roller.GetCurrentCheckpoint(logEvent.Timestamp);

            // We only take one attempt at it because repeated failures to open log files REALLY slow
            // an app down.
            _nextCheckpoint = _roller.GetNextCheckpoint(logEvent.Timestamp);

            var existingFiles = Enumerable.Empty<string>();
            try
            {
                existingFiles = Directory.GetFiles(_roller.LogFileDirectory, _roller.DirectorySearchPattern).Select(Path.GetFileName);
            }
            catch (DirectoryNotFoundException)
            {
            }

            var latestForThisCheckpoint = _roller
                .SelectMatches(existingFiles)
                .Where(m => m.DateTime == currentCheckpoint)
                .OrderByDescending(m => m.SequenceNumber)
                .FirstOrDefault();

            var sequence = latestForThisCheckpoint?.SequenceNumber ?? 0;

            const int maxAttempts = 3;
            for (var attempt = 0; attempt < maxAttempts; attempt++)
            {
                string path;
                _roller.GetLogFilePath(logEvent, sequence, out path);

                try
                {
                    _currentFile = _shared
                        ? (ILogEventSink)new SharedFileSink(path, _textFormatter, _fileSizeLimitBytes, _encoding)
                        : new FileSink(path, _textFormatter, _fileSizeLimitBytes, _encoding, _buffered);
                }
                catch (IOException ex)
                {
                    if (IoErrors.IsLockedFile(ex))
                    {
                        SelfLog.WriteLine("Rolling file target {0} was locked, attempting to open next in sequence (attempt {1})", path, attempt + 1);
                        sequence++;
                        continue;
                    }

                    throw;
                }

                ApplyRetentionPolicy(path);
                return;
            }
        }

        private void ApplyRetentionPolicy(string currentFilePath)
        {
            if (_retainedFileCountLimit == null) return;

            var currentFileName = Path.GetFileName(currentFilePath);

            // We consider the current file to exist, even if nothing's been written yet, because
            // files are only opened on response to an event being processed.
            var potentialMatches = Directory.GetFiles(_roller.LogFileDirectory, _roller.DirectorySearchPattern)
                .Select(Path.GetFileName)
                .Union(new[] { currentFileName });

            var newestFirst = _roller
                .SelectMatches(potentialMatches)
                .OrderByDescending(m => m.DateTime)
                .ThenByDescending(m => m.SequenceNumber)
                .Select(m => m.FileName);

            var toRemove = newestFirst
                .Where(n => StringComparer.OrdinalIgnoreCase.Compare(currentFileName, n) != 0)
                .Skip(_retainedFileCountLimit.Value - 1)
                .ToList();

            foreach (var obsolete in toRemove)
            {
                var fullPath = Path.Combine(_roller.LogFileDirectory, obsolete);
                try
                {
                    File.Delete(fullPath);
                }
                catch (Exception ex)
                {
                    SelfLog.WriteLine("Error {0} while removing obsolete file {1}", ex, fullPath);
                }
            }
        }

        private void CloseFile()
        {
            if (_currentFile != null)
            {
                (_currentFile as IDisposable)?.Dispose();
                _currentFile = null;
            }

            _nextCheckpoint = null;
        }
    }
}