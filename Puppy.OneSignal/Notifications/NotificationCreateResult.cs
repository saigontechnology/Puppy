#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> NotificationCreateResult.cs </Name>
//         <Created> 30/05/2017 4:52:14 PM </Created>
//         <Key> 82e47d17-d84a-40a2-99dd-c92aa1aee0a6 </Key>
//     </File>
//     <Summary>
//         NotificationCreateResult.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using RestSharp.Deserializers;

namespace Puppy.OneSignal.Notifications
{
    /// <summary>
    ///     Result of notification create operation. 
    /// </summary>
    public class NotificationCreateResult
    {
        /// <summary>
        ///     Returns the number of recepients who received the message. 
        /// </summary>
        [DeserializeAs(Name = "recipients")]
        public int Recipients { get; set; }

        /// <summary>
        ///     Returns the id of the result. 
        /// </summary>
        [DeserializeAs(Name = "id")]
        public string Id { get; set; }
    }
}