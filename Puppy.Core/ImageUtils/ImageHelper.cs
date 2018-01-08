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
using Puppy.Core.StringUtils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using Image = System.Drawing.Image;

namespace Puppy.Core.ImageUtils
{
    public static class ImageHelper
    {
        public const string ImageMimeTypeUnknown = "image/unknown";

        #region Dominant Color

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

        /// <summary>
        ///     Try get dominant color, return true if get dominant color success, else is fail 
        /// </summary>
        /// <param name="imagePath">    </param>
        /// <param name="dominantColor"></param>
        /// <returns></returns>
        /// <remarks> return null in case fail </remarks>
        public static bool TryGetDominantColor(string imagePath, out Color? dominantColor)
        {
            try
            {
                dominantColor = GetDominantColor(imagePath);

                return true;
            }
            catch
            {
                dominantColor = null;

                return false;
            }
        }

        public static Color GetDominantColor(Bitmap bmp)
        {
            // Scale image to standard size (Max width is 1024, max height is 768)
            float width = Math.Min(bmp.Width, 1024);
            float height = Math.Min(bmp.Height, 768);
            int scale = (int)Math.Min(bmp.Width / width, bmp.Height / height);
            Bitmap bmpResize = new Bitmap(bmp, new Size(bmp.Width / scale, bmp.Height / scale));

            var r = 0;
            var g = 0;
            var b = 0;

            var total = 0;

            for (var x = 0; x < bmpResize.Width; x++)
                for (var y = 0; y < bmpResize.Height; y++)
                {
                    var clr = bmpResize.GetPixel(x, y);

                    r += clr.R;
                    g += clr.G;
                    b += clr.B;

                    total++;
                }

            //Calculate Average
            r /= total;
            g /= total;
            b /= total;

            Color color = Color.FromArgb(r, g, b);
            return color;
        }

        /// <summary>
        ///     Try get dominant color, return true if get dominant color success, else is fail 
        /// </summary>
        /// <param name="bmp">          </param>
        /// <param name="dominantColor"></param>
        /// <returns></returns>
        /// <remarks> return null in case fail </remarks>
        public static bool TryGetDominantColor(Bitmap bmp, out Color? dominantColor)
        {
            try
            {
                dominantColor = GetDominantColor(bmp);
                return true;
            }
            catch
            {
                dominantColor = null;
                return false;
            }
        }

        #endregion

        #region Image Info

        /// <summary>
        ///     <para> Get image info. </para>
        ///     <para> If not know mime type but valid image then return <see cref="ImageMimeTypeUnknown" /> </para>
        ///     <para> Invalid image will be return <c> NULL </c> </para>
        /// </summary>
        /// <param name="base64"></param>
        public static ImageModel GetImageInfo(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);

            return GetImageInfo(bytes);
        }

        /// <summary>
        ///     <para> Get image info. </para>
        ///     <para> If not know mime type but valid image then return <see cref="ImageMimeTypeUnknown" /> </para>
        ///     <para> Invalid image will be return <c> NULL </c> </para>
        /// </summary>
        /// <param name="bytes"></param>
        public static ImageModel GetImageInfo(byte[] bytes)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return GetImageInfo(stream);
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
                    using (var image = Image.FromStream(imageStream))
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

        #endregion

        #region Image Resize

        public static byte[] Resize(string path, int newWidth, int newHeight)
        {
            path = path.GetFullPath();

            byte[] imageBytes = File.ReadAllBytes(path);

            return Resize(imageBytes, newWidth, newHeight);
        }

