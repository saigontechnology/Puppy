#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2018 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> RootOnlyContractResolver.cs </Name>
//         <Created> 17/01/2018 8:37:54 PM </Created>
//         <Key> 18d6d62f-df43-41b8-b927-b9b64a7b3243 </Key>
//     </File>
//     <Summary>
//         RootOnlyContractResolver.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Puppy.Core.ObjectUtils
{
    public class RootOnlyContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty prop = base.CreateProperty(member, memberSerialization);

            var propInfo = member as PropertyInfo;

            if (propInfo == null)
            {
                return prop;
            }

            if (propInfo.GetMethod.IsVirtual && !propInfo.GetMethod.IsFinal)
            {
                prop.ShouldSerialize = obj => false;
            }
            return prop;
        }
    }
}