using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TopCore.Framework.Search.Elastic.ContextSearch.SearchModel.AggModel.Buckets;

namespace TopCore.Framework.Search.Elastic.ContextSearch.SearchModel.AggModel
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