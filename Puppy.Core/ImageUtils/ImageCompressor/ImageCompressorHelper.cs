using EnumsNET;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Puppy.Core.ImageUtils.ImageCompressor
{
    internal static class ImageCompressorHelper
    {
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

        internal static bool TryGetCompressImageType(string extension, out CompressImageType compressImageType)
        {
            foreach (CompressImageType type in Enum.GetValues(typeof(CompressImageType)))
            {
                if (!string.Equals(type.AsString(EnumFormat.Description), extension, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                compressImageType = type;

                return true;
            }

            compressImageType = CompressImageType.Invalid;

            return false;
        }

        internal static bool TryGetCompressImageType(MemoryStream imageStream, out CompressImageType compressImageType)
        {
            bool isValid = false;

            compressImageType = CompressImageType.Invalid;

            try
            {
                // Raster Image
                using (Image image = Image.FromStream(imageStream))
                {
                    isValid = true;

                    if (ImageFormat.Jpeg.Equals(image.RawFormat))
                    {
                        compressImageType = CompressImageType.Jpeg;
                    }
                    else if (ImageFormat.Png.Equals(image.RawFormat))
                    {
                        compressImageType = CompressImageType.Png;
                    }
                    else if (ImageFormat.Gif.Equals(image.RawFormat))
                    {
                        compressImageType = CompressImageType.Gif;
                    }
                    else
                    {
                        isValid = false;

                        compressImageType = CompressImageType.Invalid;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return isValid;
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
                        qualityPercent = ImageCompressorConstants.DefaultPngQualityPercent;
                        break;

                    case CompressAlgorithm.Jpeg:
                        qualityPercent = ImageCompressorConstants.DefaultJpegQualityPercent;
                        break;

                    case CompressAlgorithm.Gif:
                        qualityPercent = ImageCompressorConstants.DefaultGifQualityPercent;
                        break;
                }
            }
            return qualityPercent;
        }

        /// <summary>
        ///     Min 0 - max 99 
        /// </summary>
        /// <param name="qualityPercent">   </param>
        /// <param name="compressImageType"></param>
        /// <returns></returns>
        internal static int GetQualityPercent(int qualityPercent, CompressImageType compressImageType)
        {
            qualityPercent = qualityPercent < 0 ? 0 : (qualityPercent > 99 ? 99 : qualityPercent);

            if (qualityPercent <= 0)
            {
                switch (compressImageType)
                {
                    case CompressImageType.Png:
                        qualityPercent = ImageCompressorConstants.DefaultPngQualityPercent;
                        break;

                    case CompressImageType.Jpeg:
                        qualityPercent = ImageCompressorConstants.DefaultJpegQualityPercent;
                        break;

                    case CompressImageType.Gif:
                        qualityPercent = ImageCompressorConstants.DefaultGifQualityPercent;
                        break;
                }
            }
            return qualityPercent;
        }
    }
}