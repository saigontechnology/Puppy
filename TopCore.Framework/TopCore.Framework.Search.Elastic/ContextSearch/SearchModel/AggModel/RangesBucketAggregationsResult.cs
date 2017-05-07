using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TopCore.Framework.Search.Elastic.ContextSearch.SearchModel.AggModel.Buckets;

namespace TopCore.Framework.Search.Elastic.ContextSearch.SearchModel.AggModel
{
	public class RangesBucketAggregationsResult : AggregationResult<RangesBucketAggregationsResult>
    {
        public List<RangeBucket> Buckets { get; set; }

        public override RangesBucketAggregationsResult GetValueFromJToken(JToken result)
        {
            return result.ToObject<RangesBucketAggregationsResult>();
        }
    }
}