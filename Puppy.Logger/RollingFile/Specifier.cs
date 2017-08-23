#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> Specifier.cs </Name>
//         <Created> 17/08/17 10:25:31 PM </Created>
//         <Key> 5a888ed8-6af6-45a7-b6e5-bb7d03d194ea </Key>
//     </File>
//     <Summary>
//         Specifier.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;
using System.Linq;

namespace Puppy.Logger.RollingFile
{
    internal class Specifier
    {
        public static readonly Specifier Date = new Specifier(nameof(Date), LoggerConfig.DateFormat, TimeSpan.FromDays(1));
        public static readonly Specifier Hour = new Specifier(nameof(Hour), LoggerConfig.HourFormat, TimeSpan.FromHours(1));
        public static readonly Specifier HalfHour = new Specifier(nameof(HalfHour), LoggerConfig.HalfHourFormat, TimeSpan.FromMinutes(30));

        public string Name { get; private set; }

        public string Token { get; private set; }

        public string Format { get; private set; }

        public TimeSpan Interval { get; private set; }

        private Specifier(string name, string format, TimeSpan interval)

        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(format)) throw new ArgumentNullException(nameof(format));

            Name = name;
            Token = "{" + name + "}";
            Format = format;
            Interval = interval;
        }

        public DateTime GetCurrentCheckpoint(DateTimeOffset instant)
        {
            if (this == Hour)
                return instant.Date.AddHours(instant.Hour);

            if (this != HalfHour)
            {
                return instant.Date;
            }
            var hour = instant.Date.AddHours(instant.Hour);
            return instant.Minute >= 30 ? hour.AddMinutes(30) : hour;
        }

        public DateTime GetNextCheckpoint(DateTimeOffset instant)
        {
            return GetCurrentCheckpoint(instant).Add(Interval);
        }

        public static bool TryGetSpecifier(string pathTemplate, out Specifier specifier)
        {
            if (string.IsNullOrWhiteSpace(pathTemplate)) throw new ArgumentNullException(nameof(pathTemplate));

            var specifiers = new[] { HalfHour, Hour, Date }.Where(s => pathTemplate.Contains(s.Token)).ToArray();

            if (specifiers.Length > 1)
                throw new ArgumentException("Only one interval specifier can be used in a rolling log file path.", nameof(pathTemplate));

            specifier = specifiers.FirstOrDefault();

            if (specifier == null) return specifier != null;

            // Update format from logger config
            switch (specifier.Name)
            {
                case nameof(Date):
                    {
                        specifier.Format = LoggerConfig.DateFormat;
                        break;
                    }
                case nameof(Hour):
                    {
                        specifier.Format = LoggerConfig.HourFormat;
                        break;
                    }
                case nameof(HalfHour):
                    {
                        specifier.Format = LoggerConfig.HalfHourFormat;
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("Only one interval specifier can be used in a rolling log file path.", nameof(pathTemplate));
                    }
            }

            return specifier != null;
        }
    }
}