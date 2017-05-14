using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.ContextSearch.SearchModel.AggModel.Buckets;

namespace TopCore.Framework.Search.Elastic.ContextSearch.SearchModel.AggModel
{
    public class GeoDistanceBucketAggregationsResult : AggregationResult<GeoDistanceBucketAggregationsResult>
    {
        public List<GeoDistanceRangeBucket> Buckets { get; set; }

        public override GeoDistanceBucketAggregationsResult GetValueFromJToken(JToken result)
        {
            return result.ToObject<GeoDistanceBucketAggregationsResult>();
        }
    }
}