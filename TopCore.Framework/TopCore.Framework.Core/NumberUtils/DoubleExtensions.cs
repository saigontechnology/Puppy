#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore.WebAPI </Project>
//     <File>
//         <Name> DoubleExtensions.cs </Name>
//         <Created> 07 May 17 2:42:28 AM </Created>
//         <Key> 4d396da9-5310-4949-a8ec-dd053c674c0a </Key>
//     </File>
//     <Summary>
//         DoubleExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;

namespace TopCore.Framework.Core.NumberUtils
{
    public static class DoubleExtensions
    {
        public static double Ceiling(this double value, double significance)
        {
            if (Math.Abs(value % significance) > 0)
                return (int) (value / significance) * significance + significance;

            return Convert.ToDouble(value);
        }

        public static double Floor(this double value, double significance)
        {
            if (Math.Abs(value % significance) > 0)
                return (int) (value / significance) * significance;

            return Convert.ToDouble(value);
        }
    }
}