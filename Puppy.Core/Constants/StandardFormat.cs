#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> StandardFormat.cs </Name>
//         <Created> 11/08/17 5:07:33 PM </Created>
//         <Key> 1bf5b0fa-8b81-4083-a7ce-6b8aee7d62bb </Key>
//     </File>
//     <Summary>
//         StandardFormat.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Puppy.Core.Constants
{
    public static class StandardFormat
    {
        /// <summary>
        ///     Isolate Datetime StandardFormat 
        /// </summary>
        public const string DateTimeOffSetFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

        public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DateFormatString = DateTimeOffSetFormat,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
    }
}