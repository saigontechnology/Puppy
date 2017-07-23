using Newtonsoft.Json;
using Puppy.Elastic.Model;

namespace Puppy.Elastic.ContextAddDeleteUpdate.IndexModel
{
    public class OptimizeResult
    {
        [JsonProperty(PropertyName = "_shards")]
        public Shards Shards { get; set; }
    }
}