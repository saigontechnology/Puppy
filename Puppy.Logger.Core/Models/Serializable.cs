#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> Serializable.cs </Name>
//         <Created> 11/08/17 5:11:21 PM </Created>
//         <Key> 08056cc7-81c9-49c9-a4a9-b62c131ff068 </Key>
//     </File>
//     <Summary>
//         Serializable.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;

namespace Puppy.Logger.Core.Models
{
    public abstract class Serializable
    {
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Constant.JsonFormatting, Constant.JsonSerializerSettings);
        }
    }
}