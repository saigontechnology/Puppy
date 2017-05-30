#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> OneSignalClient.cs </Name>
//         <Created> 30/05/2017 5:30:31 PM </Created>
//         <Key> 91d8a982-97d6-4b46-99a7-b9f4ca6246ce </Key>
//     </File>
//     <Summary>
//         OneSignalClient.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Puppy.OneSignal.Devices;
using Puppy.OneSignal.Notifications;

namespace Puppy.OneSignal
{
    /// <summary>
    ///     OneSignal client 
    /// </summary>
    public class OneSignalClient : IOneSignalClient
    {
        /// <summary>
        ///     Default constructor. 
        /// </summary>
        /// <param name="apiKey"> Your OneSignal API key </param>
        /// <param name="apiUri"> API uri (default is "https://onesignal.com/api/v1") </param>
        public OneSignalClient(string apiKey, string apiUri = "https://onesignal.com/api/v1")
        {
            Devices = new DevicesResource(apiKey, apiUri);
            Notifications = new NotificationsResource(apiKey, apiUri);
        }

        /// <summary>
        ///     Device resources. 
        /// </summary>
        public IDevicesResource Devices { get; }

        /// <summary>
        ///     Notification resources. 
        /// </summary>
        public INotificationsResource Notifications { get; }
    }
}