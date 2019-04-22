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

using Puppy.Core.ImageUtils;
using Puppy.Core.StringUtils;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Puppy.Core.FileUtils
{
    public static class FileHelper
    {
        public static void CreateIfNotExist(params string[] paths)
        {
            foreach (var path in paths)
            {
                StringHelper.CheckNullOrWhiteSpace(path);

                var fullPath = path.GetFullPath();

                if (!File.Exists(fullPath))
                {
                    File.Create(fullPath);
                }
            }
        }

        /// <summary>
        ///     Create an empty temp file with extension 
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string CreateTempFile(string extension)
        {
            string temp = Path.GetTempFileName();
            extension = extension.StartsWith(".") ? extension : $".{extension}";
            var path = Path.ChangeExtension(temp, extension);
            File.Move(temp, path);
            return path;
        }

        /// <summary>
        ///     Create an temp file from stream with extension 
        /// </summary>
        /// <param name="stream">   </param>
        /// <param name="extension"></param>
        /// <param name="fileSize"> </param>
        /// <returns></returns>
        internal static string CreateTempFile(Stream stream, string extension, out long fileSize)
        {
            string tempFile = Path.GetTempFileName();

            string filePath = Path.ChangeExtension(tempFile, extension);

            File.Move(tempFile, filePath);

            // Save the input stream to a temp file for processing.
            using (FileStream fileStream = File.Create(filePath))
            {
                stream.Position = 0;
                stream.CopyTo(fileStream);
            }

            fileSize = stream.Length;

            return filePath;
        }

        public static bool IsLocked(string path)
        {
            StringHelper.CheckNullOrWhiteSpace(path);
            var fileInfo = new FileInfo(path);
            return fileInfo.IsLocked();
        }

        public static bool IsValidBase64(string value)
        {
            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Convert.FromBase64String(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Delete file if exist and can delete 
        /// </summary>
        /// <param name="path"></param>
        /// <returns> return true if success delete file, else is fail </returns>
        public static bool SafeDelete(string path)
        {
            try
            {
                path = path.GetFullPath();

                if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                {
                    return true;
                }

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Save file with content base on <see cref="base64" />, save file as
        ///     <see cref="savePath" /> and store info include <see cref="originalFileName" /> in <see cref="FileModel" />
        /// </summary>
        /// <param name="base64">          
        ///     File content as base 64 format, can not be empty/null
        /// </param>
        /// <param name="originalFileName"> File original name, can be empty/null </param>
        /// <param name="savePath">        
        ///     File save path, if relative path auto combine with
        ///     <see cref="Directory.GetCurrentDirectory" />, can not be empty/null
        /// </param>
        /// <returns></returns>
        public static FileModel Save(string base64, string originalFileName, string savePath)
        {
            StringHelper.CheckNullOrWhiteSpace(base64, savePath);

            FileModel fileModel = new FileModel
            {
                Base64 = base64,
                Location = savePath,
                OriginalFileName = originalFileName
            };

            // Check base 64 is image or not
            var imageInfo = ImageHelper.GetImageInfo(base64);

            if (imageInfo != null)
            {
                // Basic info
                fileModel.FileType = FileType.Image;
                fileModel.MimeType = imageInfo.MimeType;
                fileModel.Extension = MimeTypeHelper.GetExtension(fileModel.MimeType);

                // Image info
                fileModel.ImageWidthPx = imageInfo.WidthPx;
                fileModel.ImageHeightPx = imageInfo.HeightPx;
                fileModel.ImageDominantHexColor = imageInfo.DominantHexColor;
            }
            else
            {
                //Basic info
                fileModel.FileType = FileType.UnKnown;
                fileModel.Extension = Path.GetExtension(fileModel.Location);
                fileModel.MimeType = fileModel.Extension != null ? MimeTypeHelper.GetMimeType(fileModel.Extension) : null;
            }

            fileModel.Location = String.IsNullOrWhiteSpace(Path.GetExtension(fileModel.Location)) ? (fileModel.Location + fileModel.Extension) : fileModel.Location;

            var physicalPath = fileModel.Location.GetFullPath();

            // Save/Write file and get content length
            base64.Save(physicalPath, out var contentLength);
            fileModel.ContentLength = contentLength;

            return fileModel;
        }

        /// <summary>
        ///     Get file size from string, convert to bytes in case string is Base64 format, else get
        ///     size with encoding format.
        /// </summary>
        /// <param name="value">   </param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static int GetByteSize(string value, Encoding encoding)
        {
            if (encoding == null)
            {
                encoding = Encoding.Unicode;
            }

            int fileSize;

            if (IsValidBase64(value))
            {
                byte[] bytes = Convert.FromBase64String(value);
                fileSize = bytes.Length;
            }
            else
            {
                fileSize = encoding.GetByteCount(value);
            }

            return fileSize;
        }

        public static long GetByteSize(string filePath)
        {
            filePath = filePath.GetFullPath();
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length;
        }

        /// <summary>
        ///     Replaces characters in <c> text </c> that are not allowed in file names with the
        ///     specified replacement character.
        /// </summary>
        /// <param name="text">          
        ///     Text to make into a valid filename. The same string is returned if it is valid already.
        /// </param>
        /// <param name="replacement">   
        ///     Replacement character, or null to simply remove bad characters.
        /// </param>
        /// <param name="isFancy">       
        ///     Whether to replace quotes and slashes with the non-ASCII characters ” and ⁄.
        /// </param>
        /// <param name="isRemoveAccent"> Remove all diacritics (accents) in string </param>
        /// <returns>
        ///     A string that can be used as a filename. If the output string would otherwise be
        ///     empty, returns <see cref="replacement" /> as string.
        /// </returns>
        /// <remarks>
        ///     Valid file name also follow maximum length is 260 characters rule (split from right
        ///     to left if length &gt; 260)
        /// </remarks>
        public static string MakeValidFileName(string text, char? replacement = '_', bool isFancy = true, bool isRemoveAccent = true)
        {
            if (isRemoveAccent)
            {
                text = StringHelper.RemoveAccents(text);
            }

            StringBuilder sb = new StringBuilder(text.Length);

            var invalids = Path.GetInvalidFileNameChars();

            bool changed = false;

            foreach (var c in text)
            {
                if (invalids.Contains(c))
                {
                    changed = true;

                    var replace = replacement ?? '\0';

                    if (isFancy)
                    {
                        if (c == '"') replace = '”'; // U+201D right double quotation mark
                        else if (c == '\'') replace = '’'; // U+2019 right single quotation mark
                        else if (c == '/') replace = '⁄'; // U+2044 fraction slash
                    }

                    if (replace != '\0')
                    {
                        sb.Append(replace);
                    }
                }
                else
                {
                    sb.Append(c);
                }
            }

            if (sb.Length == 0)
            {
                return replacement?.ToString();
            }

            var fileName = changed ? sb.ToString() : text;

            if (fileName?.Length > 260)
            {
                return fileName.Substring(fileName.Length - 260);
            }

            return fileName;
        }

        public static void WriteToStream(string filePath, MemoryStream stream)
        {
            if (!Uri.IsWellFormedUriString(filePath, UriKind.RelativeOrAbsolute) && !File.Exists(filePath))
            {
                return;
            }

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

        /// <summary>
        ///     Get all file from path
        /// </summary>
        /// <returns></returns>
        public static FileInfo[] GetAllFile(string folderPath, string searchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories)
        {
            var dirInfo = new DirectoryInfo(folderPath);
            var allFiles = dirInfo.GetFiles(searchPattern, searchOption);

            return allFiles;
        }
    }
}