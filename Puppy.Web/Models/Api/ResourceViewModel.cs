using Newtonsoft.Json;

namespace Puppy.Web.Models.Api
{
    public abstract class ResourceViewModel
    {
        [JsonProperty(Order = -2)]
        public ILinkViewModel Meta { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FormViewModel[] Forms { get; set; }
    }
}