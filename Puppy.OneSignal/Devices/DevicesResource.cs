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

using Flurl;
using Flurl.Http;
using System.Threading.Tasks;

namespace Puppy.OneSignal.Devices
{
    /// <summary>
    ///     Implementation of ResourceBase, used to help client add or edit device. 
    /// </summary>
    public class DevicesResource : ResourceBase, IDevicesResource
    {
        /// <summary>
        ///     Default constructor 
        /// </summary>
        /// <param name="apiKey"> Your OneSignal API key </param>
        /// <param name="apiUri"> API uri (https://onesignal.com/api/v1) </param>
        public DevicesResource(string apiKey, string apiUri = "https://onesignal.com/api/v1") : base(apiKey, apiUri)
        {
        }

        /// <summary>
        ///     Adds new device into OneSignal App. 
        /// </summary>
        /// <param name="options"> Here you can specify options used to add new device. </param>
        /// <returns> Result of device add operation. </returns>
        public async Task<DeviceAddResult> AddAsync(DeviceAddOptions options)
        {
            var result =
                await ApiUri.AppendPathSegment("players")
                    .WithHeader("Authorization", $"Basic {ApiKey}")
                    .PostJsonAsync(options)
                    .ReceiveJson<DeviceAddResult>()
                    .ConfigureAwait(true);

            return result;
        }

        /// <summary>
        ///     Edits existing device defined in OneSignal App. 
        /// </summary>
        /// <param name="id">      Id of the device </param>
        /// <param name="options"> Options used to modify attributes of the device. </param>
        /// <exception cref="Exception"></exception>
        public async Task EditAsync(string id, DeviceEditOptions options)
        {
            var result =
                await ApiUri.AppendPathSegment($"players/{id}")
                    .WithHeader("Authorization", $"Basic {ApiKey}")
                    .PutJsonAsync(options)
                    .ConfigureAwait(true);
        }
    }
}