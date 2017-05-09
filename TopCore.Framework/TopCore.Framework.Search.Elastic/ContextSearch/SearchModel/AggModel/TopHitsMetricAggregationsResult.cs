using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TopCore.Framework.Search.Elastic.ContextSearch.SearchModel.AggModel
{
    public class TopHitsMetricAggregationsResult<T> : AggregationResult<TopHitsMetricAggregationsResult<T>>
    {
        [JsonProperty(PropertyName = "hits")]
        public Hits<T> Hits { get; set; }

        public override TopHitsMetricAggregationsResult<T> GetValueFromJToken(JToken result)
        {
            return result.ToObject<TopHitsMetricAggregationsResult<T>>();
        }
    }
}