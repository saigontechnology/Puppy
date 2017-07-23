using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Puppy.Elastic.ContextSearch.SearchModel.AggModel
{
    public class PercentileRanksMetricAggregationsResult : AggregationResult<PercentileRanksMetricAggregationsResult>
    {
        public Dictionary<string, double> Values { get; set; }

        public override PercentileRanksMetricAggregationsResult GetValueFromJToken(JToken result)
        {
            Values = new Dictionary<string, double>();
            Values = result["values"].ToObject<Dictionary<string, double>>();
            return this;
        }
    }
}