using Newtonsoft.Json;

namespace Puppy.Search.Elastic.ContextSearch.SearchModel.AggModel.Buckets
{
    public class Bucket : BaseBucket
    {
        [JsonProperty("key")]
        public object Key { get; set; }
    }
}