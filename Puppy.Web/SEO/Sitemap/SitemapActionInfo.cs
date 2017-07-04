#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> SitemapActionInfo.cs </Name>
//         <Created> 04/07/2017 4:17:05 PM </Created>
//         <Key> f5c569cd-297d-4354-992d-e19d2a96dfcb </Key>
//     </File>
//     <Summary>
//         SitemapActionInfo.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;
using System.Reflection;

namespace Puppy.Web.SEO.Sitemap
{
    public class SitemapActionInfo
    {
        public MethodInfo Action { get; set; }
        public Type Controller { get; set; }
        public double Priority { get; set; }
        public SitemapFrequency SitemapFrequency { get; set; }
    }
}