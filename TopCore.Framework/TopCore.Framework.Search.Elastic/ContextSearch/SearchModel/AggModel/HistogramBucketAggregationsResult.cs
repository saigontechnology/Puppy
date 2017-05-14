using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.ContextSearch.SearchModel.AggModel.Buckets;

namespace TopCore.Framework.Search.Elastic.ContextSearch.SearchModel.AggModel
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