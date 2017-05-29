using Newtonsoft.Json;
using Puppy.Search.Elastic.Model;

namespace Puppy.Search.Elastic.ContextAddDeleteUpdate.IndexModel
{
    public class OptimizeResult
    {
        [JsonProperty(PropertyName = "_shards")]
        public Shards Shards { get; set; }
    }
}