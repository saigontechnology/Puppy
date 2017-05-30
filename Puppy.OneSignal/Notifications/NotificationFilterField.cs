#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> NotificationFilterField.cs </Name>
//         <Created> 30/05/2017 4:52:52 PM </Created>
//         <Key> 92b188a0-dece-496d-b70a-5ae9063d2300 </Key>
//     </File>
//     <Summary>
//         NotificationFilterField.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Newtonsoft.Json;

namespace Puppy.OneSignal.Notifications
{
    /// <summary>
    ///     Complex type used to describe filter field. 
    /// </summary>
    public class NotificationFilterField : INotificationFilter
    {
        /// <summary>
        ///     The type of the filter field. 
        /// </summary>
        [JsonProperty("field")]
        [JsonConverter(typeof(NotificationFilterFieldTypeConverter))]
        public NotificationFilterFieldTypeEnum Field { get; set; }

        /// <summary>
        ///     The key used for comparison. 
        /// </summary>
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <summary>
        ///     The relation between key and value. 
        /// </summary>
        [JsonProperty("relation")]
        public string Relation { get; set; }

        /// <summary>
        ///     The value. 
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}