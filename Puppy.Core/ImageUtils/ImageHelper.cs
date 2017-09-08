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
        ///     <para> Get image mime type. </para>
        ///     <para> If not know mime type but valid image then return <see cref="ImageMimeTypeUnknown" /> </para>
        ///     <para> Invalid image will be return <c> NULL </c> </para>
        /// </summary>
        /// <param name="imageStream"></param>
        public static string GetImageMimeType(MemoryStream imageStream)
        {
            try
            {
                // Check Vector image first
                if (IsSvgImage(imageStream))
                {
                    return "image/svg+xml";
                }

                // Raster check (jpg, png, etc.)
                using (Image image = Image.FromStream(imageStream))
                {
                    foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageDecoders())
                    {
                        if (codec.FormatID == image.RawFormat.Guid)
                            return codec.MimeType;
                    }

                    return ImageMimeTypeUnknown;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     <para> Get image mime type. </para>
        ///     <para> If not know mime type but valid image then return <see cref="ImageMimeTypeUnknown" /> </para>
        ///     <para> Invalid image will be return <c> NULL </c> </para>
        /// </summary>
        public static string GetImageMimeType(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return GetImageMimeType(stream);
            }
        }
    }
}