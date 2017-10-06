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

namespace Puppy.Web.HttpUtils.HttpDetection
{
    public static class HttpDetectionConstants
    {
        public static readonly string TabletAgentsRegex = "/(tablet|ipad|playbook|hp-tablet|kindle|silk)|(android(?!.*mobile))/";
        public static readonly string CrawlerAgentsRegex = "/bot|slurp|spider/";
        public static readonly string MobileAgentsRegex = "/Mobile|iP(hone|od|ad)|Android|BlackBerry|IEMobile|Kindle|NetFront|Silk-Accelerated|(hpw|web)OS|Fennec|Minimo|Opera M(obi|ini)|Blazer|Dolfin|Dolphin|Skyfire|Zune/";
    }
}