using Microsoft.AspNetCore.Routing;

namespace Puppy.Web.Models.Api
{
    public class PlaceholderLinkModel : ILinkViewModel
    {
        public PlaceholderLinkModel()
        {
        }

        public PlaceholderLinkModel(ILinkViewModel existing)
        {
            Href = existing.Href;
            Method = existing.Method;

            var asPlaceholder = existing as PlaceholderLinkModel;
            if (asPlaceholder != null)
                Values = new RouteValueDictionary(asPlaceholder.Values);
        }

        public RouteValueDictionary Values { get; set; } = new RouteValueDictionary();

        public string Href { get; set; }
        public string Method { get; set; }

        public static PlaceholderLinkModel ToResource(string endpoint, string method = "GET", object values = null)
        {
            return new PlaceholderLinkModel
            {
                Href = endpoint,
                Method = method,
                Values = new RouteValueDictionary(values)
            };
        }

        public static PlaceholderLinkModel ToCollection(string hrefPattern, string method = "GET", object values = null)
        {
            var placeholderLinkViewModel = new PlaceholderLinkModel
            {
                Method = method,
                Values = new RouteValueDictionary(values),
                Href = hrefPattern
            };

            foreach (var newLinkValue in placeholderLinkViewModel.Values)
            {
                var hrefKey = "{" + newLinkValue.Key + "}";
                if (placeholderLinkViewModel.Href.Contains(hrefKey))
                {
                    placeholderLinkViewModel.Href = placeholderLinkViewModel.Href.Replace(hrefKey, newLinkValue.Value?.ToString() ?? string.Empty);
                }
                else
                {
                    var hrefValue = newLinkValue.Value?.ToString();

                    if (string.IsNullOrWhiteSpace(hrefValue))
                        continue;

                    placeholderLinkViewModel.Href += placeholderLinkViewModel.Href.Contains("?") ? "&" : "?";
                    var hrefParam = $"{newLinkValue.Key}={hrefValue}";
                    placeholderLinkViewModel.Href += hrefParam;
                }
            }

            return placeholderLinkViewModel;
        }
    }
}