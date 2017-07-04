#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> SitemapAttribute.cs </Name>
//         <Created> 04/07/2017 4:15:22 PM </Created>
//         <Key> 5416b948-6d3a-46f7-b87e-c81538a1bec5 </Key>
//     </File>
//     <Summary>
//         SitemapAttribute.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;

namespace Puppy.Web.SEO.Sitemap
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class SitemapAttribute : Attribute
    {
        public SitemapAttribute(SitemapFrequency sitemapFrequency, double priority)
        {
            SitemapFrequency = sitemapFrequency;
            Priority = priority;
        }

        public double Priority { get; set; }
        public SitemapFrequency SitemapFrequency { get; set; }
    }
}