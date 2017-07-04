#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> SitemapIndexGenerator.cs </Name>
//         <Created> 04/07/2017 4:19:26 PM </Created>
//         <Key> 1cbb0fb9-9270-4a6a-af17-7fac8951e1b1 </Key>
//     </File>
//     <Summary>
//         SitemapIndexGenerator.cs
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

namespace Puppy.Web.SEO.SitemapIndex
{
    /// <summary>
    ///     Generate Sitemap index (see more http://www.sitemaps.org/protocol.html) 
    /// </summary>
    public class SitemapIndexGenerator
    {
        private static readonly XNamespace xmlns = @"http://www.sitemaps.org/schemas/sitemap/0.9";

        private static readonly XNamespace xsi = @"http://www.w3.org/2001/XMLSchema-instance";

        public virtual string GenerateSiteMapIndex(IEnumerable<SitemapIndexItem> items)
        {
            if (items == null || !items.Any())
                throw new ArgumentNullException($"{nameof(items)} is null");

            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(xmlns + "sitemapindex",
                    new XAttribute("xmlns", xmlns),
                    new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                    new XAttribute(xsi + "schemaLocation",
                        @"http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/siteindex.xsd"),
                    from item in items
                    select CreateItemElement(item)
                )
            );

            var xml = sitemap.ToString(Encoding.UTF8);
            SitemapHelper.CheckDocumentSize(xml);
            return xml;
        }

        private XElement CreateItemElement(SitemapIndexItem item)
        {
            var itemElement = new XElement(xmlns + "sitemap", new XElement(xmlns + "loc", item.Url.ToLowerInvariant()));

            // all other elements are optional
            if (item.LastModified.HasValue)
                itemElement.Add(new XElement(xmlns + "lastmod",
                    item.LastModified.Value.ToString("yyyy-MM-ddTHH:mm:sszzz")));
            return itemElement;
        }
    }
}