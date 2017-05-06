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
		public static Random Random = new Random();

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
		///     Normalize: <code>UPPER CASE</code> with <code>remove all diacritic (accents)</code> in string 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		/// <remarks>if value is is Null Or WhiteSpace will return <code>string.Empty</code></remarks>
		public static string Normalize(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
				return string.Empty;

			value = value.Trim();

			var normalizedString = RemoveAccents(value);
			return normalizedString.ToUpperInvariant();
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
	}
}