using Newtonsoft.Json;

namespace Puppy.Elastic.ContextSearch.SearchModel.AggModel.Buckets
{
    public class DateHistogramBucket : BaseBucket
    {
        [JsonProperty("key")]
        public object Key { get; set; }

        [JsonProperty("key_as_string")]
        public string KeyAsString { get; set; }
    }
}