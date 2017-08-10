#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> LogLevel.cs </Name>
//         <Created> 10/08/17 11:23:19 PM </Created>
//         <Key> 60b281bc-b8e3-4340-a9c7-c15c1be8971a </Key>
//     </File>
//     <Summary>
//         LogLevel.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Puppy.Logger.Core
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LogLevel
    {
        Verbose,
        Debug,
        Information,
        Warning,
        Error,
        Fatal,
    }
}