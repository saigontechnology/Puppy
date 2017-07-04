#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> SitemapImageDetail.cs </Name>
//         <Created> 04/07/2017 4:40:32 PM </Created>
//         <Key> 13735022-0d82-4515-a091-d481a91e80ca </Key>
//     </File>
//     <Summary>
//         SitemapImageDetail.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

namespace Puppy.Web.SEO.SitemapImage
{
    public class SitemapImageDetail
    {
        /// <summary>
        ///     Caption/description of image. 
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        ///     Geo location of image. 
        /// </summary>
        public string GeoLocation { get; set; }

        /// <summary>
        ///     URL of image. 
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        ///     Url to License of image 
        /// </summary>
        public string License { get; set; }

        /// <summary>
        ///     Title of image. 
        /// </summary>
        public string Title { get; set; }
    }
}