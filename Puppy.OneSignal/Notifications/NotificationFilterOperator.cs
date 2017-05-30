#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> NotificationFilterOperator.cs </Name>
//         <Created> 30/05/2017 4:53:19 PM </Created>
//         <Key> 90e4d60f-fc53-4688-a1d9-5afeb57c8b7c </Key>
//     </File>
//     <Summary>
//         NotificationFilterOperator.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Newtonsoft.Json;

namespace Puppy.OneSignal.Notifications
{
    /// <summary>
    ///     Notification filter operator is used to define logical AND, OR 
    /// </summary>
    public class NotificationFilterOperator : INotificationFilter
    {
        /// <summary>
        ///     Can be AND or OR operator 
        /// </summary>
        [JsonProperty("operator")]
        public string Operator { get; set; }
    }
}