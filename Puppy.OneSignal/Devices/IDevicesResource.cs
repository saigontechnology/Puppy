#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy → Interface </Project>
//     <File>
//         <Name> IDevicesResource.cs </Name>
//         <Created> 30/05/2017 4:39:56 PM </Created>
//         <Key> d02eb53b-5065-40e7-9746-b1e10c4eedc0 </Key>
//     </File>
//     <Summary>
//         IDevicesResource.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;
using System.Threading.Tasks;

namespace Puppy.OneSignal.Devices
{
    /// <summary>
    ///     Interface used to unify creation of classes used to help client add or edit device. 
    /// </summary>
    public interface IDevicesResource
    {
        /// <summary>
        ///     Adds new device into OneSignal App. 
        /// </summary>
        /// <param name="options"> Here you can specify options used to add new device. </param>
        /// <returns> Result of device add operation. </returns>
        Task<DeviceAddResult> AddAsync(DeviceAddOptions options);

        /// <summary>
        ///     Edits existing device defined in OneSignal App. 
        /// </summary>
        /// <param name="id">      Id of the device </param>
        /// <param name="options"> Options used to modify attributes of the device. </param>
        /// <exception cref="Exception"></exception>
        Task EditAsync(string id, DeviceEditOptions options);

        /// <summary>
        ///     Get device info 
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="appId">   </param>
        /// <returns></returns>
        Task<DeviceInfo> GetAsync(string playerId, string appId);
    }
}