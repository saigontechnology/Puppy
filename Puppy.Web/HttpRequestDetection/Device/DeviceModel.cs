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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Puppy.Core.Models;
using Puppy.Core.StringUtils;
using Puppy.Web.Constants;
using System.Linq;
using System.Text.RegularExpressions;

namespace Puppy.Web.HttpRequestDetection.Device
{
    public class DeviceModel : SerializableModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public DeviceType Type { get; set; }

        public bool IsCrawler { get; set; }

        // Marker
        public string MarkerFullInfo { get; set; }

        public string MarkerName { get; set; }

        public string MarkerVersion { get; set; }

        // OS
        public string OsFullInfo { get; set; }

        public string OsName { get; set; }

        public string OsVersion { get; set; }

        // Engine
        public string EngineFullInfo { get; set; }

        public string EngineName { get; set; }

        public string EngineVersion { get; set; }

        // Browser
        public string BrowserFullInfo { get; set; }

        public string BrowserName { get; set; }

        public string BrowserVersion { get; set; }

        // Others

        public string IpAddress { get; set; }

        public string UserAgent { get; set; }

        public string DeviceHash { get; set; }

        public DeviceModel()
        {
        }

        public DeviceModel(HttpRequest request)
        {
            Type = GetDeviceType(request);
            IsCrawler = GetIsCrawlerRequest(request);

            // Marker
            MarkerFullInfo = HttpDetectionHelper.GetMarkerFullInfo(request);
            MarkerName = HttpDetectionHelper.GetMarkerName(request);
            MarkerVersion = HttpDetectionHelper.GetMarkerVersion(request);

            // OS
            OsFullInfo = HttpDetectionHelper.GetOsFullInfo(request);
            OsName = HttpDetectionHelper.GetOsName(request);
            OsVersion = HttpDetectionHelper.GetOsVersion(request);

            // Engine
            EngineFullInfo = HttpDetectionHelper.GetEngineFullInfo(request);
            EngineName = HttpDetectionHelper.GetEngineName(request);
            EngineVersion = HttpDetectionHelper.GetEngineVersion(request);

            // Browser
            BrowserFullInfo = HttpDetectionHelper.GetBrowserFullInfo(request);
            BrowserName = HttpDetectionHelper.GetBrowserName(request);
            BrowserVersion = HttpDetectionHelper.GetBrowserVersion(request);

            // Others
            IpAddress = HttpDetectionHelper.GetIpAddress(request);
            UserAgent = HttpDetectionHelper.GetUserAgent(request);

            DeviceHash = GetDeviceHash(this);
        }

        private static DeviceType GetDeviceType(HttpRequest request)
        {
            var agent = HttpDetectionHelper.GetUserAgent(request, true);
            if (string.IsNullOrWhiteSpace(agent)) return DeviceType.Unknown;

            if (Regex.IsMatch(agent, HttpDetectionConstants.TabletAgentsRegex, RegexOptions.IgnoreCase))
                return DeviceType.Tablet;

            if (Regex.IsMatch(agent, HttpDetectionConstants.MobileAgentsRegex, RegexOptions.IgnoreCase))
                return DeviceType.Mobile;

            // mobile opera mini special case
            if (request.Headers.Any(header => header.Value.Any(value => value.Contains("operamini"))))
                return DeviceType.Mobile;

            // mobile user agent prof detection
            if (request.Headers.ContainsKey("x-wap-profile") || request.Headers.ContainsKey("profile"))
                return DeviceType.Mobile;

            // mobile accept-header base detection
            if (request.Headers[HeaderKey.Accept].Any(accept => accept.ToLowerInvariant() == "wap"))
                return DeviceType.Mobile;

            return DeviceType.Desktop;
        }

        private static bool GetIsCrawlerRequest(HttpRequest request)
        {
            var agent = HttpDetectionHelper.GetUserAgent(request, true);
            if (string.IsNullOrWhiteSpace(agent)) return false;

            if (Regex.IsMatch(agent, HttpDetectionConstants.CrawlerAgentsRegex, RegexOptions.IgnoreCase))
                return true;

            return false;
        }

        private static string GetDeviceHash(DeviceModel deviceModel)
        {
            string ipAddress = string.IsNullOrWhiteSpace(deviceModel.IpAddress) ? StringHelper.GetRandomString(16) : deviceModel.IpAddress;
            string identityDevice = $"{deviceModel.OsName}|{deviceModel.OsVersion}_{deviceModel.EngineName}|{deviceModel.EngineVersion}_{deviceModel.BrowserName}|{deviceModel.BrowserVersion}_{ipAddress}";
            return identityDevice.GetSha256();
        }
    }
}