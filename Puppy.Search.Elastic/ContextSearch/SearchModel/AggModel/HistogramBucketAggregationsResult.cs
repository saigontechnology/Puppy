using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Puppy.Search.Elastic.ContextSearch.SearchModel.AggModel.Buckets;

namespace Puppy.Search.Elastic.ContextSearch.SearchModel.AggModel
{
    public class HistogramBucketAggregationsResult : AggregationResult<HistogramBucketAggregationsResult>
    {
        public List<Bucket> Buckets { get; set; }

        public override HistogramBucketAggregationsResult GetValueFromJToken(JToken result)
        {
            return result.ToObject<HistogramBucketAggregationsResult>();
        }
    }
}