using Newtonsoft.Json.Linq;
using Puppy.Search.Elastic.ContextSearch.SearchModel.AggModel.Buckets;
using System.Collections.Generic;

namespace Puppy.Search.Elastic.ContextSearch.SearchModel.AggModel
{
    public class GeohashGridBucketAggregationsResult : AggregationResult<GeohashGridBucketAggregationsResult>
    {
        public List<Bucket> Buckets { get; set; }

        public override GeohashGridBucketAggregationsResult GetValueFromJToken(JToken result)
        {
            return result.ToObject<GeohashGridBucketAggregationsResult>();
        }
    }
}