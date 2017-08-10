#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ServerInfoConfig.cs </Name>
//         <Created> 11/08/17 12:13:20 AM </Created>
//         <Key> 8fcc0527-e8d8-48eb-98ad-e7169450d242 </Key>
//     </File>
//     <Summary>
//         ServerInfoConfig.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;
using System;

namespace Puppy.Web.Middlewares.ServerInfo
{
    public static class ServerInfoConfig
    {
        /// <summary>
        ///     Server Name 
        /// </summary>
        public static string Name { get; set; } = "cloudflare-nginx";

        [JsonIgnore]
        public static string NameHeader { get; } = "Server";

        /// <summary>
        ///     Power By Technology 
        /// </summary>
        public static string PoweredBy { get; set; } = "PHP/5.6.30";

        [JsonIgnore]
        public static string PoweredByHeader { get; } = "X-Powered-By";

        /// <summary>
        ///     Author Full Name 
        /// </summary>
        public static string AuthorName { get; set; } = "Top Nguyen";

        [JsonIgnore]
        public static string AuthorNameHeader { get; } = "X-Author-Name";

        /// <summary>
        ///     Author of Website URL 
        /// </summary>
        public static string AuthorWebsite { get; set; } = "http://topnguyen.net";

        [JsonIgnore]
        public static string AuthorWebsiteHeader { get; } = "X-Author-Website";

        /// <summary>
        ///     Author Email 
        /// </summary>
        public static string AuthorEmail { get; set; } = "topnguyen92@gmail.com";

        [JsonIgnore]
        public static string AuthorEmailHeader { get; } = "X-Author-Email";

        /// <summary>
        ///     System Time Zone Info, Can Find Full list ID via Current Machine System 
        /// </summary>
        /// <remarks>
        ///     System store list Time Zone Info in <c> Regedit Key </c>:
        ///     "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Time Zones"
        /// </remarks>
        public static string TimeZoneId { get; set; } = "Co-ordinated Universal Time"; // "SE Asia Standard Time" for VietNam

        public static TimeZoneInfo TimeZoneInfo => TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);

        /// <summary>
        ///     Cookie Schema Name should unique in server machine between web applications 
        /// </summary>
        public static string CookieSchemaName { get; set; } = "Puppy_Cookie";
    }
}