#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore </Project>
//     <File>
//         <Name> ImageHelper.cs </Name>
//         <Created> 25/05/2017 3:10:07 PM </Created>
//         <Key> 523e302f-4cc0-4492-83d0-09b343ad923b </Key>
//     </File>
//     <Summary>
//         ImageHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Puppy.Core.FileUtils;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Puppy.Core.ImageUtils
{
    public static class ImageHelper
    {
        public const string ImageMimeTypeUnknown = "image/unknown";

        public static Color GetDominantColor(string imagePath)
        {
            using (var image = Image.FromFile(imagePath))
            {
                using (var bitmap = new Bitmap(image))
                {
                    return GetDominantColor(bitmap);
                }
            }
        }

        public static Color GetDominantColor(Bitmap bmp)
        {
            var r = 0;
            var g = 0;
            var b = 0;

            var total = 0;

            for (var x = 0; x < bmp.Width; x++)
                for (var y = 0; y < bmp.Height; y++)
                {
                    var clr = bmp.GetPixel(x, y);

                    r += clr.R;
                    g += clr.G;
                    b += clr.B;

                    total++;
                }

            //Calculate Average
            r /= total;
            g /= total;
            b /= total;

            return Color.FromArgb(r, g, b);
        }

        public static bool IsSvgImage(MemoryStream imageStream)
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
        ///     <para> Get image info. </para>
        ///     <para> If not know mime type but valid image then return <see cref="ImageMimeTypeUnknown" /> </para>
        ///     <para> Invalid image will be return <c> NULL </c> </para>
        /// </summary>
        /// <param name="imageStream"></param>
        public static ImageModel GetImageInfo(MemoryStream imageStream)
        {
            try
            {
                ImageModel imageInfo = new ImageModel();

                // Check Vector image first, if image is vector then no info for width and height
                if (IsSvgImage(imageStream))
                {
                    imageInfo.MimeType = "image/svg+xml";
                }
                else
                {
                    // Raster check (jpg, png, etc.)
                    using (Image image = Image.FromStream(imageStream))
                    {
                        // Get image mime type
                        bool isUnknownMimeType = true;
                        foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
                        {
                            if (codec.FormatID == image.RawFormat.Guid)
                            {
                                imageInfo.MimeType = codec.MimeType;
                                isUnknownMimeType = false;
                                break;
                            }
                        }

                        if (isUnknownMimeType)
                        {
                            imageInfo.MimeType = ImageMimeTypeUnknown;
                        }

                        // Get width and height in pixel info
                        imageInfo.WidthPx = image.Width;
                        imageInfo.HeightPx = image.Height;
                    }
                }

                // Get others info
                imageInfo.Extension = MimeTypeHelper.GetExtension(imageInfo.MimeType);

                // Get image dominant color
                using (var bitmap = new Bitmap(imageStream))
                {
                    imageInfo.DominantHexColor = GetDominantColor(bitmap).GetHexCode();
                }

                return imageInfo;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     <para> Get image info. </para>
        ///     <para> If not know mime type but valid image then return <see cref="ImageMimeTypeUnknown" /> </para>
        ///     <para> Invalid image will be return <c> NULL </c> </para>
        /// </summary>
        /// <param name="base64"></param>
        public static ImageModel GetImageInfo(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return GetImageInfo(stream);
            }
        }
    }
}