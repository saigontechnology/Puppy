using Newtonsoft.Json;

namespace Puppy.Elastic.ContextSearch.SearchModel.AggModel.Buckets
{
    public class Bucket : BaseBucket
    {
        [JsonProperty("key")]
        public object Key { get; set; }
    }
}