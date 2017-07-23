using Newtonsoft.Json.Linq;
using Puppy.Elastic.ContextSearch.SearchModel.AggModel.Buckets;
using System.Collections.Generic;

namespace Puppy.Elastic.ContextSearch.SearchModel.AggModel
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