using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puppy.Elastic.ContextSearch.SearchModel.AggModel.Buckets;
using System.Collections.Generic;

namespace Puppy.Elastic.ContextSearch.SearchModel.AggModel
{
    public class SignificantTermsBucketAggregationsResult : AggregationResult<SignificantTermsBucketAggregationsResult>
    {
        [JsonProperty("doc_count")]
        public int DocCount { get; set; }

        public List<SignificantTermsBucket> Buckets { get; set; }

        public override SignificantTermsBucketAggregationsResult GetValueFromJToken(JToken result)
        {
            return result.ToObject<SignificantTermsBucketAggregationsResult>();
        }
    }
}