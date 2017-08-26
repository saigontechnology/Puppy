using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace Puppy.Web.Models.Api
{
    public class PlaceholderLinkModel : ILinkViewModel
    {
        [JsonProperty(Order = -2)]
        public string Method { get; set; }

        [JsonProperty(Order = -1)]
        public string Href { get; set; }

        [JsonProperty(Order = 1)]
        public RouteValueDictionary Values { get; set; } = new RouteValueDictionary();

        public PlaceholderLinkModel()
        {
        }

        public PlaceholderLinkModel(ILinkViewModel existing)
        {
            Href = existing.Href;
            Method = existing.Method;
            PlaceholderLinkModel asPlaceholder = existing as PlaceholderLinkModel;
            if (asPlaceholder != null)
                Values = new RouteValueDictionary(asPlaceholder.Values);
        }

        public static PlaceholderLinkModel ToCollection(string endpoint, string method = "GET", object values = null)
        {
            var placeholderLinkModel = new PlaceholderLinkModel
            {
                Method = method,
                Values = new RouteValueDictionary(values)
            };

            placeholderLinkModel.Href = placeholderLinkModel.Values.GetUrlWithQueries(endpoint);
            return placeholderLinkModel;
        }
    }
}