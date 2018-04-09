#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2018 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey </Project>
//     <File>
//         <Name> DecimalExtensions.cs </Name>
//         <Created> 07/03/2018 10:12:52 AM </Created>
//         <Key> 9ca69118-53e1-49ad-9a76-1ba9a0c97abc </Key>
//     </File>
//     <Summary>
//         DecimalExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System.Globalization;

namespace Puppy.Core.NumberUtils
{
    public static class DecimalExtensions
    {
        /// <summary>
        ///     Get Display string of the double 
        /// </summary>
        /// <param name="value">    </param>
        /// <param name="k">         1,000 Display by K char </param>
        /// <param name="m">         1,000,000 Display by M char </param>
        /// <param name="b">         1,000,000,000 Display by B char </param>
        /// <param name="pointChar"> Use . as point character </param>
        /// <returns></returns>
        public static string ToShortDisplay(this decimal value, string k = "K", string m = "M", string b = "B", string pointChar = ".")
        {
            if (value >= 10000000000)
            {
                return (value / (decimal)1000000000D).ToString("0.##", CultureInfo.InvariantCulture).Replace(".", pointChar) + $" {b}";
            }

            if (value >= 1000000000)
            {
                return (value / (decimal)1000000000D).ToString("0.#", CultureInfo.InvariantCulture).Replace(".", pointChar) + $" {b}";
            }

            if (value >= 10000000)
            {
                return (value / (decimal)1000000D).ToString("0.##", CultureInfo.InvariantCulture).Replace(".", pointChar) + $" {m}";
            }

            if (value >= 1000000)
            {
                return (value / (decimal)1000000D).ToString("0.#", CultureInfo.InvariantCulture).Replace(".", pointChar) + $" {m}";
            }

            if (value >= 10000)
            {
                return (value / (decimal)1000D).ToString("0.##", CultureInfo.InvariantCulture).Replace(".", pointChar) + $" {k}";
            }

            if (value >= 1000)
            {
                return (value / (decimal)1000D).ToString("0.#", CultureInfo.InvariantCulture).Replace(".", pointChar) + $" {k}";
            }

            return value.ToString("N");
        }
    }
}