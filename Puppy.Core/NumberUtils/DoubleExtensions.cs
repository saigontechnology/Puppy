#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
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

namespace Puppy.Core.NumberUtils
{
    public static class DoubleExtensions
    {
        public static double CeilingSignificance(this double value, double significance)
        {
            if (Math.Abs(value % significance) > 0)
            {
                value = (int)(value / significance);
                value *= significance;
                value += significance;
            }

            return value;
        }

        public static double CeilingPlaces(this double value, int places)
        {
            double multiplier = Math.Pow(10, Convert.ToDouble(places));

            value = Math.Ceiling(value * multiplier) / multiplier;

            return value;
        }

        public static double FloorSignificance(this double value, double significance)
        {
            if (Math.Abs(value % significance) > 0)
            {
                value = (int)(value / significance);
                value *= significance;
            }

            return value;
        }
    }
}