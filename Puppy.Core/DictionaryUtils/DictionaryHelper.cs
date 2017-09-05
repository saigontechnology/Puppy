#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> DictionaryHelper.cs </Name>
//         <Created> 03/09/17 2:20:31 PM </Created>
//         <Key> b6fc68cf-437c-4dbc-b647-78615413d4f4 </Key>
//     </File>
//     <Summary>
//         DictionaryHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;
using Puppy.Core.ObjectUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Puppy.Core.DictionaryUtils
{
    public static class DictionaryHelper
    {
        public static Dictionary<string, string> ToDictionary<T>(T data)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            List<string> properties = typeof(T).GetProperties().Select(x => x.Name).ToList();

            foreach (var propertyname in properties)
            {
                var propertyData = data.GetPropertyValue(propertyname);

                var dataStr = Convert.GetTypeCode(propertyData) != TypeCode.Object
                    ? propertyData as string
                    : JsonConvert.SerializeObject(propertyData, Constants.StandardFormat.JsonSerializerSettings).Trim('"');

                if (!string.IsNullOrEmpty(dataStr))
                {
                    dictionary.Add(propertyname, dataStr);
                }
            }

            return dictionary;
        }
    }
}