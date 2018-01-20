using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;

namespace Puppy.Core.ImageUtils.ImageCompressor
{
    internal static class Helper
    {
        internal static void WriteToFile(string filePath, MemoryStream stream)
        {
            if (stream == null)
            {
                return;
            }

            using (FileStream file = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
            {
                stream.Position = 0;
                stream.CopyTo(file);
            }
        }

        internal static void WriteToStream(string filePath, MemoryStream stream)
        {
            if ((!Uri.IsWellFormedUriString(filePath, UriKind.RelativeOrAbsolute) && !File.Exists(filePath)))
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

        internal static string GetEnumDescription<T>(this T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException($"{enumerationValue} must be of Enum type", nameof(enumerationValue));
            }

            //Tries to find a DescriptionAttribute for a potential friendly name
            //for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo.Length > 0)
            {
                object[] attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attributes.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attributes[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }

        /// <summary>
        ///     Throw argument exception if path not valid 
        /// </summary>
        /// <param name="filePath"></param>
        /// <example cref="ArgumentException"></example>
        internal static void CheckFilePath(string filePath)
        {
            if (!Uri.IsWellFormedUriString(filePath, UriKind.RelativeOrAbsolute) && !File.Exists(filePath))
            {
                throw new ArgumentException($"{nameof(filePath)} is invalid: {filePath}", nameof(filePath));
            }
        }

        internal static string CreateTemporaryFile(MemoryStream stream, string extension, out long fileSize)
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

        internal static void DeleteFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return;
            }
            try
            {
                File.Delete(filePath);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        internal static bool TryGetImageType(string extension, out ImageType imageType)
        {
            foreach (ImageType type in Enum.GetValues(typeof(ImageType)))
            {
                if (!string.Equals(type.GetEnumDescription(), extension, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                imageType = type;
                return true;
            }
            imageType = ImageType.Invalid;
            return false;
        }

        internal static bool TryGetImageType(MemoryStream imageStream, out ImageType imageType)
        {
            bool isValid = false;

            imageType = ImageType.Invalid;

            try
            {
                // Check Vector image first
                if (IsSvgImage(imageStream))
                {
                    isValid = true;

                    imageType = ImageType.Svg;
                }
                else
                {
                    // Raster check (jpg, png, etc.)
                    using (Image image = Image.FromStream(imageStream))
                    {
                        isValid = true;

                        if (ImageFormat.Jpeg.Equals(image.RawFormat))
                        {
                            imageType = ImageType.Jpeg;
                        }
                        else if (ImageFormat.Png.Equals(image.RawFormat))
                        {
                            imageType = ImageType.Png;
                        }
                        else if (ImageFormat.Gif.Equals(image.RawFormat))
                        {
                            imageType = ImageType.Gif;
                        }
                        else
                        {
                            isValid = false;

                            imageType = ImageType.Invalid;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return isValid;
        }

        internal static bool IsSvgImage(MemoryStream imageStream)
        {
            try
            {
                imageStream.Position = 0;

                byte[] bytes = imageStream.ToArray();

                var text = Encoding.UTF8.GetString(bytes);

                bool isSvgImage = text.StartsWith("<?xml ") || text.StartsWith("<svg ");

                return isSvgImage;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Min 0 - max 99 
        /// </summary>
        /// <param name="qualityPercent"></param>
        /// <param name="algorithm">     </param>
        /// <returns></returns>
        internal static int GetQualityPercent(int qualityPercent, CompressAlgorithm algorithm)
        {
            qualityPercent = qualityPercent < 0 ? 0 : (qualityPercent > 99 ? 99 : qualityPercent);
            if (qualityPercent <= 0)
            {
                switch (algorithm)
                {
                    case CompressAlgorithm.PngPrimary:
                    case CompressAlgorithm.PngSecondary:
                        qualityPercent = Constants.DefaultPngQualityPercent;
                        break;

                    case CompressAlgorithm.Jpeg:
                        qualityPercent = Constants.DefaultJpegQualityPercent;
                        break;

                    case CompressAlgorithm.Gif:
                        qualityPercent = Constants.DefaultGifQualityPercent;
                        break;
                }
            }
            return qualityPercent;
        }

        /// <summary>
        ///     Min 0 - max 99 
        /// </summary>
        /// <param name="qualityPercent"></param>
        /// <param name="imageType">     </param>
        /// <returns></returns>
        internal static int GetQualityPercent(int qualityPercent, ImageType imageType)
        {
            qualityPercent = qualityPercent < 0 ? 0 : (qualityPercent > 99 ? 99 : qualityPercent);
            if (qualityPercent <= 0)
            {
                switch (imageType)
                {
                    case ImageType.Png:
                        qualityPercent = Constants.DefaultPngQualityPercent;
                        break;

                    case ImageType.Jpeg:
                        qualityPercent = Constants.DefaultJpegQualityPercent;
                        break;

                    case ImageType.Gif:
                        qualityPercent = Constants.DefaultGifQualityPercent;
                        break;
                }
            }
            return qualityPercent;
        }

        public static int GetTimeOut(int timeout)
        {
            return timeout > 0 ? timeout : Constants.TimeoutMillisecond;
        }
    }
}