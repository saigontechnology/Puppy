#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore </Project>
//     <File>
//         <Name> DateTimeHelper.cs </Name>
//         <Created> 24 Apr 17 10:49:25 PM </Created>
//         <Key> 94595166-5bf2-462e-9d98-e1180e08ed6c </Key>
//     </File>
//     <Summary>
//         DateTimeHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;

namespace TopCore.Framework.Core.DateTimeUtils
{
    public static class DateTimeHelper
    {
        public static DateTime GetVietNamFromUtc(DateTime dateTimeUtc)
        {
            var vietNamZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return GetDateTimeFromUtc(dateTimeUtc, vietNamZone);
        }

        public static DateTime GetDateTimeFromUtc(DateTime dateTimeUtc, TimeZoneInfo timeZoneInfo)
        {
            var dateTimeWithTimeZone = TimeZoneInfo.ConvertTime(dateTimeUtc, timeZoneInfo);
            return dateTimeWithTimeZone;
        }

        /// <summary>
        ///     Truncate date time
        /// </summary>
        /// <param name="dt">        </param>
        /// <param name="truncateTo"></param>
        /// <returns></returns>
        public static DateTime TruncateTo(DateTime dt, Enums.DateTruncate truncateTo)
        {
            if (truncateTo == Enums.DateTruncate.Year)
                return new DateTime(dt.Year, 0, 0);

            if (truncateTo == Enums.DateTruncate.Month)
                return new DateTime(dt.Year, dt.Month, 0);

            if (truncateTo == Enums.DateTruncate.Day)
                return new DateTime(dt.Year, dt.Month, dt.Day);

            if (truncateTo == Enums.DateTruncate.Hour)
                return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);

            if (truncateTo == Enums.DateTruncate.Minute)
                return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);

            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0);
        }

        /// <summary>
        ///     Get Date Time without milliseconds
        /// </summary>
        /// <returns></returns>
        public static DateTime WithoutMillisecond(DateTime dateTime)
        {
            return TruncateTo(dateTime, Enums.DateTruncate.Second);
        }
    }

    public class Enums
    {
        public enum DateTruncate
        {
            Year,
            Month,
            Day,
            Hour,
            Minute,
            Second
        }
    }
}