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
using System.Globalization;

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

        /// <summary>
        ///     Get Display string of the double 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="k">     1,000 Display by K char </param>
        /// <param name="m">     1,000,000 Display by M char </param>
        /// <param name="b">     1,000,000,000 Display by B char </param>
        /// <param name="pointChar"> Use . as point character </param>
        /// <returns></returns>
        public static string ToShortDisplay(this double value, string k = "K", string m = "M", string b = "B", string pointChar = ".")
        {

            if (value >= 10000000000)
            {
                return (value / 10000000000D).ToString("0.##", CultureInfo.InvariantCulture).Replace(".", pointChar) + $" {b}";
            }

            if (value >= 1000000000)
            {
                return (value / 1000000000D).ToString("0.#", CultureInfo.InvariantCulture).Replace(".", pointChar) + $" {b}";
            }

            if (value >= 100000000)
            {
                return (value / 10000000D).ToString("0.##", CultureInfo.InvariantCulture).Replace(".", pointChar) + $" {m}";
            }

            if (value >= 1000000)
            {
                return (value / 1000000D).ToString("0.#", CultureInfo.InvariantCulture).Replace(".", pointChar) + $" {m}";
            }

            if (value >= 100000)
            {
                return (value / 1000D).ToString("0.##", CultureInfo.InvariantCulture).Replace(".", pointChar) + $" {k}";
            }

            if (value >= 1000)
            {
                return (value / 1000D).ToString("0.#", CultureInfo.InvariantCulture).Replace(".", pointChar) + $" {k}";
            }

            return value.ToString("#,0");
        }
    }
}