#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore </Project>
//     <File>
//         <Name> DateTimeExtensions.cs </Name>
//         <Created> 28 Apr 17 2:52:29 PM </Created>
//         <Key> 143563ad-5bc1-4db4-a54d-549a760fffa1 </Key>
//     </File>
//     <Summary>
//         DateTimeExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;

namespace Puppy.Core.DateTimeUtils
{
    public static class DateTimeExtensions
    {
        public static DateTime GetVietNamFromUtc(this DateTime dateTimeUtc)
        {
            var vietNamZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            return GetDateTimeFromUtc(dateTimeUtc, vietNamZone);
        }

        public static DateTime GetDateTimeFromUtc(this DateTime dateTimeUtc, TimeZoneInfo timeZoneInfo)
        {
            var dateTimeWithTimeZone = TimeZoneInfo.ConvertTimeFromUtc(dateTimeUtc, timeZoneInfo);
            return dateTimeWithTimeZone;
        }

        /// <summary>
        ///     See more: https://msdn.microsoft.com/en-us/library/gg154758.aspx 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="timeZone"> Time Zone ID, See more: https://msdn.microsoft.com/en-us/library/gg154758.aspx </param>
        /// <returns></returns>
        public static DateTime WithTimeZone(this DateTime dateTime, string timeZone)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);

            var dateTimeWithTimeZone = new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, timeZoneInfo.BaseUtcOffset);

            return dateTimeWithTimeZone.DateTime;
        }

        /// <summary>
        ///     See more: https://msdn.microsoft.com/en-us/library/gg154758.aspx 
        /// </summary>
        /// <param name="dateTimeOffset"></param>
        /// <param name="timeZone">       Time Zone ID, See more: https://msdn.microsoft.com/en-us/library/gg154758.aspx </param>
        /// <returns></returns>
        public static DateTimeOffset WithTimeZone(this DateTimeOffset dateTimeOffset, string timeZone)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);

            var dateTimeWithTimeZone = new DateTimeOffset(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second, dateTimeOffset.Millisecond, timeZoneInfo.BaseUtcOffset);

            var offset = dateTimeWithTimeZone.Offset;

            return dateTimeWithTimeZone;
        }

        /// <summary>
        ///     Truncate date time 
        /// </summary>
        /// <param name="dt">        </param>
        /// <param name="truncateTo"></param>
        /// <returns></returns>
        public static DateTime TruncateTo(this DateTime dt, Enums.DateTruncate truncateTo)
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

        /// <summary>
        ///     Return number of seconds from value - 1/1/1970 00:00:00 with UTC date time kind 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetEpochTime(this DateTime value)
        {
            return value.Subtract(DateTimeHelper.EpochTime).TotalSeconds;
        }

        /// <summary>
        ///     Return number of seconds from value - 1/1/1970 00:00:00 with UTC date time kind 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double GetEpochTime(this DateTimeOffset value)
        {
            return value.Subtract(DateTimeHelper.EpochTime).TotalSeconds;
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