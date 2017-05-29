#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore</Project>
//     <File>
//         <Name> ColorExtensions.cs </Name>
//         <Created> 25/05/2017 5:58:32 PM </Created>
//         <Key> d4315339-dad3-4855-a416-dab0c7286d72 </Key>
//     </File>
//     <Summary>
//         ColorExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System.Drawing;

namespace Puppy.Core.ImageUtils
{
    public static class ColorExtensions
    {
        public static string GetHexCode(this Color color)
        {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }
    }
}