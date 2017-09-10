#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> FileModel.cs </Name>
//         <Created> 10/09/17 7:44:42 PM </Created>
//         <Key> a7cb1ef6-dd18-4a8c-97df-0f997417d7ff </Key>
//     </File>
//     <Summary>
//         FileModel.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

namespace Puppy.Core.FileUtils
{
    public class FileModel
    {
        public string Base64 { get; set; }

        /// <summary>
        ///     Saved Location 
        /// </summary>
        public string Location { get; set; }

        public string OriginalFileName { get; set; }

        public FileType FileType { get; set; }

        public string Extension { get; set; }

        public string MimeType { get; set; }

        public double ContentLength { get; set; }

        public string ImageDominantHexColor { get; set; }

        public int ImageWidthPx { get; set; }

        public int ImageHeightPx { get; set; }
    }
}