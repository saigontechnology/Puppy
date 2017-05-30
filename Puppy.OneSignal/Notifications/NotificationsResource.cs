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

using RestSharp;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Puppy.OneSignal.Notifications
{
    /// <summary>
    ///     Class used to define resources needed for client to manage notifications. 
    /// </summary>
    public class NotificationsResource : BaseResource, INotificationsResource
    {
        /// <summary>
        ///     Default constructor 
        /// </summary>
        /// <param name="apiKey"> Your OneSignal API key </param>
        /// <param name="apiUri"> API uri (https://onesignal.com/api/v1/notifications) </param>
        public NotificationsResource(string apiKey, string apiUri) : base(apiKey, apiUri)
        {
        }

        /// <summary>
        ///     Creates new notification to be sent by OneSignal system. 
        /// </summary>
        /// <param name="options"> Options used for notification create operation. </param>
        /// <returns></returns>
        public async Task<NotificationCreateResult> Create(NotificationCreateOptions options)
        {
            var restRequest = new RestRequest("notifications", Method.POST);

            restRequest.AddHeader("Authorization", string.Format("Basic {0}", ApiKey));

            restRequest.RequestFormat = DataFormat.Json;
            restRequest.JsonSerializer = new NewtonsoftJsonSerializer();
            restRequest.AddBody(options);

            var restResponse = await RestClient.ExecuteAsync<NotificationCreateResult>(restRequest);

            if (!(restResponse.StatusCode != HttpStatusCode.Created || restResponse.StatusCode != HttpStatusCode.OK))
                if (restResponse.ErrorException != null)
                    throw restResponse.ErrorException;
                else if (restResponse.StatusCode != HttpStatusCode.OK && restResponse.Content != null)
                    throw new Exception(restResponse.Content);

            return restResponse.Data;
        }

        /// <summary>
        ///     Get delivery and convert report about single notification. 
        /// </summary>
        /// <param name="options">
        ///     Options used for getting delivery and convert report about single notification.
        /// </param>
        /// <returns></returns>
        public async Task<NotificationViewResult> ViewAsync(NotificationViewOptions options)
        {
            var baseRequestPath = "notifications/{0}?app_id={1}";

            var restRequest = new RestRequest(string.Format(baseRequestPath, options.Id, options.AppId), Method.GET);

            restRequest.AddHeader("Authorization", string.Format("Basic {0}", ApiKey));

            restRequest.RequestFormat = DataFormat.Json;
            restRequest.JsonSerializer = new NewtonsoftJsonSerializer();

            var restResponse = await RestClient.ExecuteAsync<NotificationViewResult>(restRequest);

            if (!(restResponse.StatusCode != HttpStatusCode.Created || restResponse.StatusCode != HttpStatusCode.OK))
                if (restResponse.ErrorException != null)
                    throw restResponse.ErrorException;
                else if (restResponse.StatusCode != HttpStatusCode.OK && restResponse.Content != null)
                    throw new Exception(restResponse.Content);

            return restResponse.Data;
        }

        /// <summary>
        ///     Cancel a notification scheduled by the OneSignal system 
        /// </summary>
        /// <param name="options"> Options used for notification cancel operation. </param>
        /// <returns></returns>
        public async Task<NotificationCancelResult> Cancel(NotificationCancelOptions options)
        {
            var restRequest = new RestRequest("notifications/" + options.Id, Method.DELETE);

            restRequest.AddHeader("Authorization", string.Format("Basic {0}", ApiKey));

            restRequest.AddParameter("app_id", options.AppId);

            restRequest.RequestFormat = DataFormat.Json;

            var restResponse = await RestClient.ExecuteAsync<NotificationCancelResult>(restRequest);

            if (restResponse.ErrorException != null)
                throw restResponse.ErrorException;
            if (restResponse.StatusCode != HttpStatusCode.OK && restResponse.Content != null)
                throw new Exception(restResponse.Content);

            return restResponse.Data;
        }
    }
}