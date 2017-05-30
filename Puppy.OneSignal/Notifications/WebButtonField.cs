#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> WebButtonField.cs </Name>
//         <Created> 30/05/2017 4:54:34 PM </Created>
//         <Key> dc5ed01e-228d-4da4-87a9-3792e6d3f92d </Key>
//     </File>
//     <Summary>
//         WebButtonField.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;

namespace Puppy.OneSignal.Notifications
{
    /// <summary>
    ///     Class used to describe web button. 
    /// </summary>
    public class WebButtonField
    {
        /// <summary>
        ///     Web button ID. This is required field. 
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        ///     Web button text. 
        /// </summary>
        [JsonProperty("text")]
        public string Text { get; set; }

        /// <summary>
        ///     Web button icon. 
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; set; }

        /// <summary>
        ///     Web button url. 
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}