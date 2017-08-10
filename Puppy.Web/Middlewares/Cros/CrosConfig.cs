#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey </Project>
//     <File>
//         <Name> CrosConfig.cs </Name>
//         <Created> 18/07/17 11:59:55 AM </Created>
//         <Key> 6fa8500b-a4b1-41da-adee-d2af676d3206 </Key>
//     </File>
//     <Summary>
//         CrosConfig.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

namespace Puppy.Web.Middlewares.Cros
{
    public static class CrosConfig
    {
        public static string PolicyAllowAllName { get; set; } = "PolicyAllowAll";
        public static string AccessControlAllowOrigin { get; set; } = "*";
        public static string AccessControlAllowHeaders { get; set; } = "Authorization, Content-Type";
        public static string AccessControlAllowMethods { get; set; } = "GET, POST, PUT, DELETE, OPTIONS, HEAD";
    }
}