#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore.WebAPI </Project>
//     <File>
//         <Name> OtpTracking.cs </Name>
//         <Created> 29/05/2017 4:08:19 PM </Created>
//         <Key> 68153f56-7a9e-4cee-8418-077ebc614938 </Key>
//     </File>
//     <Summary>
//         OtpTracking.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using TopCore.Auth.Domain.Utils;
using TopCore.Framework.EF;

namespace TopCore.Auth.Domain.Entities
{
    public sealed class OtpTracking : EntityBase
    {
        public OtpTracking()
        {
            CreatedTime = SystemUtils.GetSystemTimeNow();
        }

        public string RequestIpAddress { get; set; }

        public string ClientId { get; set; }

        public string UserId { get; set; }
    }
}