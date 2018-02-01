#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy → Interface </Project>
//     <File>
//         <Name> ISitemapGenerator.cs </Name>
//         <Created> 07/07/17 11:52:08 AM </Created>
//         <Key> 0877cdcc-5c6b-4068-ad3f-ee4412a701d4 </Key>
//     </File>
//     <Summary>
//         ISitemapGenerator.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Puppy.Web.SEO
{
    public interface ISiteMapGenerator<T> where T : class, ISiteMapItem
    {
        string GenerateXmlString(IEnumerable<T> items);

        ContentResult GenerateContentResult(IEnumerable<T> items);
    }
}