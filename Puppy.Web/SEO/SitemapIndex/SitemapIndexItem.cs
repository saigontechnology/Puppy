#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> SitemapIndexItem.cs </Name>
//         <Created> 04/07/2017 4:19:02 PM </Created>
//         <Key> b91f83d5-4c35-4570-b75a-5c1012168d0b </Key>
//     </File>
//     <Summary>
//         SitemapIndexItem.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;

namespace Puppy.Web.SEO.SitemapIndex
{
    /// <summary>
    ///     Represents a sitemap index item. 
    /// </summary>
    public class SitemapIndexItem : ISitemapItem
    {
        /// <summary>
        ///     Creates a new instance of <see cref="SitemapIndexItem" /> 
        /// </summary>
        /// <param name="url">          URL of the page. Optional. </param>
        /// <param name="lastModified"> The date of last modification of the file. Optional. </param>
        /// <exception cref="System.ArgumentNullException">
        ///     If the <paramref name="url" /> is null or empty.
        /// </exception>
        public SitemapIndexItem(string url, DateTime? lastModified = null)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException($"{nameof(url)} is null");

            Url = url;
            LastModified = lastModified;
        }

        /// <summary>
        ///     The date of last modification of the file. 
        /// </summary>
        public DateTime? LastModified { get; protected set; }

        /// <summary>
        ///     URL of the page. 
        /// </summary>
        public string Url { get; protected set; }
    }
}