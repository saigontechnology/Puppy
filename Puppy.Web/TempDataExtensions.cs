#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> TempDataExtensions.cs </Name>
//         <Created> 09/10/17 9:43:01 PM </Created>
//         <Key> f76cfa32-1805-4b39-9d55-78172d3f2b33 </Key>
//     </File>
//     <Summary>
//         TempDataExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace Puppy.Web
{
    public static class TempDataExtensions
    {
        public static void Set<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            if (tempData.TryGetValue(key, out var o))
            {
                return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
            }

            return null;
        }

        public static void SafeRemove(this ITempDataDictionary tempData, string key)
        {
            if (tempData.TryGetValue(key, out var _))
            {
                tempData.Remove(key);
            }
        }
    }
}