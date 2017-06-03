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

using System;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Puppy.Eye.TripAdvisor.DotNet
{
    public class RestaurantEye
    {
        public async Task<List<string>> GetListDetailUrl(string cityCode)
        {
            var listUrl = new List<string>();

            using (var client = new HttpClient())
            {
                // Request
                var url = BuildPageUrl(cityCode);
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
                int totalPage = int.Parse(maxPageNumberSelector.GetAttribute("data-page-number"));
                const int itemPerPage = 30;

                // Get detail urls in each page
                for (int i = 0; i < totalPage; i++)
                {
                    int offSet = itemPerPage * i;
                    var pageUrl = BuildPageUrl(cityCode, offSet);

                    // Request
                    var pageResult = await client.GetAsync(pageUrl);
                    var pageBuffer = await pageResult.Content.ReadAsByteArrayAsync();
                    var pageByteArray = pageBuffer.ToArray();
                    var pageResponseString = Encoding.UTF8.GetString(pageByteArray, 0, pageByteArray.Length);
                    var pageDocument = parser.Parse(pageResponseString);

                    var listDetailUrlInPage = pageDocument.QuerySelectorAll("h3.title a").Select(x => "https://www.tripadvisor.com.vn" + x.GetAttribute("href"));
                    listUrl.AddRange(listDetailUrlInPage);
                }

                return listUrl.Distinct().ToList();
            }
        }

        public string BuildPageUrl(string cityCode, int offSet = 0)
        {
            var url = $"https://www.tripadvisor.com.vn/RestaurantSearch-g293925-oa{offSet}-{cityCode}.html";
            return url;
        }

        public async Task<RestaurantDetailModel> GetDetail(string url)
        {
            using (var client = new HttpClient())
            {
                // Request
                var result = await client.GetAsync(url);
                var buffer = await result.Content.ReadAsByteArrayAsync();
                var byteArray = buffer.ToArray();
                var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);

                // Parse
                var parser = new HtmlParser();
                var document = parser.Parse(responseString);

                RestaurantDetailModel restaurantDetailModel = new RestaurantDetailModel();

                // SEO
                restaurantDetailModel.SeoTitle = document.All
                    .Where(x => x.NodeName == "Meta" && x.GetAttribute("property") == "og:title")
                    .Select(x => x.GetAttribute("content")).Single().Trim();
                restaurantDetailModel.SeoKeywords = document.All
                    .Where(x => x.NodeName == "Meta" && x.GetAttribute("property") == "keywords")
                    .Select(x => x.GetAttribute("content")).Single().Trim();
                restaurantDetailModel.SeoDescription = document.All
                    .Where(x => x.NodeName == "Meta" && x.GetAttribute("property") == "description")
                    .Select(x => x.GetAttribute("content")).Single().Trim();

                // Basic Info

                var summaryInfoString = document.All
                    .Where(m => m.NodeName == "SCRIPT" && m.InnerHtml.Contains("@context"))
                    .Select(x => x.InnerHtml).Single().Trim();

                SummaryInfo summaryInfo = JsonConvert.DeserializeObject<SummaryInfo>(summaryInfoString);

                restaurantDetailModel.Url = summaryInfo.Url;
                restaurantDetailModel.Name = summaryInfo.Name;
                restaurantDetailModel.ImageUrl = summaryInfo.Image;
                restaurantDetailModel.Phone = document.QuerySelector(".phone").TextContent.Trim();

                // Rating
                restaurantDetailModel.AverageRating = double.Parse(summaryInfo.AggregateRating.RatingValue);
                restaurantDetailModel.RatingCount = int.Parse(summaryInfo.AggregateRating.ReviewCount);

                // Address
                restaurantDetailModel.Street = summaryInfo.Address.StreetAddress;
                restaurantDetailModel.Region = summaryInfo.Address.AddressRegion;
                restaurantDetailModel.Country = summaryInfo.Address.AddressCountry.Name;


                restaurantDetailModel.Latitude = 0;
                restaurantDetailModel.Longitude = 0;
                return restaurantDetailModel;
            }
        }
    }

    public class RestaurantDetailModel
    {
        [Key]
        public string Url { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string SeoTitle { get; set; }

        public string SeoKeywords { get; set; }

        public string SeoDescription { get; set; }

        public double AverageRating { get; set; }

        public int RatingCount { get; set; }

        public string Phone { get; set; }

        public string Street { get; set; }

        public string Region { get; set; }

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
}