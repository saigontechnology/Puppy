#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> SitemapMvcActionHelper.cs </Name>
//         <Created> 04/07/2017 4:17:38 PM </Created>
//         <Key> 72ae70c1-8bff-40c6-bbd0-cfdc0b9c6005 </Key>
//     </File>
//     <Summary>
//         SitemapMvcActionHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Puppy.Web.SEO.Sitemap
{
    public static class SitemapMvcActionHelper
    {
        public static List<SitemapActionInfo> GetListActionInfo(Assembly asm)
        {
            var listAction = asm.GetTypes()
                .Where(type => typeof(Controller).IsAssignableFrom(type))
                .SelectMany(
                    type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                )
                .Where(m => m.GetCustomAttributes(typeof(SitemapAttribute), true).Any())
                .Select(x => new SitemapActionInfo
                {
                    Controller = x.DeclaringType,
                    Action = x,
                    SitemapFrequency = (x.GetCustomAttributes(typeof(SitemapAttribute), false).LastOrDefault() as SitemapAttribute)?.SitemapFrequency ?? SitemapFrequency.Never,
                    Priority = (x.GetCustomAttributes(typeof(SitemapAttribute), false).LastOrDefault() as SitemapAttribute)?.Priority ?? 0
                })
                .ToList();
            return listAction;
        }

        public static ContentResult GetContentResult(Type startup, IUrlHelper iUrlHelper)
        {
            var asm = startup.GetTypeInfo().Assembly;
            var actionList = SitemapMvcActionHelper.GetListActionInfo(asm);
            var sitemapItems = actionList.Select(
                    x =>
                        new SitemapItem(
                            iUrlHelper.AbsoluteAction(x.Action.Name, x.Controller.Name.Replace("Controller", string.Empty)),
                            changeFrequency: x.SitemapFrequency,
                            priority: x.Priority))
                .ToList();

            var sitemapContentResult = new SitemapGenerator().GenerateContentResult(sitemapItems);
            return sitemapContentResult;
        }
    }
}