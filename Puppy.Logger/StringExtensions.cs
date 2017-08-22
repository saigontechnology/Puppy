#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> StringExtensions.cs </Name>
//         <Created> 22/08/17 9:31:39 PM </Created>
//         <Key> 41fe1558-f0ba-43ad-a01a-f4f94e65ba1d </Key>
//     </File>
//     <Summary>
//         StringExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;
using System.IO;

namespace Puppy.Logger
{
    public static class StringExtensions
    {
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
    }
}