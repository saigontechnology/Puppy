#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy → Interface </Project>
//     <File>
//         <Name> IOneSignalClient.cs </Name>
//         <Created> 30/05/2017 4:36:44 PM </Created>
//         <Key> 30163e3d-5971-4b04-b5a9-ca52ee39e27c </Key>
//     </File>
//     <Summary>
//         IOneSignalClient.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Puppy.OneSignal.Devices;
using Puppy.OneSignal.Notifications;

namespace Puppy.OneSignal
{
    /// <summary>
    ///     OneSignal client interface. 
    /// </summary>
    public interface IOneSignalClient
    {
        /// <summary>
        ///     Device resources. 
        /// </summary>
        IDevicesResource Devices { get; }

        /// <summary>
        ///     Notification resources. 
        /// </summary>
        INotificationsResource Notifications { get; }
    }
}