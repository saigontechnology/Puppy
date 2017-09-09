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
using Puppy.Core.LinqUtils;
using Puppy.Core.Models;
using Puppy.Core.StringUtils;
using Puppy.Web.Constants;
using System;
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
            MarkerFullInfo = GetMarkerFullInfo(request);
            MarkerName = GetMarkerName(request);
            MarkerVersion = GetMarkerVersion(request);

            // OS
            OsFullInfo = GetOsFullInfo(request);
            OsName = GetOsName(request);
            OsVersion = GetOsVersion(request);

            // Engine
            EngineFullInfo = GetEngineFullInfo(request);
            EngineName = GetEngineName(request);
            EngineVersion = GetEngineVersion(request);

            // Browser
            BrowserFullInfo = GetBrowserFullInfo(request);
            BrowserName = GetBrowserName(request);
            BrowserVersion = GetBrowserVersion(request);

            // Others
            IpAddress = GetIpAddress(request);
            UserAgent = GetUserAgent(request);

            DeviceHash = GetDeviceHash(this);
        }

        private static DeviceType GetDeviceType(HttpRequest request)
        {
            var agent = GetUserAgent(request, true);
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
            var agent = GetUserAgent(request, true);
            if (string.IsNullOrWhiteSpace(agent)) return false;

            if (Regex.IsMatch(agent, HttpDetectionConstants.CrawlerAgentsRegex, RegexOptions.IgnoreCase))
                return true;

            return false;
        }

        // Marker
        private static string GetMarkerFullInfo(HttpRequest request)
        {
            var agent = GetUserAgent(request);
            if (string.IsNullOrWhiteSpace(agent)) return null;

            int iEnd = agent.IndexOf('(');
            string markerFullInfo = agent.Substring(0, iEnd)?.Trim();
            return markerFullInfo;
        }

        private static string GetMarkerName(HttpRequest request)
        {
            string markerName = GetMarkerFullInfo(request)?.Split('/').FirstOrDefault()?.Trim();
            return markerName;
        }

        private static string GetMarkerVersion(HttpRequest request)
        {
            string markerVersion = GetMarkerFullInfo(request)?.Split('/').LastOrDefault()?.Trim();
            return markerVersion;
        }

        // OS
        private static string GetOsFullInfo(HttpRequest request)
        {
            var agent = GetUserAgent(request);
            if (string.IsNullOrWhiteSpace(agent)) return null;

            int iStart = agent.IndexOf('(') + 1;
            int iEnd = agent.IndexOf(')') - iStart;
            string osFullInfo = agent.Substring(iStart, iEnd)?.Trim();
            return osFullInfo;
        }

        private static string GetOsName(HttpRequest request)
        {
            string osName = GetOsFullInfo(request)?.Split(';').FirstOrDefault()?.Trim();
            return osName;
        }

        private static string GetOsVersion(HttpRequest request)
        {
            var infos = GetOsFullInfo(request)?.Split(';');

            string osVersion = null;

            if (infos != null && infos.Length > 0)
            {
                int i = 1;
                while (i <= infos.Length && (osVersion == null || osVersion.ToLowerInvariant() == "u"))
                {
                    osVersion = infos[i];
                    i++;
                }
            }

            return osVersion;
        }

        // Engine
        private static string GetEngineFullInfo(HttpRequest request)
        {
            var agent = GetUserAgent(request);
            if (string.IsNullOrWhiteSpace(agent)) return null;

            int iStart = agent.IndexOf(')') + 1;
            string engineFullInfo = agent.Substring(iStart)?.Trim();

            if (string.IsNullOrWhiteSpace(engineFullInfo)) return null;

            int iEnd = engineFullInfo.IndexOf(' ');
            engineFullInfo = engineFullInfo.Substring(0, iEnd);
            return engineFullInfo;
        }

        private static string GetEngineName(HttpRequest request)
        {
            string engineName = GetEngineFullInfo(request)?.Split('/').FirstOrDefault()?.Trim();

            // Standardize engine name
            const string webKitStandardName = "WebKit";
            engineName = engineName?.EndsWith(webKitStandardName) == true ? webKitStandardName : engineName;

            return engineName;
        }

        private static string GetEngineVersion(HttpRequest request)
        {
            string engineName = GetEngineFullInfo(request)?.Split('/').LastOrDefault()?.Trim();
            return engineName;
        }

        // Browser

        /// <summary>
        ///     Get browser info in user agent 
        /// </summary>
        /// <param name="request">      </param>
        /// <param name="isStandardize"> will exclude "version", "mobile" from browser info </param>
        /// <returns></returns>
        private static string GetBrowserFullInfo(HttpRequest request, bool isStandardize = true)
        {
            var agent = GetUserAgent(request);
            if (string.IsNullOrWhiteSpace(agent)) return null;

            int iStart = agent.LastIndexOf(')') + 1;
            string browserFullInfo = agent.Substring(iStart)?.Trim();

            // Filters
            if (!string.IsNullOrWhiteSpace(browserFullInfo))
            {
                var listExclude = new[] { "version", "mobile" };

                var browserInfos = browserFullInfo.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                if (isStandardize)
                {
                    browserInfos = browserInfos.RemoveWhere(x => listExclude.Any(y => x.ToLowerInvariant().Contains(y))).ToList();
                }

                browserFullInfo = string.Join(", ", browserInfos);
            }

            return browserFullInfo;
        }

        private static string GetBrowserName(HttpRequest request)
        {
            string browserName = GetBrowserFullInfo(request)?.Split(',').FirstOrDefault()?.Split('/').FirstOrDefault()?.Trim();
            return browserName;
        }

        private static string GetBrowserVersion(HttpRequest request)
        {
            string browserFullInfo = GetBrowserFullInfo(request, false);

            // Filters
            if (string.IsNullOrWhiteSpace(browserFullInfo)) return null;

            var browserInfos = browserFullInfo.Split(' ').ToList();
            var browserVersion = browserInfos.FirstOrDefault(x => x.ToLowerInvariant().Contains("version"))?.Split('/').LastOrDefault()?.Trim().Trim(',');

            if (string.IsNullOrWhiteSpace(browserVersion))
            {
                browserVersion = GetBrowserFullInfo(request)?.Split(',').FirstOrDefault()?.Split('/').LastOrDefault()?.Trim();
            }

            return browserVersion;
        }

        // Other
        private static string GetIpAddress(HttpRequest request)
        {
            var result = string.Empty;

            if (request.Headers?.Any() == true)
            {
                // look for the X-Forwarded-For (XFF) HTTP header field it's used for identifying the
                // originating IP address of a client connecting to a web server through an HTTP
                // proxy or load balancer.
                string xff = request.Headers
                    .Where(x => HeaderKey.XForwardedFor.Equals(x.Value, StringComparison.OrdinalIgnoreCase))
                    .Select(k => request.Headers[k.Key])
                    .FirstOrDefault();

                // if you want to exclude private IP addresses, then see http://stackoverflow.com/questions/2577496/how-can-i-get-the-clients-ip-address-in-asp-net-mvc
                if (!string.IsNullOrWhiteSpace(xff))
                {
                    var lastIp = xff.Split(',').FirstOrDefault();
                    result = lastIp;
                }
            }

            if (string.IsNullOrWhiteSpace(result))
            {
                result = request.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            }

            if (string.IsNullOrWhiteSpace(result))
            {
                return null;
            }

            // Standardize
            if (result == "::1")
            {
                result = "127.0.0.1";
            }

            // Remove port
            int index = result.IndexOf(":", StringComparison.OrdinalIgnoreCase);

            if (index > 0)
            {
                result = result.Substring(0, index);
            }

            return result;
        }

        private static string GetUserAgent(HttpRequest request, bool isLowerCase = false)
        {
            var agent = request?.Headers[HeaderKey.UserAgent].ToString();

            if (!string.IsNullOrWhiteSpace(agent) && isLowerCase)
            {
                agent = agent.ToLowerInvariant();
            }

            return agent;
        }

        private static string GetDeviceHash(DeviceModel deviceModel)
        {
            string ipAddress = string.IsNullOrWhiteSpace(deviceModel.IpAddress) ? StringHelper.GetRandomString(16) : deviceModel.IpAddress;
            string identityDevice = $"{deviceModel.OsName}|{deviceModel.OsVersion}_{deviceModel.EngineName}|{deviceModel.EngineVersion}_{deviceModel.BrowserName}|{deviceModel.BrowserVersion}_{ipAddress}";
            return identityDevice.GetSha256();
        }
    }
}