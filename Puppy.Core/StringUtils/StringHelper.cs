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
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Puppy.Core.StringUtils
{
    public static class StringHelper
    {
        public const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public const string LowerChars = "abcdefghijklmnopqrstuvwxyz";
        public const string UpperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string NumberChars = "0123456789";
        public const string SpecialChars = "!@#$%^*&";

        /// <summary>
        ///     Get Random String 
        /// </summary>
        /// <param name="length"> </param>
        /// <param name="alpha">  </param>
        /// <param name="upper">  </param>
        /// <param name="lower">  </param>
        /// <param name="numeric"></param>
        /// <param name="special"></param>
        /// <returns></returns>
        public static string GetRandomString(int length, bool alpha = true, bool upper = true, bool lower = true,
            bool numeric = true, bool special = false)
        {
            var characters = string.Empty;
            if (alpha)
            {
                if (upper)
                {
                    characters += UpperChars;
                }

                if (lower)
                {
                    characters += LowerChars;
                }
            }

            if (numeric)
            {
                characters += NumberChars;
            }

            if (special)
            {
                characters += SpecialChars;
            }

            return GetRandomString(length, characters);
        }

        /// <summary>
        ///     Get Random String 
        /// </summary>
        /// <param name="length">    </param>
        /// <param name="characters"></param>
        /// <returns></returns>
        public static string GetRandomString(int length, string characters)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "length cannot be less than zero.");
            }

            if (string.IsNullOrWhiteSpace(characters))
            {
                throw new ArgumentOutOfRangeException(nameof(characters), "characters invalid.");
            }

            const int byteSize = 0x100;
            if (byteSize < characters.Length)
            {
                throw new ArgumentException(
                    string.Format("{0} may contain no more than {1} characters.", nameof(characters), byteSize),
                    nameof(characters));
            }

            var outOfRangeStart = byteSize - (byteSize % characters.Length);
            using (var rng = RandomNumberGenerator.Create())
            {
                var sb = new StringBuilder();
                var buffer = new byte[128];
                while (sb.Length < length)
                {
                    rng.GetBytes(buffer);
                    for (var i = 0; i < buffer.Length && sb.Length < length; ++i)
                    {
                        // Divide the byte into charSet-sized groups. If the random value falls into
                        // the last group and the last group is too small to choose from the entire
                        // allowedCharSet, ignore the value in order to avoid biasing the result.
                        if (outOfRangeStart <= buffer[i])
                        {
                            continue;
                        }

                        sb.Append(characters[buffer[i] % characters.Length]);
                    }
                }

                return sb.ToString();
            }
        }

        /// <summary>
        ///     Remove all diacritics (accents) in string 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks> Already handle edge case <see cref="ConvertEdgeCases" /> </remarks>
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
                {
                    var edgeCases = ConvertEdgeCases(c);
                    stringBuilder.Append(edgeCases);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        ///     Normalize: UPPER CASE with remove all diacritic (accents) in string 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        ///     Already handle edge case <see cref="ConvertEdgeCases" />. If value is is Null Or
        ///     WhiteSpace will return string.Empty
        /// </remarks>
        public static string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            value = value.Trim();

            // Convert Edge case
            value = string.Join(string.Empty, value.Select(ConvertEdgeCases));

            var normalizedString = RemoveAccents(value);

            return normalizedString.ToUpperInvariant();
        }

        public static string ConvertEdgeCases(char c)
        {
            string swap;
            switch (c)
            {
                case 'ı':
                    swap = "i";
                    break;

                case 'ł':
                case 'Ł':
                    swap = "l";
                    break;

                case 'đ':
                    swap = "d";
                    break;

                case 'Đ':
                    swap = "D";
                    break;

                case 'ß':
                    swap = "ss";
                    break;

                case 'ø':
                    swap = "o";
                    break;

                case 'Þ':
                    swap = "th";
                    break;

                default:
                    swap = c.ToString();
                    break;
            }

            return swap;
        }

        public static string ReplaceNullOrWhiteSpaceToEmpty(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value;
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
            var regexUtilities = new RegexUtilities();
            return regexUtilities.IsValidEmail(value);
        }

        /// <summary>
        ///     Check string is valid phone number 
        /// </summary>
        /// <param name="value">       Phone number </param>
        /// <param name="countryCode"> Country code, ex: vietnam is 84 </param>
        /// <param name="minLength">   Phone min length without first 0 or country code </param>
        /// <param name="maxLength">   Phone max length without first 0 or country code </param>
        /// <returns></returns>
        public static bool IsValidPhoneNumber(string value, string countryCode = "84", int minLength = 9,
            int maxLength = 10)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            countryCode = countryCode?.Replace("+", string.Empty);
            value = value.Replace("+", string.Empty);

            var regexStartWithZero = $@"^\d[0-9]{{{minLength},{maxLength}}}$";
            var regexStartWithCountryCode = $@"^\+({countryCode})[0-9]{{{minLength},{maxLength}}}$";

            if (value.StartsWith("0"))
                return Regex.Match(value, regexStartWithZero).Success;
            if (value.StartsWith($"+{countryCode}") || value.StartsWith(countryCode))
            {
                value = $"+{value}";
                return Regex.Match(value, regexStartWithCountryCode).Success;
            }

            return false;
        }

        public static string UriBuilder(params string[] uriPaths)
        {
            return uriPaths.Where(x => !string.IsNullOrWhiteSpace(x)).Aggregate((current, path) => $"{current.TrimEnd('/')}/{path.TrimStart('/')}");
        }
    }

    internal class RegexUtilities
    {
        private bool _invalid;

        public bool IsValidEmail(string strIn)
        {
            _invalid = false;
            if (string.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names.
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper,
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
            var idn = new IdnMapping();

            var domainName = match.Groups[2].Value;
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