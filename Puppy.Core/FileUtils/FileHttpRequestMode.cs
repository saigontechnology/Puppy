#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> FileHttpAccessMode.cs </Name>
//         <Created> 10/09/17 7:49:48 PM </Created>
//         <Key> 7dfd88dd-1067-4055-aad2-6ec286f38a6f </Key>
//     </File>
//     <Summary>
//         FileHttpAccessMode.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Puppy.Core.FileUtils
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FileHttpRequestMode
    {
        View = 1,
        Download = 2
    }
}