#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore </Project>
//     <File>
//         <Name> StringExtensions.cs </Name>
//         <Created> 28 Apr 17 2:52:15 PM </Created>
//         <Key> e00e427c-6798-4957-a7c2-bcd2f38b2b4a </Key>
//     </File>
//     <Summary>
//         StringExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Puppy.Core.StringUtils
{
    public static class StringExtensions
    {
        public static bool IsMissing(this string value)
        {
            return String.IsNullOrWhiteSpace(value);
        }

        public static bool IsMissingOrTooLong(this string value, int maxLength)
        {
            return String.IsNullOrWhiteSpace(value) || value.Length > maxLength;
        }

        public static bool IsPresent(this string value)
        {
            return !String.IsNullOrWhiteSpace(value);
        }

        public static string EnsureLeadingSlash(this string url)
        {
            if (!url.StartsWith("/"))
                return "/" + url;
            return url;
        }

        public static string EnsureTrailingSlash(this string url)
        {
            if (!url.EndsWith("/"))
                return url + "/";
            return url;
        }

        public static string RemoveLeadingSlash(this string url)
        {
            if (url != null && url.StartsWith("/"))
                url = url.Substring(1);
            return url;
        }

        public static string RemoveTrailingSlash(this string url)
        {
            if (url != null && url.EndsWith("/"))
                url = url.Substring(0, url.Length - 1);
            return url;
        }

        public static string CleanUrlPath(this string url)
        {
            if (String.IsNullOrWhiteSpace(url))
                url = "/";
            if (url != "/" && url.EndsWith("/"))
                url = url.Substring(0, url.Length - 1);
            return url;
        }

        public static bool IsLocalUrl(this string url)
        {
            if (String.IsNullOrEmpty(url))
                return false;
            if (url[0] == 47 && (url.Length == 1 || url[1] != 47 && url[1] != 92))
                return true;
            if (url.Length > 1 && url[0] == 126)
                return url[1] == 47;
            return false;
        }

        public static bool IsUrl(this string value)
        {
            bool isUrl = Uri.TryCreate(value, UriKind.Absolute, out var uriResult) && (uriResult.Scheme.ToLower() == "http" || uriResult.Scheme.ToLower() == "https");
            return isUrl;
        }

        public static string AddQueryString(this string url, string query)
        {
            if (!url.Contains("?"))
                url += "?";
            else if (!url.EndsWith("&"))
                url += "&";
            return url + query;
        }

        public static string GetOrigin(this string url)
        {
            if (url != null && (url.StartsWith("http://") || url.StartsWith("https://")))
            {
                var num = url.IndexOf("//", StringComparison.Ordinal);
                if (num > 0)
                {
                    var length = url.IndexOf("/", num + 2, StringComparison.Ordinal);
                    if (length >= 0)
                        url = url.Substring(0, length);
                    return url;
                }
            }
            return null;
        }

        public static string GetFullPath(this string path)
        {
            if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out var pathUri))
                throw new ArgumentException($"Invalid path {nameof(path)}");

            if (!pathUri.IsAbsoluteUri)
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), path);
            }

            return path;
        }

        public static bool IsBase64(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            try
            {
                var byteArray = Convert.FromBase64String(value);
                return byteArray != null;
            }
            catch
            {
                return false;
            }
        }

        public static string GetSha256(this string value)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
                var hash = BitConverter.ToString(hashBytes).Replace("-", "");
                return hash;
            }
        }

        public static string GetSha512(this string value)
        {
            using (var sha512 = SHA512.Create())
            {
                byte[] hashBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(value));
                var hash = BitConverter.ToString(hashBytes).Replace("-", "");
                return hash;
            }
        }

        public static string GetHmacSha256(this string value, string key)
        {
            var keyBytes = Convert.FromBase64String(key);
            var valueBytes = Encoding.UTF8.GetBytes(value);
            using (var shaAlgorithm = new HMACSHA256(keyBytes))
            {
                var hashBytes = shaAlgorithm.ComputeHash(valueBytes);
                var hash = BitConverter.ToString(hashBytes).Replace("-", "");
                return hash;
            }
        }

        public static string GetHmacSha512(this string value, string key)
        {
            var keyBytes = Convert.FromBase64String(key);
            var valueBytes = Encoding.UTF8.GetBytes(value);
            using (var shaAlgorithm = new HMACSHA512(keyBytes))
            {
                var hashBytes = shaAlgorithm.ComputeHash(valueBytes);
                var hash = BitConverter.ToString(hashBytes).Replace("-", "");
                return hash;
            }
        }
    }
}