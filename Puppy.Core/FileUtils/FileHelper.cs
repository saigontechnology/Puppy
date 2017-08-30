#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> FileHelper.cs </Name>
//         <Created> 30/08/17 5:29:36 PM </Created>
//         <Key> f3912706-12ac-4685-a6b6-7ac383fdaa49 </Key>
//     </File>
//     <Summary>
//         FileHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Puppy.Core.StringUtils;
using System;
using System.IO;
using System.Text;

namespace Puppy.Core.FileUtils
{
    public static class FileHelper
    {
        public static void Save(this byte[] byteArray, string filePath)
        {
            filePath = filePath.GetFullPath();

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                stream.Write(byteArray, 0, byteArray.Length);
                stream.Flush();
            }
        }

        /// <summary>
        ///     Save File from Base 64 Data 
        /// </summary>
        /// <param name="base64">  </param>
        /// <param name="filePath"></param>
        /// <returns> Relative New File Path </returns>
        public static void Save(this string base64, string filePath)
        {
            filePath = filePath.GetFullPath();

            byte[] bytes = Convert.FromBase64String(base64);
            bytes.Save(filePath);
        }

        public static void Save(this MemoryStream stream, string filePath)
        {
            filePath = filePath.GetFullPath();

            using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                stream.Position = 0;
                stream.CopyTo(file);
            }
        }

        public static void Save(this StringWriter stringWriter, string filePath)
        {
            filePath = filePath.GetFullPath();

            byte[] buffer = Encoding.ASCII.GetBytes(stringWriter.ToString());
            buffer.Save(filePath);
        }

        internal static void Save(this string filePath, MemoryStream stream)
        {
            filePath = filePath.GetFullPath();

            using (FileStream fileStream = File.OpenRead(filePath))
            {
                // Replace contents.
                stream.SetLength(0);

                // Copy file to stream
                fileStream.Position = 0;
                fileStream.CopyTo(stream);

                // Reset position after copy
                stream.Position = 0;
            }
        }
    }
}