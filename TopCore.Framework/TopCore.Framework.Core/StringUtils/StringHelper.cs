#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore </Project>
//     <File>
//         <Name> StringHelper.cs </Name>
//         <Created> 24 Apr 17 10:44:55 PM </Created>
//         <Key> dbf0d9b6-5968-47f0-b083-8375bb027161 </Key>
//     </File>
//     <Summary>
//         StringHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TopCore.Framework.Core.StringUtils
{
    public static class StringHelper
    {
        public const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public const string NumberChars = "0123456789";
        public static Random Random = new Random();

        /// <summary>
        ///     Generate Random UPPER String 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRandomUpperString(int length)
        {
            return GetRandomString(length).ToUpperInvariant();
        }

        /// <summary>
        ///     Generate Random String 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRandomString(int length)
        {
            var stringChars = new char[length];

            for (var i = 0; i < stringChars.Length; i++)
                stringChars[i] = Chars[Random.Next(Chars.Length)];

            return new string(stringChars);
        }

        /// <summary>
        ///     Generate Random String 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRandomNumber(int length)
        {
            var stringChars = new char[length];

            for (var i = 0; i < stringChars.Length; i++)
                stringChars[i] = NumberChars[Random.Next(NumberChars.Length)];

            return new string(stringChars);
        }

        /// <summary>
        ///     Remove all diacritics (accents) in string 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveAccents(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            var normalizedString = value.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        ///     Normalize: UPPER CASE with remove all diacritic (accents) in string 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        ///     Replace đ, Đ to D.if value is is Null Or WhiteSpace will return string.Empty
        /// </remarks>
        public static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            value = value.Trim();

            // Special word in vietnamese
            value = value.Replace("đ", "d").Replace("Đ", "D");

            var normalizedString = RemoveAccents(value);
            return normalizedString.ToUpperInvariant();
        }

        public static string ReplaceNullOrWhiteSpaceToEmpty(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;
            return value;
        }

        /// <summary>
        ///     Remove all tag html 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string RemoveHtmlTag(string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }

        public static bool IsGuid(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            Guid guid;
            var isValid = Guid.TryParse(value, out guid);
            return isValid;
        }

        public static bool IsValidEmail(string value)
        {
            RegexUtilities regexUtilities = new RegexUtilities();
            return regexUtilities.IsValidEmail(value);
        }

        public static bool IsValidPhoneNumber(string value, string countryCode = "+84", int minLength = 9, int maxLength = 11)
        {
            var regex = value.StartsWith("0")
                ? $@"^(\d[0-9]{{{minLength},{maxLength}}}$"
                : $@"^(\+{countryCode}[0-9]{{{minLength},{maxLength}}}$";

            return Regex.Match(value, regex).Success;
        }
    }

    internal class RegexUtilities
    {
        private bool _invalid;

        public bool IsValidEmail(string strIn)
        {
            _invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper,
                    RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            if (_invalid)
                return false;

            // Return true if strIn is in valid e-mail format.
            try
            {
                return Regex.IsMatch(strIn,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                _invalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
    }
}