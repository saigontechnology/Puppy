#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ActionButtonField.cs </Name>
//         <Created> 30/05/2017 4:44:39 PM </Created>
//         <Key> 0601a760-5797-48a5-9d3f-cc37fc37df0b </Key>
//     </File>
//     <Summary>
//         ActionButtonField.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Newtonsoft.Json;

namespace Puppy.OneSignal.Notifications
{
    /// <summary>
    ///     Class used to describe action field. 
    /// </summary>
    public class ActionButtonField
    {
        /// <summary>
        ///     Action button ID. 
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        ///     Action button text. 
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        ///     Action button icon. 
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }
    }
}