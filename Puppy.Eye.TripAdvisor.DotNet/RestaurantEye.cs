#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> RestaurantEye.cs </Name>
//         <Created> 03/06/2017 3:04:07 PM </Created>
//         <Key> c49b42ce-2db4-44d9-8ae9-b567022994e9 </Key>
//     </File>
//     <Summary>
//         RestaurantEye.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Puppy.Eye.TripAdvisor.DotNet
{
    public class RestaurantEye
    {
        public async Task<List<string>> GetListDetailUrl(string cityCode, string cityName)
        {
            var listUrl = new List<string>();

            using (var client = new HttpClient())
            {
                // Request
                var url = BuildPageUrl(cityCode, cityName);
                var result = await client.GetAsync(url);
                var buffer = await result.Content.ReadAsByteArrayAsync();
                var byteArray = buffer.ToArray();
                var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

                // Parse
                var parser = new HtmlParser();
                var document = parser.Parse(responseString);

                // Get total page
                var listPageNumberSelector = document.QuerySelectorAll(".pageNumbers a");
                var maxPageNumberSelector = listPageNumberSelector.LastOrDefault();
                var totalPage = int.Parse(maxPageNumberSelector?.GetAttribute("data-page-number") ?? "1");
                const int itemPerPage = 30;

                // Get detail urls in each page
                for (var i = 0; i < totalPage; i++)
                {
                    var offSet = itemPerPage * i;
                    var pageUrl = BuildPageUrl(cityCode, cityName, offSet);

                    // Request
                    var pageResult = await client.GetAsync(pageUrl);
                    var pageBuffer = await pageResult.Content.ReadAsByteArrayAsync();
                    var pageByteArray = pageBuffer.ToArray();
                    var pageResponseString = Encoding.UTF8.GetString(pageByteArray, 0, pageByteArray.Length);
                    var pageDocument = parser.Parse(pageResponseString);

                    var listDetailUrlInPage = pageDocument.QuerySelectorAll("h3.title a")
                        .Select(x => "https://www.tripadvisor.com.vn" + x.GetAttribute("href"));
                    listUrl.AddRange(listDetailUrlInPage);
                }

                return listUrl.Distinct().ToList();
            }
        }

        public string BuildPageUrl(string cityCode, string cityName, int offSet = 0)
        {
            var url = $"https://www.tripadvisor.com.vn/RestaurantSearch-g{cityCode}-oa{offSet}-{cityName}.html";
            return url;
        }

        public async Task<RestaurantDetailModel> GetDetail(string url)
        {
            try
            {
                int regionId = int.Parse(url.Split('-')[1].Replace("g", string.Empty));
                int restaurantId = int.Parse(url.Split('-')[2].Replace("d", string.Empty));

                using (var client = new HttpClient())
                {
                    client.Timeout = new TimeSpan(0, 1, 0, 0);

                    // Request
                    var result = await client.GetAsync(url).ConfigureAwait(true);
                    var buffer = await result.Content.ReadAsByteArrayAsync().ConfigureAwait(true);
                    var byteArray = buffer.ToArray();
                    var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

                    // Parse
                    var parser = new HtmlParser();
                    var document = parser.Parse(responseString);

                    var detail = new RestaurantDetailModel();

                    // SEO
                    detail.SeoTitle = document.All
                        .Where(x => x.NodeName == "META" && x.GetAttribute("property") == "og:title")
                        .Select(x => x.GetAttribute("content")).Single().Trim();

                    detail.SeoKeywords = document.All
                        .Where(x => x.NodeName == "META" && x.GetAttribute("name") == "keywords")
                        .Select(x => x.GetAttribute("content")).Single().Trim();
                    detail.SeoDescription = document.All
                        .Where(x => x.NodeName == "META" && x.GetAttribute("name") == "description")
                        .Select(x => x.GetAttribute("content")).Single().Trim();

                    // Basic Info
                    var summaryInfoString = document.All
                        .Where(m => m.NodeName == "SCRIPT" && m.InnerHtml.Contains("@context"))
                        .Select(x => x.InnerHtml).Single().Trim();

                    var summaryInfo = JsonConvert.DeserializeObject<SummaryInfo>(summaryInfoString);

                    detail.Url = url;
                    detail.Id = restaurantId;
                    detail.Name = summaryInfo.Name;
                    detail.Cuisines = document.QuerySelector(".cuisines")?.QuerySelector(".text")?.TextContent.Trim();

                    detail.SeoImageUrl = summaryInfo.Image;
                    detail.Phone = document.QuerySelector(".phone").TextContent.Trim();

                    // Rating
                    if (string.IsNullOrWhiteSpace(summaryInfo.AggregateRating?.ReviewCount))
                    {
                        return null;
                    }
                    detail.AverageRating = double.Parse(summaryInfo.AggregateRating.RatingValue);
                    detail.RatingCount = int.Parse(summaryInfo.AggregateRating.ReviewCount);
                    detail.RatingDescription = document.QuerySelector(".header_popularity")?.TextContent.Trim();

                    if (!string.IsNullOrWhiteSpace(detail.RatingDescription))
                    {
                        int indexOfRank = detail.RatingDescription.IndexOf(" ", StringComparison.OrdinalIgnoreCase) + 1;
                        int indexOfEndRank = detail.RatingDescription.IndexOf("trong", StringComparison.OrdinalIgnoreCase) - 1;
                        detail.Rank = int.Parse(detail.RatingDescription.Substring(indexOfRank, indexOfEndRank - indexOfRank).Replace(".", string.Empty));

                        int indexOfTotalRank = detail.RatingDescription.IndexOf("trong số", StringComparison.OrdinalIgnoreCase) + 9;
                        int indexOfTotalEndRank = detail.RatingDescription.IndexOf("Nhà", StringComparison.OrdinalIgnoreCase) - 1;
                        detail.TotalRank = int.Parse(detail.RatingDescription.Substring(indexOfTotalRank, indexOfTotalEndRank - indexOfTotalRank).Replace(".", string.Empty));
                    }

                    var startCounts = document.QuerySelector("#ratingFilter").QuerySelectorAll(".filterItem")
                        .Select(x => x.QuerySelectorAll("span")[3].TextContent).ToList();

                    detail.Star5Count = int.Parse(startCounts[0]);
                    detail.Star4Count = int.Parse(startCounts[1]);
                    detail.Star3Count = int.Parse(startCounts[2]);
                    detail.Star2Count = int.Parse(startCounts[3]);
                    detail.Star1Count = int.Parse(startCounts[4]);

                    // Address
                    detail.Street = summaryInfo.Address.StreetAddress;
                    detail.RegionId = regionId;
                    detail.RegionName = summaryInfo.Address.AddressLocality;
                    detail.Country = summaryInfo.Address.AddressCountry.Name;

                    // GEO
                    var geoString = document.All
                        .Where(m => m.NodeName == "SCRIPT" && m.InnerHtml != null && m.InnerHtml.Contains("window.mapDivId"))
                        .Select(x => x.InnerHtml).Single().Trim();

                    int startGeoJsonIndex = geoString.IndexOf("window.map0Div = ",
                        StringComparison.InvariantCultureIgnoreCase) + "window.map0Div = ".Length;
                    geoString = geoString.Substring(startGeoJsonIndex, geoString.Length - startGeoJsonIndex);
                    geoString = geoString.Substring(0, geoString.IndexOf(";", StringComparison.InvariantCultureIgnoreCase));

                    AddressMap addressMap = JsonConvert.DeserializeObject<AddressMap>(geoString);

                    detail.Latitude = double.Parse(addressMap.Lat.ToString(CultureInfo.InvariantCulture));
                    detail.Longitude = double.Parse(addressMap.Lng.ToString(CultureInfo.InvariantCulture));

                    return detail;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }

    public class RestaurantDetailModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public string Url { get; set; }

        public string Name { get; set; }

        public string Cuisines { get; set; }

        public string SeoImageUrl { get; set; }

        public string SeoTitle { get; set; }

        public string SeoKeywords { get; set; }

        public string SeoDescription { get; set; }

        public int Rank { get; set; }

        public int TotalRank { get; set; }

        public string RatingDescription { get; set; }

        public double AverageRating { get; set; }

        public int RatingCount { get; set; }

        public int Star5Count { get; set; }
        public int Star4Count { get; set; }
        public int Star3Count { get; set; }
        public int Star2Count { get; set; }
        public int Star1Count { get; set; }

        public string Phone { get; set; }

        public string Street { get; set; }

        public int RegionId { get; set; }

        public string RegionName { get; set; }

        public string Country { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public DateTimeOffset CreateTime { get; set; } = DateTimeOffset.UtcNow;
    }

    internal class SummaryInfo
    {
        [JsonProperty(PropertyName = "context")]
        public string Context { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "image")]
        public string Image { get; set; }

        [JsonProperty(PropertyName = "aggregateRating")]
        public AggregateRating AggregateRating { get; set; }

        [JsonProperty(PropertyName = "address")]
        public Address Address { get; set; }

        public string Phone { get; set; }
    }

    internal class AggregateRating
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "ratingValue")]
        public string RatingValue { get; set; }

        [JsonProperty(PropertyName = "reviewCount")]
        public string ReviewCount { get; set; }
    }

    internal class Address
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "streetAddress")]
        public string StreetAddress { get; set; }

        [JsonProperty(PropertyName = "addressLocality")]
        public string AddressLocality { get; set; }

        [JsonProperty(PropertyName = "addressRegion")]
        public string AddressRegion { get; set; }

        [JsonProperty(PropertyName = "postalCode")]
        public object PostalCode { get; set; }

        [JsonProperty(PropertyName = "addressCountry")]
        public AddressCountry AddressCountry { get; set; }
    }

    internal class AddressCountry
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }

    internal class AddressMap
    {
        [JsonProperty(PropertyName = "lat")]
        public float Lat { get; set; }

        [JsonProperty(PropertyName = "lng")]
        public float Lng { get; set; }

        [JsonProperty(PropertyName = "zoom")]
        public object Zoom { get; set; }

        [JsonProperty(PropertyName = "locId")]
        public int LocId { get; set; }

        [JsonProperty(PropertyName = "geoId")]
        public int GeoId { get; set; }

        [JsonProperty(PropertyName = "isAttraction")]
        public bool IsAttraction { get; set; }

        [JsonProperty(PropertyName = "isEatery")]
        public bool IsEatery { get; set; }

        [JsonProperty(PropertyName = "isLodging")]
        public bool IsLodging { get; set; }

        [JsonProperty(PropertyName = "isNeighborhood")]
        public bool IsNeighborhood { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [JsonProperty(PropertyName = "homeIcon")]
        public bool HomeIcon { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "minPins")]
        public object[][] MinPins { get; set; }

        [JsonProperty(PropertyName = "units")]
        public string Units { get; set; }

        [JsonProperty(PropertyName = "geoMap")]
        public bool GeoMap { get; set; }

        [JsonProperty(PropertyName = "tabletFullSite")]
        public bool TabletFullSite { get; set; }

        [JsonProperty(PropertyName = "reuseHoverDivs")]
        public bool ReuseHoverDivs { get; set; }

        [JsonProperty(PropertyName = "noSponsors")]
        public bool NoSponsors { get; set; }
    }
}