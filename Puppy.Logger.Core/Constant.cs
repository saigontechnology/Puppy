#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> Constant.cs </Name>
//         <Created> 11/08/17 5:07:33 PM </Created>
//         <Key> 1bf5b0fa-8b81-4083-a7ce-6b8aee7d62bb </Key>
//     </File>
//     <Summary>
//         Constant.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;

namespace Puppy.Logger.Core
{
    public static class Constant
    {
        public const string DateTimeOffSetFormat = "yyyy/MM/dd hh:mm:ss.fff zzz";

        public static readonly Formatting JsonFormatting = Formatting.Indented;

        public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Error
        };
    }
}