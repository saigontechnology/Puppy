#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy → Interface </Project>
//     <File>
//         <Name> INotificationsResource.cs </Name>
//         <Created> 30/05/2017 4:50:43 PM </Created>
//         <Key> 3b770678-424a-46b0-bdcb-cfc95ee93f4a </Key>
//     </File>
//     <Summary>
//         INotificationsResource.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System.Threading.Tasks;

namespace Puppy.OneSignal.Notifications
{
    /// <summary>
    ///     Interface used to unify Notification Resource classes. 
    /// </summary>
    public interface INotificationsResource
    {
        /// <summary>
        ///     Creates a new notification. 
        /// </summary>
        /// <param name="options">
        ///     This parameter can contain variety of possible options used to create notification.
        /// </param>
        /// <returns> Returns result of notification create operation. </returns>
        Task<NotificationCreateResult> CreateAsync(NotificationCreateOptions options);

        /// <summary>
        ///     Cancel notification Stop a scheduled or currently outgoing notification 
        /// </summary>
        /// <param name="options">
        ///     This parameter contains the information required to cancel a scheduled notification
        /// </param>
        /// <returns> Returns result of notification cancel operation. </returns>
        Task<NotificationCancelResult> CancelAsync(NotificationCancelOptions options);

        /// <summary>
        ///     Get report about notification 
        /// </summary>
        /// <param name="options">
        ///     This parameter can contain variety of possible options used to create notification.
        /// </param>
        /// <returns> Returns result of notification create operation. </returns>
        Task<NotificationViewResult> ViewAsync(NotificationViewOptions options);
    }
}