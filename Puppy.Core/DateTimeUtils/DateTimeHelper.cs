#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> DateTimeHelper.cs </Name>
//         <Created> 17/07/17 10:53:05 PM </Created>
//         <Key> 6d40165e-1334-4210-9199-da7dc7f3896e </Key>
//     </File>
//     <Summary>
//         DateTimeHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;
using System.Globalization;
using TimeZoneConverter;

namespace Puppy.Core.DateTimeUtils
{
    public static class DateTimeHelper
    {
        public static readonly DateTime EpochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime Parse(string value, string format)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            return DateTime.ParseExact(value, format, CultureInfo.InvariantCulture);
        }

        public static bool TryParse(string value, string format, out DateTime dateTime)
        {
            return DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
        }

        public static DateTime? ReplaceNullOrDefault(DateTime? value, DateTime? replace)
        {
            value = value ?? replace;
            return value;
        }

        public static DateTime ReplaceNullOrDefault(DateTime value, DateTime replace)
        {
            value = value == default ? replace : value;
            return value;
        }

        public static DateTimeOffset? ReplaceNullOrDefault(DateTimeOffset? value, DateTimeOffset? replace)
        {
            value = value ?? replace;
            return value;
        }

        public static DateTimeOffset ReplaceNullOrDefault(DateTimeOffset value, DateTimeOffset replace)
        {
            value = value == default ? replace : value;
            return value;
        }

        /// <summary>
        ///     Return a Date Time from Epoch Time plus value as total seconds 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTimeOffset GetDateTimeFromEpoch(double value)
        {
            return EpochTime.AddSeconds(value);
        }

        /// <summary>
        ///     Support find time zone id by difference platform: Windows, Mac, Linux. 
        /// </summary>
        /// <param name="timeZoneId"></param>
        /// <returns></returns>
        public static TimeZoneInfo GetTimeZoneInfo(string timeZoneId)
        {
            var timeZoneInfo = TZConvert.GetTimeZoneInfo(timeZoneId);

            return timeZoneInfo;
        }

        /// <summary>
        ///     Support find time zone id by difference platform: Windows, Mac, Linux. 
        /// </summary>
        /// <param name="timeZoneId">  </param>
        /// <param name="timeZoneInfo"></param>
        /// <returns></returns>
        public static bool TryGetTimeZoneInfo(string timeZoneId, out TimeZoneInfo timeZoneInfo)
        {
            return TZConvert.TryGetTimeZoneInfo(timeZoneId, out timeZoneInfo);
        }
    }
}