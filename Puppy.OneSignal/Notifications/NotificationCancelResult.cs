#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> NotificationCancelResult.cs </Name>
//         <Created> 30/05/2017 4:51:20 PM </Created>
//         <Key> 17bbb3e6-10fb-45a5-a933-d80a71d63c9e </Key>
//     </File>
//     <Summary>
//         NotificationCancelResult.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Newtonsoft.Json;

namespace Puppy.OneSignal.Notifications
{
    /// <summary>
    ///     Result of notification cancel operation. 
    /// </summary>
    public class NotificationCancelResult
    {
        /// <summary>
        ///     Returns whether the message was canceled or not {'success': "true"} 
        /// </summary>
        [JsonProperty(PropertyName = "success")]
        public string Success { get; set; }
    }
}