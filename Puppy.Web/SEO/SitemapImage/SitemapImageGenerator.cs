#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> SitemapImageGenerator.cs </Name>
//         <Created> 04/07/2017 4:40:58 PM </Created>
//         <Key> 74f52908-8429-469d-9a87-7f3d43cbe552 </Key>
//     </File>
//     <Summary>
//         SitemapImageGenerator.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Puppy.Core;
using Puppy.Web.SEO.Sitemap;

namespace Puppy.Web.SEO.SitemapImage
{
    /// <summary>
    ///     Generate Image Sitemap (see more https://support.google.com/webmasters/answer/178636?hl=en) 
    /// </summary>
    public class SitemapImageGenerator
    {
        public static readonly XNamespace image = @"http://www.google.com/schemas/sitemap-image/1.1";

        public static readonly XNamespace xmlns = @"http://www.sitemaps.org/schemas/sitemap/0.9";

        public virtual string GenerateSiteMapImage(IEnumerable<SitemapImageItem> items)
        {
            if (items == null || !items.Any())
                throw new ArgumentNullException($"{nameof(items)} is null");

            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(xmlns + "urlset",
                    new XAttribute("xmlns", xmlns),
                    new XAttribute(XNamespace.Xmlns + "image", image),
                    from item in items
                    select CreateItemElement(item)
                )
            );

            var xml = sitemap.ToString(Encoding.UTF8);
            SitemapHelper.CheckDocumentSize(xml);
            return xml;
        }

        private string CreateItemElement(SitemapImageItem item)
        {
            var itemElement = new XElement(xmlns + "url", new XElement(xmlns + "loc", item.Url.ToLowerInvariant()));

            foreach (var sitemapImageDetail in item.ListImage)
            {
                var imageElement = new XElement(image + "image");

                // all other elements are optional
                imageElement.Add(new XElement(image + "loc", sitemapImageDetail.ImagePath.ToLowerInvariant()));

                if (!string.IsNullOrWhiteSpace(sitemapImageDetail.Caption))
                    imageElement.Add(new XElement(image + "caption", sitemapImageDetail.Caption));

                if (!string.IsNullOrWhiteSpace(sitemapImageDetail.GeoLocation))
                    imageElement.Add(new XElement(image + "geo_location", sitemapImageDetail.GeoLocation));

                if (!string.IsNullOrWhiteSpace(sitemapImageDetail.Title))
                    imageElement.Add(new XElement(image + "title", sitemapImageDetail.Title));

                if (!string.IsNullOrWhiteSpace(sitemapImageDetail.License))
                    imageElement.Add(new XElement(image + "license", sitemapImageDetail.License));

                itemElement.Add(imageElement);
            }

            var document = new XDocument(itemElement);
            var xml = document.ToString(Encoding.UTF8);
            return xml;
        }
    }
}