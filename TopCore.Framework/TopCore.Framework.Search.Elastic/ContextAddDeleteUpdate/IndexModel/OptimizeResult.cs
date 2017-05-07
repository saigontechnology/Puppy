using Newtonsoft.Json;
using TopCore.Framework.Search.Elastic.Model;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel
{
	public class OptimizeResult
    {
        [JsonProperty(PropertyName = "_shards")]
        public Shards Shards { get; set; }
    }
}