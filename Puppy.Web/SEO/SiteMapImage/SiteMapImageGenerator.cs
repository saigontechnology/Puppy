#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> SiteMapImageGenerator.cs </Name>
//         <Created> 04/07/2017 4:40:58 PM </Created>
//         <Key> 74f52908-8429-469d-9a87-7f3d43cbe552 </Key>
//     </File>
//     <Summary>
//         SiteMapImageGenerator.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.AspNetCore.Mvc;
using Puppy.Core.XmlUtils;
using Puppy.Web.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Puppy.Web.SEO.SiteMapImage
{
    /// <summary>
    ///     Generate Image Sitemap (see more
    ///     https://support.google.com/webmasters/answer/178636?hl=en) List up to 1,000 images for
    ///     each page
    /// </summary>
    public class SiteMapImageGenerator : ISiteMapGenerator<SiteMapImageItem>
    {
        public static readonly XNamespace Image = @"http://www.google.com/schemas/sitemap-image/1.1";

        public static readonly XNamespace Xmlns = @"http://www.sitemaps.org/schemas/sitemap/0.9";

        public virtual ContentResult GenerateContentResult(IEnumerable<SiteMapImageItem> items)
        {
            string sitemapContent = GenerateXmlString(items);

            ContentResult contentResult = new ContentResult
            {
                ContentType = ContentType.Xml,
                StatusCode = 200,
                Content = sitemapContent
            };
            return contentResult;
        }

        public virtual string GenerateXmlString(IEnumerable<SiteMapImageItem> items)
        {
            if (items == null || !items.Any())
                throw new ArgumentNullException($"{nameof(items)} is null");

            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(Xmlns + "urlset",
                    new XAttribute("xmlns", Xmlns),
                    new XAttribute(XNamespace.Xmlns + "image", Image),
                    from item in items
                    select CreateItemElement(item)
                )
            );

            var xml = sitemap.ToString(Encoding.UTF8);
            SiteMapHelper.CheckDocumentSize(xml);
            return xml;
        }

        private string CreateItemElement(SiteMapImageItem item)
        {
            var itemElement = new XElement(Xmlns + "url", new XElement(Xmlns + "loc", item.Url.ToLowerInvariant()));

            foreach (var SiteMapImageDetail in item.ListImage)
            {
                var imageElement = new XElement(Image + "image");

                // all other elements are optional
                imageElement.Add(new XElement(Image + "loc", SiteMapImageDetail.ImagePath.ToLowerInvariant()));

                if (!string.IsNullOrWhiteSpace(SiteMapImageDetail.Caption))
                    imageElement.Add(new XElement(Image + "caption", SiteMapImageDetail.Caption));

                if (!string.IsNullOrWhiteSpace(SiteMapImageDetail.GeoLocation))
                    imageElement.Add(new XElement(Image + "geo_location", SiteMapImageDetail.GeoLocation));

                if (!string.IsNullOrWhiteSpace(SiteMapImageDetail.Title))
                    imageElement.Add(new XElement(Image + "title", SiteMapImageDetail.Title));

                if (!string.IsNullOrWhiteSpace(SiteMapImageDetail.License))
                    imageElement.Add(new XElement(Image + "license", SiteMapImageDetail.License));

                itemElement.Add(imageElement);
            }

            var document = new XDocument(itemElement);
            var xml = document.ToString(Encoding.UTF8);
            return xml;
        }
    }
}