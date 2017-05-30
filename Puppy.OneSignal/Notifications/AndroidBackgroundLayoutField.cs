#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> AndroidBackgroundLayoutField.cs </Name>
//         <Created> 30/05/2017 4:48:19 PM </Created>
//         <Key> adbb3f82-0f53-479b-a09d-379ba66ea4b0 </Key>
//     </File>
//     <Summary>
//         AndroidBackgroundLayoutField.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Newtonsoft.Json;

namespace Puppy.OneSignal.Notifications
{
    /// <summary>
    ///     Class used to describe android background layout of the notification message displayed in device.
    /// </summary>
    public class AndroidBackgroundLayoutField
    {
        /// <summary>
        ///     Background image. 
        /// </summary>
        [JsonProperty("image")]
        public string Image { get; set; }

        /// <summary>
        ///     Background heading color. 
        /// </summary>
        [JsonProperty("headings_color")]
        public string HeadingsColor { get; set; }

        /// <summary>
        ///     Background content color. 
        /// </summary>
        [JsonProperty("contents_color")]
        public string ContentsColor { get; set; }
    }
}