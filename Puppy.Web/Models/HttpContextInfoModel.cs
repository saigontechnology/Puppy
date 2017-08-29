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
using Puppy.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puppy.Web.Models
{
    [Serializable]
    public sealed class HttpContextInfoModel : SerializableModel
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

        public HttpContextInfoModel()
        {
        }

        public HttpContextInfoModel(HttpContext context) : this()
        {
            if (context == null) return;

            Headers = context.Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToList());
            Protocol = context.Request.Protocol;
            Method = context.Request.Method;
            Endpoint = $"{context.Request.Scheme}://{context.Request.Host.Value}{context.Request.Path.Value}";
            QueryStrings = context.Request.Query.ToDictionary(x => x.Key, x => x.Value.ToList());
            DisplayUrl = $"[{Protocol}]({Method}){Endpoint}{context.Request.QueryString}";
            RequestBody = context.Request.GetBody();
        }
    }
}