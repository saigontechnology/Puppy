#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> HttpContext.cs </Name>
//         <Created> 11/08/17 2:28:30 PM </Created>
//         <Key> 464c387f-a9b1-461f-986e-c60ca46b7dcd </Key>
//     </File>
//     <Summary>
//         HttpContext.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace Puppy.Logger.Core.Models
{
    [Serializable]
    [DesignerCategory(nameof(Puppy))]
    public sealed class HttpContextInfo : Serializable
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public DateTimeOffset? RequestTime { get; set; }

        public Dictionary<string, List<string>> Headers { get; set; } = new Dictionary<string, List<string>>();

        public string Protocol { get; set; }

        public string Method { get; set; }

        public string Endpoint { get; set; }

        public Dictionary<string, List<string>> QueryStrings { get; set; } = new Dictionary<string, List<string>>();

        /// <summary>
        ///     DisplayUrl is combine protocol, method, endpoint and query string 
        /// </summary>
        public string DisplayUrl { get; set; }

        /// <summary>
        ///     Need to <c> EnableRewind </c> for Request in middleware to get Request Body. 
        /// </summary>
        public object RequestBody { get; set; }

        public HttpContextInfo()
        {
        }

        public HttpContextInfo(HttpContext context) : this()
        {
            Headers = context.Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToList());
            Protocol = context.Request.Protocol;
            Method = context.Request.Method;
            Endpoint = $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.Path.Value}";
            QueryStrings = context.Request.Query.ToDictionary(x => x.Key, x => x.Value.ToList());
            DisplayUrl = $"[{Protocol}]({Method}){Endpoint}{context.Request.QueryString}";
            RequestBody = GetRequestBody(context);
        }

        private static object GetRequestBody(HttpContext context)
        {
            try
            {
                object requestBodyObj;

                // Reset Body to Original Position
                context.Request.Body.Position = 0;

                using (StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    string requestBody = reader.ReadToEnd();

                    // Reformat to have beautiful json string
                    requestBodyObj = JsonConvert.DeserializeObject(requestBody);
                }
                // Reset Body to Original Position
                context.Request.Body.Position = 0;

                return requestBodyObj;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}