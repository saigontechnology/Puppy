#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> SitemapImageItem.cs </Name>
//         <Created> 04/07/2017 4:40:15 PM </Created>
//         <Key> 2c71bb2b-7a82-497c-86d6-7014847ab89b </Key>
//     </File>
//     <Summary>
//         SitemapImageItem.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;
using System.Collections.Generic;
using System.Linq;

namespace Puppy.Web.SEO.SitemapImage
{
    /// <summary>
    ///     Represents a sitemap image item. 
    /// </summary>
    public class SitemapImageItem : ISitemapItem
    {
        /// <summary>
        ///     Creates a new instance of <see cref="SitemapImageItem" /> 
        /// </summary>
        /// <param name="url">       URL of the page. </param>
        /// <param name="listImage"> List image </param>
        /// <exception cref="System.ArgumentNullException">
        ///     If the <paramref name="url" /> is null or empty.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        ///     If the <paramref name="listImage" /> is null or empty.
        /// </exception>
        public SitemapImageItem(string url, List<SitemapImageDetail> listImage)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException($"{nameof(url)} is null");
            if (listImage == null || !listImage.Any())
                throw new ArgumentNullException($"{nameof(listImage)} is null");
            Url = url;
            ListImage = listImage;
        }

        public List<SitemapImageDetail> ListImage { get; protected set; }

        /// <summary>
        ///     URL of the page. 
        /// </summary>
        public string Url { get; protected set; }
    }
}