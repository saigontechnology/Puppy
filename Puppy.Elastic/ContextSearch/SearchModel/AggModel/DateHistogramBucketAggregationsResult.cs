using Newtonsoft.Json.Linq;
using Puppy.Elastic.ContextSearch.SearchModel.AggModel.Buckets;
using System.Collections.Generic;

namespace Puppy.Elastic.ContextSearch.SearchModel.AggModel
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