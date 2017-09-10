#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> FileType.cs </Name>
//         <Created> 10/09/17 7:45:30 PM </Created>
//         <Key> 59284981-bb8c-440b-94eb-a2a9fbacf48d </Key>
//     </File>
//     <Summary>
//         FileType.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Puppy.Core.FileUtils
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FileType
    {
        UnKnown = 1,
        Image = 2
    }
}