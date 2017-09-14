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

namespace Puppy.Core.DateTimeUtils
{
    public static class DateTimeHelper
    {
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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
            value = (value == default(DateTime) || value == null) ? replace : value;
            return value;
        }

        public static DateTime ReplaceNullOrDefault(DateTime value, DateTime replace)
        {
            value = value == default(DateTime) ? replace : value;
            return value;
        }

        public static DateTimeOffset? ReplaceNullOrDefault(DateTimeOffset? value, DateTimeOffset? replace)
        {
            value = (value == default(DateTimeOffset) || value == null) ? replace : value;
            return value;
        }

        public static DateTimeOffset ReplaceNullOrDefault(DateTimeOffset value, DateTimeOffset replace)
        {
            value = value == default(DateTimeOffset) ? replace : value;
            return value;
        }

        /// <summary>
        ///     Return number of seconds from value - 1/1/1970 00:00:00 with UTC date time kind 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetEpochTime(DateTime value)
        {
            return value.Subtract(Epoch).TotalSeconds;
        }

        /// <summary>
        ///     Return number of seconds from value - 1/1/1970 00:00:00 with UTC date time kind 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetEpochTime(DateTimeOffset value)
        {
            return value.Subtract(Epoch).TotalSeconds;
        }
    }
}