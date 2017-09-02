#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> FileExtensions.cs </Name>
//         <Created> 02/09/17 9:02:15 PM </Created>
//         <Key> ed7b90e5-522a-47b4-adfc-1c11cec63b51 </Key>
//     </File>
//     <Summary>
//         FileExtensions.cs
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
    public static class FileExtensions
    {
        public static void Save(this byte[] byteArray, string path)
        {
            path = path.GetFullPath();

            using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                stream.Write(byteArray, 0, byteArray.Length);
                stream.Flush();
            }
        }

        /// <summary>
        ///     Save File from Base 64 Data 
        /// </summary>
        /// <param name="base64"></param>
        /// <param name="path">  </param>
        /// <returns> Relative New File Path </returns>
        public static void Save(this string base64, string path)
        {
            StringHelper.CheckNullOrWhiteSpace(path);

            path = path.GetFullPath();

            byte[] bytes = Convert.FromBase64String(base64);
            bytes.Save(path);
        }

        public static void Save(this MemoryStream stream, string path)
        {
            StringHelper.CheckNullOrWhiteSpace(path);

            path = path.GetFullPath();

            using (var file = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                stream.Position = 0;
                stream.CopyTo(file);
            }
        }

        public static void Save(this StringWriter stringWriter, string path)
        {
            StringHelper.CheckNullOrWhiteSpace(path);

            path = path.GetFullPath();

            byte[] buffer = Encoding.ASCII.GetBytes(stringWriter.ToString());
            buffer.Save(path);
        }

        public static void Save(this string path, MemoryStream stream)
        {
            StringHelper.CheckNullOrWhiteSpace(path);

            path = path.GetFullPath();

            using (FileStream fileStream = File.OpenRead(path))
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

        public static string GetMimeType(this FileInfo fileInfo)
        {
            var mimeType = MimeTypeHelper.GetMimeType(fileInfo.Extension);
            return mimeType;
        }

        public static bool IsLocked(this FileInfo fileInfo)
        {
            FileStream fileStream = null;
            try
            {
                fileStream = fileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                fileStream?.Dispose();
            }
            return false;
        }
    }
}