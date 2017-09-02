#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> GuidHelper.cs </Name>
//         <Created> 24/07/17 2:28:14 AM </Created>
//         <Key> 3d39d9fc-b166-4ae1-924b-1592efee14f9 </Key>
//     </File>
//     <Summary>
//         GuidHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;

namespace Puppy.Core.GuidUtils
{
    public static class GuidHelper
    {
        private static readonly long BaseDateTicks = new DateTime(1900, 1, 1).Ticks;

        private static readonly DateTime Epoc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        ///     <para>
        ///         Generate a new sequential <see cref="Guid" /> using the <c> comb </c> algorithm.
        ///     </para>
        ///     <para>
        ///         The <c> comb </c> algorithm is designed to make the use of GUIDs as Primary Keys,
        ///         Foreign Keys, and Indexes nearly as efficient as ints.
        ///     </para>
        /// </summary>
        /// <returns> A comb Guid. </returns>
        public static Guid Generate()
        {
            var guidArray = Guid.NewGuid().ToByteArray();

            var now = DateTime.UtcNow;

            // Get the days and milliseconds which will be used to build the byte string
            var days = new TimeSpan(now.Ticks - BaseDateTicks);
            var msecs = now.TimeOfDay;

            // Convert to a byte array Note that SQL Server is accurate to 1/300th of a millisecond
            // so we divide by 3.333333
            var daysArray = BitConverter.GetBytes(days.Days);
            var msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));

            // Reverse the bytes to match SQL Servers ordering
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);

            // Copy the bytes into the guid
            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);

            return new Guid(guidArray);
        }

        public static bool IsGuid(string value)
        {
            if (String.IsNullOrWhiteSpace(value)) return false;
            var isValid = Guid.TryParse(value, out _);
            return isValid;
        }
    }
}