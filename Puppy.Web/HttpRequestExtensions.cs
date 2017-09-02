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
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

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

        public static bool IsMobileRequest(this HttpRequest request)
        {
            var mobileCheck = new Regex(@"android|(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
            var mobileVersionCheck = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

            var userAgent = request.Headers[HeaderKey.UserAgent].ToString();
            return (mobileCheck.IsMatch(userAgent) || mobileVersionCheck.IsMatch(userAgent.Substring(0, 4)));
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

        public static string GetIp(this HttpRequest request)
        {
            var result = "";
            if (request.Headers?.Any() == true)
            {
                //look for the X-Forwarded-For (XFF) HTTP header field
                //it's used for identifying the originating IP address of a client connecting to a web server through an HTTP proxy or load balancer.
                string xff = request.Headers
                    .Where(x => HeaderKey.XForwardedFor.Equals(x.Value, StringComparison.OrdinalIgnoreCase))
                    .Select(k => request.Headers[k.Key])
                    .FirstOrDefault();

                //if you want to exclude private IP addresses, then see http://stackoverflow.com/questions/2577496/how-can-i-get-the-clients-ip-address-in-asp-net-mvc
                if (!string.IsNullOrWhiteSpace(xff))
                {
                    var lastIp = xff.Split(',').FirstOrDefault();
                    result = lastIp;
                }
            }

            // Standalize
            if (result == "::1")
            {
                result = "127.0.0.1";
            }

            if (string.IsNullOrEmpty(result)) return result;

            // Remove port
            int index = result.IndexOf(":", StringComparison.OrdinalIgnoreCase);

            if (index > 0)
            {
                result = result.Substring(0, index);
            }

            return result;
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
    }
}