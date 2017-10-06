#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> RouteValueDictionaryExtensions.cs </Name>
//         <Created> 26/08/17 11:48:46 PM </Created>
//         <Key> 8af31f70-72a9-4fee-9da9-9569b848a585 </Key>
//     </File>
//     <Summary>
//         RouteValueDictionaryExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Routing;
using Puppy.Core.StringUtils;

namespace Puppy.Web.RouteUtils
{
    public static class RouteValueDictionaryExtensions
    {
        public static string GetUrlWithQueries(this RouteValueDictionary routeValueDictionary, string url)
        {
            foreach (var newLinkValue in routeValueDictionary)
            {
                var hrefKey = "{" + newLinkValue.Key + "}";
                if (url.Contains(hrefKey))
                {
                    url = url.Replace(hrefKey, newLinkValue.Value?.ToString() ?? string.Empty);
                }
                else
                {
                    var hrefValue = newLinkValue.Value?.ToString();

                    if (string.IsNullOrWhiteSpace(hrefValue))
                        continue;

                    var query = $"{newLinkValue.Key}={hrefValue}";
                    url = url.AddQueryString(query);
                }
            }

            return url;
        }

        public static void SafeSetValue(this RouteValueDictionary routeValueDictionary, string key, object value)
        {
            if (routeValueDictionary.ContainsKey(key))
                routeValueDictionary.Remove(key);
            routeValueDictionary.Add(key, value);
        }
    }
}