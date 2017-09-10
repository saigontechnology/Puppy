#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ImageModel.cs </Name>
//         <Created> 10/09/17 8:19:00 PM </Created>
//         <Key> eea3fa06-4d6d-464b-9c9e-3aef225427e2 </Key>
//     </File>
//     <Summary>
//         ImageModel.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

namespace Puppy.Core.ImageUtils
{
    public class ImageModel
    {
        public string MimeType { get; set; }

        public string Extension { get; set; }

        public string DominantHexColor { get; set; }

        public int WidthPx { get; set; }

        public int HeightPx { get; set; }
    }
}