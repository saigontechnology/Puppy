using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.ContextSearch.SearchModel.AggModel.Buckets;

namespace TopCore.Framework.Search.Elastic.ContextSearch.SearchModel.AggModel
{
    public class FiltersBucketAggregationsResult : AggregationResult<FiltersBucketAggregationsResult>
    {
        public List<BaseBucket> Buckets { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JToken> SubAggregations { get; set; }

        public T GetSubAggregationsFromJTokenName<T>(string name)
        {
            return SubAggregations[name].ToObject<T>();
        }

        public T GetSingleMetricSubAggregationValue<T>(string name)
        {
            return SubAggregations[name]["value"].Value<T>();
        }

        public override FiltersBucketAggregationsResult GetValueFromJToken(JToken result)
        {
            return result.ToObject<FiltersBucketAggregationsResult>();
        }
    }
}