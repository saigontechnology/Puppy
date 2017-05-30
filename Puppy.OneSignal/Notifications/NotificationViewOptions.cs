#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> NotificationViewOptions.cs </Name>
//         <Created> 30/05/2017 4:53:57 PM </Created>
//         <Key> 286720a5-7195-443b-aa33-99503fac3faf </Key>
//     </File>
//     <Summary>
//         NotificationViewOptions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Newtonsoft.Json;
using System;

namespace Puppy.OneSignal.Notifications
{
    /// <summary>
    ///     Get delivery and convert report about single notification. See
    ///     <see cref="!:https://documentation.onesignal.com/reference#view-notification"> API
    ///     Documentation </see> for more info.
    /// </summary>
    public class NotificationViewOptions
    {
        /// <summary>
        ///     <br /> Your OneSignal application ID, which can be found on our dashboard at
        ///     onesignal.com under App Settings &gt; Keys &amp; IDs. <br /> It is a UUID and looks
        ///     similar to 8250eaf6-1a58-489e-b136-7c74a864b434. <br />
        /// </summary>
        [JsonProperty("app_id")]
        public Guid AppId { get; set; }

        /// <summary>
        ///     <br /> Notification ID 
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }
    }
}