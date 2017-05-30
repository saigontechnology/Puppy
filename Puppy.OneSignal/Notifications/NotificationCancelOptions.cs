#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> NotificationCancelOptions.cs </Name>
//         <Created> 30/05/2017 4:51:01 PM </Created>
//         <Key> 03a0f0a8-23de-4ab3-8884-9c32e0ebba55 </Key>
//     </File>
//     <Summary>
//         NotificationCancelOptions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;

namespace Puppy.OneSignal.Notifications
{
    /// <summary>
    ///     API Documentation: https://documentation.onesignal.com/docs/notifications-cancel-notification 
    /// </summary>
    public class NotificationCancelOptions
    {
        /// <summary>
        ///     id String Required Notification id 
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        ///     app_id String Required App id 
        /// </summary>
        [JsonProperty("app_id")]
        public string AppId { get; set; }
    }
}