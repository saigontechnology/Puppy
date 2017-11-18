#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ResourceBase.cs </Name>
//         <Created> 30/05/2017 4:42:17 PM </Created>
//         <Key> 4c31a1b0-aa83-4769-ba89-20660a875107 </Key>
//     </File>
//     <Summary>
//         ResourceBase.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;

namespace Puppy.OneSignal
{
    /// <summary>
    ///     Abstract class which helps easier implementation of new client resources. 
    /// </summary>
    public abstract class ResourceBase
    {
        /// <summary>
        ///     Default constructor. 
        /// </summary>
        /// <param name="apiKey"> Your OneSignal API key </param>
        /// <param name="apiUri"> API uri </param>
        protected ResourceBase(string apiKey, string apiUri = "https://onesignal.com/api/v1")
        {
            ApiKey = apiKey;

            ApiUri = apiUri;

            FlurlHttp.Configure(config =>
            {
                config.JsonSerializer = new NewtonsoftJsonSerializer(
                    new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Include
                    }
                );
            });
        }

        protected string ApiKey { get; set; }

        protected string ApiUri { get; set; }
    }
}