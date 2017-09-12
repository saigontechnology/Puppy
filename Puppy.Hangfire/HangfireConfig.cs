#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey </Project>
//     <File>
//         <Name> HangfireConfig.cs </Name>
//         <Created> 17/07/17 11:03:19 PM </Created>
//         <Key> f148fc57-541b-428a-a489-c909d8c2dca3 </Key>
//     </File>
//     <Summary>
//         HangfireConfig.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

namespace Puppy.Hangfire
{
    public static class HangfireConfig
    {
        /// <summary>
        ///     Hangfire Dashboard Url. Ex: /developers/job, if this is <c> empty </c> then disable dashboard
        /// </summary>
        /// <remarks> Start with <c> "/" </c> but end with <c> empty </c>, default is "/developers/job" </remarks>
        public static string DashboardUrl { get; set; } = "/developers/job";

        /// <summary>
        ///     Access Key read from URI 
        /// </summary>
        /// <remarks> Empty is allow <c> Anonymous </c> </remarks>
        public static string AccessKey { get; set; } = string.Empty;

        /// <summary>
        ///     Query parameter via http request 
        /// </summary>
        /// <remarks> Empty is allow <c> Anonymous </c> </remarks>
        public static string AccessKeyQueryParam { get; set; } = "key";

        /// <summary>
        ///     Un-authorize message when user access api document with not correct key. Default is
        ///     "You don't have permission to Job Dashboard, please contact your administrator."
        /// </summary>
        public static string UnAuthorizeMessage { get; set; } = "You don't have permission to Job Dashboard, please contact your administrator.";

        /// <summary>
        ///     The path for the Back To Site link. Set to <see langword="null" /> in order to hide
        ///     the Back To Site link. Default is "/"
        /// </summary>
        public static string BackToSiteUrl { get; set; } = "/";

        /// <summary>
        ///     The interval the /stats endpoint should be polled with (milliseconds). Default is 2000
        /// </summary>
        public static int StatsPollingInterval { get; set; } = 2000;
    }
}