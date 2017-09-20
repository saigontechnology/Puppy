#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> HttpRequestExtensions.cs </Name>
//         <Created> 07/06/2017 9:54:01 PM </Created>
//         <Key> 190c8bb6-717f-4e86-a709-cc40e5329494 </Key>
//     </File>
//     <Summary>
//         HttpRequestExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Puppy.Web.Constants;
using Puppy.Web.HttpRequestDetection.Device;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Puppy.Web
{
    /// <summary>
    ///     <see cref="HttpRequest" /> extension methods. 
    /// </summary>
    public static class HttpRequestExtensions
    {
        /// <summary>
        ///     Determines whether the specified HTTP request is an AJAX request. 
        /// </summary>
        /// <param name="request"> The HTTP request. </param>
        /// <returns>
        ///     <c> true </c> if the specified HTTP request is an AJAX request; otherwise, <c> false </c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="request" /> parameter is <c> null </c>.
        /// </exception>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Headers != null)
                return request.Headers[HeaderKey.XRequestedWith] == "XmlHttpRequest";

            return false;
        }

        /// <summary>
        ///     Determines whether the specified HTTP request is a local request where the IP address
        ///     of the request originator was 127.0.0.1.
        /// </summary>
        /// <param name="request"> The HTTP request. </param>
        /// <returns>
        ///     <c> true </c> if the specified HTTP request is a local request; otherwise, <c> false </c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="request" /> parameter is <c> null </c>.
        /// </exception>
        public static bool IsLocalRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var connection = request.HttpContext.Connection;
            if (connection.RemoteIpAddress != null)
                if (connection.LocalIpAddress != null)
                    return connection.RemoteIpAddress.Equals(connection.LocalIpAddress);
                else
                    return IPAddress.IsLoopback(connection.RemoteIpAddress);

            // for in memory TestServer or when dealing with default connection info
            if (connection.RemoteIpAddress == null && connection.LocalIpAddress == null)
                return true;

            return false;
        }

        public static object GetBody(this HttpRequest request)
        {
            return GetBody<object>(request);
        }

        public static T GetBody<T>(this HttpRequest request)
        {
            try
            {
                T requestBodyObj;

                // Reset Body to Original Position
                request.Body.Position = 0;

                using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
                {
                    string requestBody = reader.ReadToEnd();

                    // Reformat to have beautiful json string
                    requestBodyObj = JsonConvert.DeserializeObject<T>(requestBody);
                }
                // Reset Body to Original Position
                request.Body.Position = 0;

                return requestBodyObj;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether current connection is secured 
        /// </summary>
        /// <returns> true - secured, false - not secured </returns>
        public static bool IsSecured(this HttpRequest request)
        {
            return request.IsHttps;
        }

        public static DeviceModel GetDeviceInfo(this HttpRequest request)
        {
            return new DeviceModel(request);
        }

        public static bool IsRequestFor(this HttpRequest request, string endpoint)
        {
            // get path query with out query param string
            var pathQuery = request.Path.Value?.Trim('/').ToLower() ?? String.Empty;
            var iPathQueryWithoutParam = pathQuery.IndexOf('?');
            pathQuery = iPathQueryWithoutParam > 0 ? pathQuery.Substring(iPathQueryWithoutParam) : pathQuery;
            pathQuery = pathQuery.ToLowerInvariant();

            // get endpoint without query param string
            endpoint = endpoint.Trim('/');
            var iEndpointWithoutParam = endpoint.IndexOf('?');
            endpoint = iEndpointWithoutParam > 0 ? endpoint.Substring(0, iEndpointWithoutParam) : endpoint;
            endpoint = endpoint.ToLowerInvariant();

            // check quest is swagger endpoint
            var isRequestTheEndpoint = pathQuery == endpoint;
            return isRequestTheEndpoint;
        }

        /// <summary>
        ///     Endpoint of current request combine schema://host with port/path 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static string GetEndpoint(this HttpRequest request)
        {
            return $"{request.Scheme}://{request.Host.Value}{request.Path.Value}";
        }
    }
}