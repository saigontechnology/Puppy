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

namespace Puppy.Web.Middlewares.ServerInfo
{
    public static class ServerInfoConfig
    {
        /// <summary>
        ///     Server Name 
        /// </summary>
        public static string Name { get; set; } = "cloudflare-nginx";

        /// <summary>
        ///     Power By Technology 
        /// </summary>
        public static string PoweredBy { get; set; } = "PHP/5.6.30";

        /// <summary>
        ///     Author Full Name 
        /// </summary>
        public static string AuthorName { get; set; } = "Top Nguyen";

        /// <summary>
        ///     Author of Website URL 
        /// </summary>
        public static string AuthorWebsite { get; set; } = "http://topnguyen.net";

        /// <summary>
        ///     Author Email 
        /// </summary>
        public static string AuthorEmail { get; set; } = "topnguyen92@gmail.com";
    }
}