        public static byte[] Resize(byte[] imageBytes, int newWidth, int newHeight)
        {
            using (MemoryStream inStream = new MemoryStream(imageBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load<Rgba32>(inStream, out IImageFormat format))
                    {
                        image.Mutate(x => x.Resize(newWidth, newHeight));

                        image.Save(outStream, format);

                        return outStream.ToArray();
                    }
                }
            }
        }

        #endregion

        #region Generate Text Image

        /// <summary>
        ///     Generate image from text (at center of the image) 
        /// </summary>
        /// <param name="text">            Will be StringHelper.Normalize(text).First().ToString() </param>
        /// <param name="height">          Default is 50 px </param>
        /// <param name="width">           Default is 50 px </param>
        /// <param name="font">           
        ///     Default is new Font(FontFamily.GenericSansSerif, 10.0F, FontStyle.Bold)
        /// </param>
        /// <param name="textColor">       Default is Color.White </param>
        /// <param name="backgroundColor"> Default is Color.Black </param>
        /// <returns></returns>
        public static string GenerateTextImageBase64(string text, int height = 50, int width = 50, Font font = null, Color textColor = default, Color backgroundColor = default)
        {
            text = StringHelper.Normalize(text).First().ToString();

            if (font == null)
            {
                font = new Font(FontFamily.GenericSansSerif, 10.0F, FontStyle.Bold);
            }

            if (textColor == default)
            {
                textColor = Color.White;
            }

            if (backgroundColor == default)
            {
                backgroundColor = Color.Black;
            }

            // Generate Image

            var img = GenerateTextImage(text, height, width, font, textColor, backgroundColor);

            // Convert to image array

            var converter = new ImageConverter();

            var imageArray = converter.ConvertTo(img, typeof(byte[])) as byte[];

            var stringBase64 = Convert.ToBase64String(imageArray);

            return stringBase64;
        }

        /// <summary>
        ///     Generate image from text (at center of the image) 
        /// </summary>
        /// <param name="text">           </param>
        /// <param name="height">         </param>
        /// <param name="width">          </param>
        /// <param name="font">           </param>
        /// <param name="textColor">      </param>
        /// <param name="backgroundColor"></param>
        /// <returns></returns>
        public static Image GenerateTextImage(string text, int height, int width, Font font, Color textColor, Color backgroundColor)
        {
            StringHelper.CheckNullOrWhiteSpace(text);

            // Create a dummy bitmap just to get a graphics object

            Image img = new Bitmap(1, 1);

            Graphics drawing = Graphics.FromImage(img);

            // Measure the string to see how big the image needs to be

            drawing.MeasureString(text, font);

            // Free up the dummy image and old graphics object

            img.Dispose();

            drawing.Dispose();

            // Create a new image of the right size

            img = new Bitmap(height, width);

            drawing = Graphics.FromImage(img);

            // Paint the background

            drawing.Clear(backgroundColor);

            // Create a brush for the text

            Brush brush = new SolidBrush(textColor);

            // String alignment

            StringFormat stringFormat = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            // Rectangular

            RectangleF rectangleF = new RectangleF(0, 0, img.Width, img.Height);

            // Draw text on image

            drawing.DrawString(text, font, brush, rectangleF, stringFormat);

            // Save drawing

            drawing.Save();

            // Dispose

            brush.Dispose();

            drawing.Dispose();

            // return image
            return img;
        }

        #endregion

        #region Base 64

        /// <summary>
        ///     Get image base 64 (data:image/jpg;base64,{string base 64}) from string base 64 and
        ///     image extension
        /// </summary>
        /// <param name="base64">        </param>
        /// <param name="imageExtension"></param>
        /// <returns></returns>
        public static string GetImageBase64Format(string base64, string imageExtension = ".jpg")
        {
            var imageMimeType = MimeTypeHelper.GetMimeType(imageExtension);

            return $@"data:{imageMimeType};base64,{base64}";
        }

        /// <summary>
        ///     Get string base 64 format from image base 64 format (data:image/jpg;base64,{string
        ///     base 64})
        /// </summary>
        /// <param name="imageBase64"></param>
        /// <returns></returns>
        public static string GetBase64Format(string imageBase64)
        {
            return imageBase64.Split(',').LastOrDefault();
        }

        #endregion
    }
}