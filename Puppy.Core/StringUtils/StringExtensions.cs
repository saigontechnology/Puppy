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
using System.Diagnostics;
using System.IO;

namespace Puppy.Core.StringUtils
{
    public static class StringExtensions
    {
        [DebuggerStepThrough]
        public static bool IsMissing(this string value)
        {
            return String.IsNullOrWhiteSpace(value);
        }

        [DebuggerStepThrough]
        public static bool IsMissingOrTooLong(this string value, int maxLength)
        {
            return String.IsNullOrWhiteSpace(value) || value.Length > maxLength;
        }

        [DebuggerStepThrough]
        public static bool IsPresent(this string value)
        {
            return !String.IsNullOrWhiteSpace(value);
        }

        [DebuggerStepThrough]
        public static string EnsureLeadingSlash(this string url)
        {
            if (!url.StartsWith("/"))
                return "/" + url;
            return url;
        }

        [DebuggerStepThrough]
        public static string EnsureTrailingSlash(this string url)
        {
            if (!url.EndsWith("/"))
                return url + "/";
            return url;
        }

        [DebuggerStepThrough]
        public static string RemoveLeadingSlash(this string url)
        {
            if (url != null && url.StartsWith("/"))
                url = url.Substring(1);
            return url;
        }

        [DebuggerStepThrough]
        public static string RemoveTrailingSlash(this string url)
        {
            if (url != null && url.EndsWith("/"))
                url = url.Substring(0, url.Length - 1);
            return url;
        }

        [DebuggerStepThrough]
        public static string CleanUrlPath(this string url)
        {
            if (String.IsNullOrWhiteSpace(url))
                url = "/";
            if (url != "/" && url.EndsWith("/"))
                url = url.Substring(0, url.Length - 1);
            return url;
        }

        [DebuggerStepThrough]
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

        [DebuggerStepThrough]
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
            Uri pathUri;

            if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out pathUri))
                throw new ArgumentException($"Invalid path {nameof(path)}");

            if (!pathUri.IsAbsoluteUri)
            {
                path = Path.Combine(Directory.GetCurrentDirectory(), path);
            }

            return path;
        }

        public static bool IsBase64(this string value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                return false;
            }
            try
            {
                var byteArray = Convert.FromBase64String(value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}