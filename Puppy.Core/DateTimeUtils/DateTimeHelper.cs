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
    }
}