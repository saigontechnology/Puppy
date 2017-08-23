using Newtonsoft.Json;

namespace Puppy.Web.Models.Api
{
    public interface ILinkViewModel
    {
        string Href { get; set; }

        [JsonProperty(PropertyName = "rel", NullValueHandling = NullValueHandling.Ignore)]
        string[] Relations { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        string Method { get; set; }
    }
}