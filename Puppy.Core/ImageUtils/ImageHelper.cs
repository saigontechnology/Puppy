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

using System.Drawing;

namespace Puppy.Core.ImageUtils
{
    public static class ImageHelper
    {
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
    }
}