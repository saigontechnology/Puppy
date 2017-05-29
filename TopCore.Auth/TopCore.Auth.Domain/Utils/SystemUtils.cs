#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore.WebAPI </Project>
//     <File>
//         <Name> SystemUtils.cs </Name>
//         <Created> 29/05/2017 9:44:29 AM </Created>
//         <Key> 9c611bdc-6df4-47bf-883e-6dad21356d5c </Key>
//     </File>
//     <Summary>
//         SystemUtils.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;
using TopCore.Framework.Core.DateTimeUtils;

namespace TopCore.Auth.Domain.Utils
{
    public static class SystemUtils
    {
        public static DateTime GetSystemTimeNow()
        {
            return DateTimeOffset.UtcNow.GetSystemTime();
        }

        public static DateTime GetSystemTime(this DateTimeOffset dateTimeoffset)
        {
            return dateTimeoffset.UtcDateTime.GetDateTimeFromUtc(Constants.System.TimeZoneInfo);
        }

        public static DateTime GetSystemTime(this DateTime dateTimeUtc)
        {
            return dateTimeUtc.GetDateTimeFromUtc(Constants.System.TimeZoneInfo);
        }
    }
}