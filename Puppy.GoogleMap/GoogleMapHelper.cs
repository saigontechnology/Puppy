#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> TopCore</Project>
//     <File>
//         <Name> GoogleMapHelper.cs </Name>
//         <Created> 26/05/2017 11:20:34 PM </Created>
//         <Key> 77cf2628-7746-4841-8668-17623de31c15 </Key>
//     </File>
//     <Summary>
//         GoogleMapHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Puppy.GoogleMap
{
    public static class GoogleMapHelper
    {
        /// <summary>
        ///     Get Distance Matrix 
        /// </summary>
        /// <param name="originalCoordinates">   </param>
        /// <param name="destinationCoordinates"></param>
        /// <param name="googleMatrixBaseUrl">   
        ///     Google Matrix API base URL, default is https://maps.googleapis.com/maps/api/distancematrix/json
        /// </param>
        /// <param name="googleApiKey">           Google API Key, get it from https://console.developers.google.com </param>
        /// <param name="extraValues">           
        ///     Extra query params, ex: "mode=driving", "language=en-US"
        /// </param>
        /// <returns></returns>
        public static async Task<DistanceDurationMatrixModel> GetDistanceDurationMatrixAsync(
            CoordinateModel[] originalCoordinates,
            CoordinateModel[] destinationCoordinates,
            string googleApiKey = "",
            KeyValuePair<string, string>[] extraValues = null,
            string googleMatrixBaseUrl = "https://maps.googleapis.com/maps/api/distancematrix/json")
        {
            var origins = string.Join("|", originalCoordinates.Select(x => $"{x.Latitude.ToString(CultureInfo.InvariantCulture)},{x.Longitude.ToString(CultureInfo.InvariantCulture)}"));

            var destinations = string.Join("|", destinationCoordinates.Select(x => $"{x.Latitude.ToString(CultureInfo.InvariantCulture)},{x.Longitude.ToString(CultureInfo.InvariantCulture)}"));

            var extraValueQuery = string.Empty;
            if (extraValues?.Any() == true)
            {
                extraValueQuery = "&" + string.Join("&", extraValues.Select(x => $"{x.Key}={x.Value}"));
            }

            var requestUrl = $"{googleMatrixBaseUrl}?origins={origins}&destinations={destinations}&key={googleApiKey}{extraValueQuery}";

            using (var client = new HttpClient())
            {
                var uri = new Uri(requestUrl);

                var response = await client.GetAsync(uri).ConfigureAwait(true);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"{nameof(GetDistanceDurationMatrixAsync)} failed with status code: " + response.StatusCode);
                }

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(true);

                return JsonConvert.DeserializeObject<DistanceDurationMatrixModel>(content);
            }
        }
    }
}