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
        public static void CreateIfNotExist(params string[] paths)
        {
            foreach (var path in paths)
            {
                StringHelper.CheckNullOrWhiteSpace(path);

                if (!File.Exists(path))
                {
                    File.Create(path);
                }
            }
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
                byte[] bytes = Convert.FromBase64String(value);
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
            var imageInfo = ImageUtils.ImageHelper.GetImageInfo(base64);

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
                // Basic info
                fileModel.FileType = FileType.UnKnown;
                fileModel.Extension = Path.GetExtension(fileModel.Location);
                fileModel.MimeType = fileModel.Extension != null ? MimeTypeHelper.GetMimeType(fileModel.Extension) : null;
            }

            fileModel.Location = string.IsNullOrWhiteSpace(Path.GetExtension(fileModel.Location)) ? (fileModel.Location + fileModel.Extension) : fileModel.Location;

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
    }
}