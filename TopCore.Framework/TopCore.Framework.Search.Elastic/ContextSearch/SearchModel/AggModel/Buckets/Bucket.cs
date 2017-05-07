using Newtonsoft.Json;

namespace TopCore.Framework.Search.Elastic.ContextSearch.SearchModel.AggModel.Buckets
{
	public class Bucket : BaseBucket
    {
        [JsonProperty("key")]
        public object Key { get; set; }
    }
}