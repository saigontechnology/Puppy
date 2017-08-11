#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> HttpContextInfo.cs </Name>
//         <Created> 11/08/17 2:28:30 PM </Created>
//         <Key> 464c387f-a9b1-461f-986e-c60ca46b7dcd </Key>
//     </File>
//     <Summary>
//         HttpContextInfo.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Puppy.Logger.Core.Models
{
    [Serializable]
    [DesignerCategory(nameof(Puppy))]
    public class HttpContextInfo : Serializable
    {
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

        public HttpContextInfo()
        {
        }

        public HttpContextInfo(HttpContext httpContext) : this()
        {
            Headers = httpContext.Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToList());
            Protocol = httpContext.Request.Protocol;
            Method = httpContext.Request.Method;
            Endpoint = $"{httpContext.Request.Scheme}://{httpContext.Request.Host.Value}{httpContext.Request.Path.Value}";
            QueryStrings = httpContext.Request.Query.ToDictionary(x => x.Key, x => x.Value.ToList());
            DisplayUrl = $"[{Protocol}]({Method}){Endpoint}{httpContext.Request.QueryString}";
        }
    }
}