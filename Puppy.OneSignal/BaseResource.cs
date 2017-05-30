#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> BaseResource.cs </Name>
//         <Created> 30/05/2017 4:42:17 PM </Created>
//         <Key> 4c31a1b0-aa83-4769-ba89-20660a875107 </Key>
//     </File>
//     <Summary>
//         BaseResource.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using RestSharp;

namespace Puppy.OneSignal
{
    /// <summary>
    ///     Abstract class which helps easier implementation of new client resources. 
    /// </summary>
    public abstract class BaseResource
    {
        /// <summary>
        ///     Rest client reference. 
        /// </summary>
        protected RestClient RestClient { get; set; }

        /// <summary>
        ///     Your OneSignal Api key. 
        /// </summary>
        protected string ApiKey { get; set; }

        /// <summary>
        ///     Default constructor. 
        /// </summary>
        /// <param name="apiKey"> Your OneSignal API key </param>
        /// <param name="apiUri"> API uri (https://onesignal.com/api/v1/notifications) </param>
        protected BaseResource(string apiKey, string apiUri)
        {
            ApiKey = apiKey;
            RestClient = new RestClient(apiUri);
        }
    }
}