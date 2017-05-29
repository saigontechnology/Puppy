using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Puppy.Search.Elastic.ContextSearch.SearchModel.AggModel.Buckets;

namespace Puppy.Search.Elastic.ContextSearch.SearchModel.AggModel
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