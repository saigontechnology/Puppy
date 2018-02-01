#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> SitemapGenerator.cs </Name>
//         <Created> 07/06/2017 9:57:33 PM </Created>
//         <Key> c3c4f203-8130-4402-a895-e561b724587c </Key>
//     </File>
//     <Summary>
//         SitemapGenerator.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.AspNetCore.Mvc;
using Puppy.Core.XmlUtils;
using Puppy.Web.Constants;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Puppy.Web.SEO.SiteMap
{
    /// <summary>
    ///     Generates sitemap XML. 
    /// </summary>
    public class SiteMapGenerator : ISiteMapGenerator<SiteMapItem>
    {
        private static readonly XNamespace Xmlns = @"http://www.sitemaps.org/schemas/sitemap/0.9";

        private static readonly XNamespace Xsi = @"http://www.w3.org/2001/XMLSchema-instance";

        public virtual ContentResult GenerateContentResult(IEnumerable<SiteMapItem> items)
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

        public virtual string GenerateXmlString(IEnumerable<SiteMapItem> items)
        {
            var sitemapCount = (int)Math.Ceiling(items.Count() / (double)SiteMapHelper.MaximumSitemapIndexCount);
            SiteMapHelper.CheckSitemapCount(sitemapCount);

            if (items == null || !items.Any())
            {
                throw new ArgumentNullException($"{nameof(items)} is null");
            }

            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(Xmlns + "urlset",
                    new XAttribute("xmlns", Xmlns),
                    new XAttribute(XNamespace.Xmlns + "xsi", Xsi),
                    new XAttribute(Xsi + "schemaLocation",
                        @"http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd"),
                    from item in items
                    select CreateItemElement(item)
                )
            );

            var xml = sitemap.ToString(Encoding.UTF8);
            SiteMapHelper.CheckDocumentSize(xml);

            return xml;
        }

        private XElement CreateItemElement(SiteMapItem item)
        {
            var itemElement = new XElement(Xmlns + "url", new XElement(Xmlns + "loc", item.Url.ToLowerInvariant()));

            // all other elements are optional
            if (item.LastModified.HasValue)
                itemElement.Add(new XElement(Xmlns + "lastmod", item.LastModified.Value.ToString("yyyy-MM-ddTHH:mm:sszzz")));

            if (item.ChangeFrequency.HasValue)
                itemElement.Add(new XElement(Xmlns + "changefreq", item.ChangeFrequency.Value.ToString().ToLower()));

            if (item.Priority.HasValue)
                itemElement.Add(new XElement(Xmlns + "priority", item.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));

            return itemElement;
        }
    }
}