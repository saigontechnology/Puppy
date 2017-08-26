using Newtonsoft.Json;
using System.Collections.Generic;

namespace Puppy.Web.Models.Api
{
    public class PagedCollectionModel<T>
    {
        [JsonProperty(Order = -2)]
        public ILinkViewModel Meta { get; set; }

        [JsonProperty(Order = 1, NullValueHandling = NullValueHandling.Ignore)]
        public ILinkViewModel First { get; set; }

        [JsonProperty(Order = 2, NullValueHandling = NullValueHandling.Ignore)]
        public ILinkViewModel Previous { get; set; }

        [JsonProperty(Order = 3, NullValueHandling = NullValueHandling.Ignore)]
        public ILinkViewModel Next { get; set; }

        [JsonProperty(Order = 4, NullValueHandling = NullValueHandling.Ignore)]
        public ILinkViewModel Last { get; set; }

        [JsonProperty(Order = 5)]
        public long Total { get; set; }

        [JsonProperty(Order = 6, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<T> Items { get; set; }
    }
}