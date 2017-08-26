using Newtonsoft.Json;

namespace Puppy.Web.Models.Api
{
    public interface ILinkViewModel
    {
        string Href { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        string Method { get; set; }
    }
}