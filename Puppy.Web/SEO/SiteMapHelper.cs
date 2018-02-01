#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> SiteMapHelper.cs </Name>
//         <Created> 04/07/2017 5:11:46 PM </Created>
//         <Key> b0de6cdb-b9fe-4b3d-ae4e-8d91c3a6477f </Key>
//     </File>
//     <Summary>
//         SiteMapHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Puppy.Web.SEO.SiteMap;
using System.Globalization;

namespace Puppy.Web.SEO
{
    public static class SiteMapHelper
    {
        /// <summary>
        ///     The maximum number of sitemaps a sitemap index file can contain. 
        /// </summary>
        public const int MaximumSitemapCount = 50000;

        /// <summary>
        ///     The maximum number of sitemap nodes allowed in a sitemap file. The absolute maximum
        ///     allowed is 50,000 according to the specification. See
        ///     http://www.sitemaps.org/protocol.html but the file size must also be less than 10MB.
        ///     After some experimentation, a maximum of 25,000 nodes keeps the file size below 10MB.
        /// </summary>
        public const int MaximumSitemapIndexCount = 25000;

        /// <summary>
        ///     The maximum size of a sitemap file in bytes (10MB). 
        /// </summary>
        public const int MaximumSitemapSizeInBytes = 10485760;

        /// <summary>
        ///     Checks the size of the XML sitemap document. If it is over 10MB, logs an error. 
        /// </summary>
        /// <param name="sitemapXml"> The sitemap XML document. </param>
        public static void CheckDocumentSize(string sitemapXml)
        {
            if (sitemapXml.Length >= MaximumSitemapSizeInBytes)
                throw new SiteMapException(string.Format(CultureInfo.CurrentCulture, "Sitemap exceeds the maximum size of 10MB. This is because you have unusually long URL's. Consider reducing the MaximumSitemapNodeCount. Size:<{0}>.", sitemapXml.Length));
        }

        /// <summary>
        ///     Checks the count of the number of sitemaps. If it is over 50,000, logs an error. 
        /// </summary>
        /// <param name="sitemapCount"> The sitemap count. </param>
        public static void CheckSitemapCount(int sitemapCount)
        {
            if (sitemapCount > MaximumSitemapCount)
                new SiteMapException(string.Format(CultureInfo.CurrentCulture, "Sitemap index file exceeds the maximum number of allowed sitemaps of 50,000. Count:<{1}>", sitemapCount));
        }
    }
}