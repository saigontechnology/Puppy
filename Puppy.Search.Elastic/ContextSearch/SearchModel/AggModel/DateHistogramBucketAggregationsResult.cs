using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Puppy.Search.Elastic.ContextSearch.SearchModel.AggModel.Buckets;

namespace Puppy.Search.Elastic.ContextSearch.SearchModel.AggModel
{
    public class DateHistogramBucketAggregationsResult : AggregationResult<DateHistogramBucketAggregationsResult>
    {
        public List<DateHistogramBucket> Buckets { get; set; }

        public override DateHistogramBucketAggregationsResult GetValueFromJToken(JToken result)
        {
            return result.ToObject<DateHistogramBucketAggregationsResult>();
        }
    }
}