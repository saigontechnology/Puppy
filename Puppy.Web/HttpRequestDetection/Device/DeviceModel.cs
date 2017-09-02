#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> DeviceModel.cs </Name>
//         <Created> 02/09/17 11:45:14 PM </Created>
//         <Key> 353e0e67-e370-4290-a026-bfaa3940a8f1 </Key>
//     </File>
//     <Summary>
//         DeviceModel.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Http;
using Puppy.Web.Constants;
using System.Linq;

namespace Puppy.Web.HttpRequestDetection.Device
{
    public class DeviceModel
    {
        public DeviceType Type { get; set; }

        public bool IsCrawler { get; set; }

        public DeviceModel()
        {
        }

        public DeviceModel(DeviceType deviceType, bool isIsCrawlerRequest)
        {
            Type = deviceType;
            IsCrawler = isIsCrawlerRequest;
        }

        public DeviceModel(HttpRequest request)
        {
            Type = GetDeviceType(request);
            IsCrawler = IsCrawlerRequest(request);
        }

        private static DeviceType GetDeviceType(HttpRequest request)
        {
            var agent = request.Headers[HeaderKey.UserAgent].ToString();

            if (agent != null && HttpDetectionConstants.TabletAgents.Any(keyword => agent.Contains(keyword)))
                return DeviceType.Tablet;

            if (agent != null && HttpDetectionConstants.MobileAgents.Any(keyword => agent.Contains(keyword)))
                return DeviceType.Mobile;

            if (agent?.Length >= 4 && HttpDetectionConstants.MobileAgentPrefix.Any(prefix => agent.StartsWith(prefix)))
                return DeviceType.Mobile;

            // mobile opera mini special case
            if (request.Headers.Any(header => header.Value.Any(value => value.Contains("OperaMini"))))
                return DeviceType.Mobile;

            // mobile user agent prof detection
            if (request.Headers.ContainsKey("x-wap-profile") || request.Headers.ContainsKey("Profile"))
                return DeviceType.Mobile;

            // mobile accept-header base detection
            if (request.Headers[HeaderKey.Accept].Any(accept => accept.ToLowerInvariant() == "wap"))
                return DeviceType.Mobile;

            return DeviceType.Desktop;
        }

        private static bool IsCrawlerRequest(HttpRequest request)
        {
            var agent = request.Headers[HeaderKey.UserAgent].ToString();
            if (agent == null) return false;
            if (HttpDetectionConstants.CrawlerAgents.Any(keyword => agent.Contains(keyword))) return true;
            return false;
        }
    }
}