using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Puppy.Web.Models.Api
{
    [Serializable]
    [KnownType(typeof(PagedCollectionModel<>))]
    public class PagedCollectionModel<T>
    {
        [JsonProperty(Order = -2)]
        public ILinkViewModel Meta { get; set; }

        [JsonProperty(Order = 1)]
        public ILinkViewModel First { get; set; }

        [JsonProperty(Order = 2)]
        public ILinkViewModel Previous { get; set; }

        [JsonProperty(Order = 3)]
        public ILinkViewModel Next { get; set; }

        [JsonProperty(Order = 4)]
        public ILinkViewModel Last { get; set; }

        [JsonProperty(Order = 5)]
        public long Total { get; set; }

        [JsonProperty(Order = 6)]
        public List<T> Items { get; set; }

        [JsonProperty(Order = 7)]
        [JsonExtensionData]
        public Dictionary<string, object> AdditionalData { get; set; }
    }
}