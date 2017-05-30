#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> DevicesResource.cs </Name>
//         <Created> 30/05/2017 4:41:17 PM </Created>
//         <Key> 06cda240-ce01-4a22-9401-4ac2f7a2639e </Key>
//     </File>
//     <Summary>
//         DevicesResource.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using RestSharp;
using System.Threading.Tasks;

namespace Puppy.OneSignal.Devices
{
    /// <summary>
    ///     Implementation of BaseResource, used to help client add or edit device. 
    /// </summary>
    public class DevicesResource : BaseResource, IDevicesResource
    {
        /// <summary>
        ///     Default constructor 
        /// </summary>
        /// <param name="apiKey"> Your OneSignal API key </param>
        /// <param name="apiUri"> API uri (https://onesignal.com/api/v1/notifications) </param>
        public DevicesResource(string apiKey, string apiUri) : base(apiKey, apiUri)
        {
        }

        /// <summary>
        ///     Adds new device into OneSignal App. 
        /// </summary>
        /// <param name="options"> Here you can specify options used to add new device. </param>
        /// <returns> Result of device add operation. </returns>
        public async Task<DeviceAddResult> Add(DeviceAddOptions options)
        {
            var restRequest = new RestRequest("players", Method.POST);

            restRequest.AddHeader("Authorization", string.Format("Basic {0}", base.ApiKey));

            restRequest.RequestFormat = DataFormat.Json;
            restRequest.JsonSerializer = new NewtonsoftJsonSerializer();
            restRequest.AddBody(options);

            var restResponse = await RestClient.ExecuteAsync<DeviceAddResult>(restRequest);

            if (restResponse.ErrorException != null)
            {
                throw restResponse.ErrorException;
            }

            return restResponse.Data;
        }

        /// <summary>
        ///     Edits existing device defined in OneSignal App. 
        /// </summary>
        /// <param name="id">      Id of the device </param>
        /// <param name="options"> Options used to modify attributes of the device. </param>
        /// <exception cref="Exception"></exception>
        public async Task Edit(string id, DeviceEditOptions options)
        {
            RestRequest restRequest = new RestRequest("players/{id}", Method.PUT);

            restRequest.AddHeader("Authorization", string.Format("Basic {0}", base.ApiKey));

            restRequest.AddUrlSegment("id", id);

            restRequest.RequestFormat = DataFormat.Json;
            restRequest.JsonSerializer = new NewtonsoftJsonSerializer();
            restRequest.AddBody(options);

            IRestResponse restResponse = await RestClient.ExecuteAsync(restRequest);

            if (restResponse.ErrorException != null)
            {
                throw restResponse.ErrorException;
            }
        }
    }
}