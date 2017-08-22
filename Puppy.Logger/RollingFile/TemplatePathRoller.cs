#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> TemplatePathRoller.cs </Name>
//         <Created> 17/08/17 10:29:09 PM </Created>
//         <Key> 509a863d-5b0b-44af-80d5-115f3560c9dc </Key>
//     </File>
//     <Summary>
//         TemplatePathRoller.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Puppy.Logger.RollingFile
{
    /// <summary>
    ///     Rolls files based on the current date, using a path formatting pattern like: Logs/log-{Date}.txt 
    /// </summary>
    internal class TemplatePathRoller
    {
        private const string DefaultSeparator = "-";

        private const string SpecifierMatchGroup = "specifier";

        private const string SequenceNumberMatchGroup = "sequence";

        private readonly string _pathTemplate;

        private readonly Regex _filenameMatcher;

        private readonly Specifier _specifier;

        public string LogFileDirectory { get; set; }

        public string DirectorySearchPattern { get; }

        public TemplatePathRoller(string pathTemplate)
        {
            if (pathTemplate == null) throw new ArgumentNullException(nameof(pathTemplate));

            var directory = Path.GetDirectoryName(pathTemplate);

            if (string.IsNullOrEmpty(directory))
            {
                directory = Directory.GetCurrentDirectory();
            }

            Specifier directorySpecifier;
            if (Specifier.TryGetSpecifier(directory, out directorySpecifier))
            {
                throw new ArgumentException($"The {directorySpecifier.Token} specifier cannot form part of the directory name.");
            }

            directory = Path.GetFullPath(directory);

            var filenameTemplate = pathTemplate.Substring(pathTemplate.LastIndexOf("\\", StringComparison.Ordinal) + 1);

            if (!Specifier.TryGetSpecifier(filenameTemplate, out _specifier))
            {
                _specifier = Specifier.Date;
                filenameTemplate = Path.GetFileNameWithoutExtension(filenameTemplate) + DefaultSeparator + _specifier.Token + Path.GetExtension(filenameTemplate);
            }

            var indexOfSpecifier = filenameTemplate.IndexOf(_specifier.Token, StringComparison.Ordinal);
            var prefix = filenameTemplate.Substring(0, indexOfSpecifier);
            var suffix = filenameTemplate.Substring(indexOfSpecifier + _specifier.Token.Length);
            _filenameMatcher = new Regex(
                                        "^" +
                                        Regex.Escape(prefix) +
                                        "(?<" + SpecifierMatchGroup + ">\\d{" + _specifier.Format.Length + "})" +
                                        "(?<" + SequenceNumberMatchGroup + ">_[0-9]{3,}){0,1}" +
                                        Regex.Escape(suffix) +
                                        "$");

            DirectorySearchPattern = filenameTemplate.Replace(_specifier.Token, "*");
            LogFileDirectory = directory;
            _pathTemplate = Path.Combine(LogFileDirectory, filenameTemplate);
        }

        public void GetLogFilePath(LogEvent logEvent, int sequenceNumber, out string path)
        {
            var currentCheckpoint = GetCurrentCheckpoint(logEvent.Timestamp);

            var tok = currentCheckpoint.ToString(_specifier.Format, CultureInfo.InvariantCulture);

            if (sequenceNumber != 0)
            {
                tok += "_" + sequenceNumber.ToString("000", CultureInfo.InvariantCulture);
            }

            // Replace by Token
            path = _pathTemplate.Replace(_specifier.Token, tok);

            // Replace by Level
            path = path.Replace("{Level}", logEvent.Level.ToString());
        }

        public IEnumerable<RollingLogFile> SelectMatches(IEnumerable<string> fileNames)
        {
            foreach (var filename in fileNames)
            {
                var match = _filenameMatcher.Match(filename);
                if (!match.Success) continue;
                var inc = 0;
                var incGroup = match.Groups[SequenceNumberMatchGroup];
                if (incGroup.Captures.Count != 0)
                {
                    var incPart = incGroup.Captures[0].Value.Substring(1);
                    inc = int.Parse(incPart, CultureInfo.InvariantCulture);
                }

                DateTime dateTime;
                var dateTimePart = match.Groups[SpecifierMatchGroup].Captures[0].Value;
                if (!DateTime.TryParseExact(dateTimePart, _specifier.Format, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out dateTime))
                {
                    continue;
                }

                yield return new RollingLogFile(filename, dateTime, inc);
            }
        }

        public DateTime GetCurrentCheckpoint(DateTimeOffset instant) => _specifier.GetCurrentCheckpoint(instant);

        public DateTime GetNextCheckpoint(DateTimeOffset instant) => _specifier.GetNextCheckpoint(instant);
    }
}