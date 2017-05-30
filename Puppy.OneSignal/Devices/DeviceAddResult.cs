#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> DeviceAddResult.cs </Name>
//         <Created> 30/05/2017 4:40:41 PM </Created>
//         <Key> a598fc6a-680b-446e-938e-7ae8ba5de6b7 </Key>
//     </File>
//     <Summary>
//         DeviceAddResult.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using RestSharp.Deserializers;

namespace Puppy.OneSignal.Devices
{
    /// <summary>
    ///     Class used to keep result of device add operation. 
    /// </summary>
    public class DeviceAddResult
    {
        /// <summary>
        ///     Returns true if operation is successfull. 
        /// </summary>
        [DeserializeAs(Name = "success")]
        public bool IsSuccess { get; set; }

        /// <summary>
        ///     Returns id of the result operation. 
        /// </summary>
        [DeserializeAs(Name = "id")]
        public string Id { get; set; }
    }
}