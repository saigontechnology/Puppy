#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> NotificationsResource.cs </Name>
//         <Created> 30/05/2017 4:53:34 PM </Created>
//         <Key> b5d0841c-7c97-4282-adfb-a40d37eb100c </Key>
//     </File>
//     <Summary>
//         NotificationsResource.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Flurl;
using Flurl.Http;
using System.Threading.Tasks;

namespace Puppy.OneSignal.Notifications
{
    /// <summary>
    ///     Class used to define resources needed for client to manage notifications. 
    /// </summary>
    public class NotificationsResource : ResourceBase, INotificationsResource
    {
        /// <summary>
        ///     Default constructor 
        /// </summary>
        /// <param name="apiKey"> Your OneSignal API key </param>
        /// <param name="apiUri"> API uri (https://onesignal.com/api/v1) </param>
        public NotificationsResource(string apiKey, string apiUri = "https://onesignal.com/api/v1") : base(apiKey, apiUri)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Creates new notification to be sent by OneSignal system. 
        /// </summary>
        /// <param name="options"> Options used for notification create operation. </param>
        /// <returns></returns>
        public async Task<NotificationCreateResult> CreateAsync(NotificationCreateOptions options)
        {
            var result =
                await ApiUri.AppendPathSegment("notifications")
                    .WithHeader("Authorization", $"Basic {ApiKey}")
                    .PostJsonAsync(options)
                    .ReceiveJson<NotificationCreateResult>()
                    .ConfigureAwait(true);

            return result;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Get delivery and convert report about single notification. 
        /// </summary>
        /// <param name="options">
        ///     Options used for getting delivery and convert report about single notification.
        /// </param>
        /// <returns></returns>
        public async Task<NotificationViewResult> ViewAsync(NotificationViewOptions options)
        {
            var result =
                await ApiUri.AppendPathSegment($"notifications/{options.Id}?app_id={options.AppId}")
                    .WithHeader("Authorization", $"Basic {ApiKey}")
                    .GetAsync()
                    .ReceiveJson<NotificationViewResult>()
                    .ConfigureAwait(true);

            return result;
        }

        /// <summary>
        ///     Cancel a notification scheduled by the OneSignal system 
        /// </summary>
        /// <param name="options"> Options used for notification cancel operation. </param>
        /// <returns></returns>
        public async Task<NotificationCancelResult> CancelAsync(NotificationCancelOptions options)
        {
            var result =
                await ApiUri.AppendPathSegment($"notifications/{options.Id}?app_id={options.AppId}")
                    .WithHeader("Authorization", $"Basic {ApiKey}")
                    .DeleteAsync()
                    .ReceiveJson<NotificationCancelResult>()
                    .ConfigureAwait(true);

            return result;
        }
    }
}