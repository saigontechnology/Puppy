#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> HttpDetectionConstants.cs </Name>
//         <Created> 02/09/17 11:47:18 PM </Created>
//         <Key> 93f65b11-38d9-4307-b521-b912d85328f1 </Key>
//     </File>
//     <Summary>
//         HttpDetectionConstants.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

namespace Puppy.Web.HttpRequestDetection
{
    public static class HttpDetectionConstants
    {
        public static readonly string[] TabletAgents = {
            "tablet",
            "ipad",
            "playbook",
            "hp-tablet",
            "kindle"
        };

        public static readonly string[] CrawlerAgents = {
            "bot",
            "slurp",
            "spider"
        };

        public static readonly string[] MobileAgents = {
            "blackberry",
            "webos",
            "ipod",
            "lge vx",
            "midp",
            "maemo",
            "mmp",
            "mobile",
            "netfront",
            "hiptop",
            "nintendo DS",
            "novarra",
            "openweb",
            "opera mobi",
            "opera mini",
            "palm",
            "psp",
            "phone",
            "smartphone",
            "symbian",
            "up.browser",
            "up.link",
            "wap",
            "windows ce"
        };

        /// <summary>
        ///     Reference 4 chare from http://www.webcab.de/wapua.htm 
        /// </summary>
        public static readonly string[] MobileAgentPrefix = {
            "w3c ",
            "w3c-",
            "acs-",
            "alav",
            "alca",
            "amoi",
            "audi",
            "avan",
            "benq",
            "bird",
            "blac",
            "blaz",
            "brew",
            "cell",
            "cldc",
            "cmd-",
            "dang",
            "doco",
            "eric",
            "hipt",
            "htc_",
            "inno",
            "ipaq",
            "ipod",
            "jigs",
            "kddi",
            "keji",
            "leno",
            "lg-c",
            "lg-d",
            "lg-g",
            "lge-",
            "lg/u",
            "maui",
            "maxo",
            "midp",
            "mits",
            "mmef",
            "mobi",
            "mot-",
            "moto",
            "mwbp",
            "nec-",
            "newt",
            "noki",
            "palm",
            "pana",
            "pant",
            "phil",
            "play",
            "port",
            "prox",
            "qwap",
            "sage",
            "sams",
            "sany",
            "sch-",
            "sec-",
            "send",
            "seri",
            "sgh-",
            "shar",
            "sie-",
            "siem",
            "smal",
            "smar",
            "sony",
            "sph-",
            "symb",
            "t-mo",
            "teli",
            "tim-",
            "tosh",
            "tsm-",
            "upg1",
            "upsi",
            "vk-v",
            "voda",
            "wap-",
            "wapa",
            "wapi",
            "wapp",
            "wapr",
            "webc",
            "winw",
            "winw",
            "xda ",
            "xda-"
        };
    }
}