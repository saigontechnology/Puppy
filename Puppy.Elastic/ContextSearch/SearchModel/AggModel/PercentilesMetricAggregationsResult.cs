using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Puppy.Elastic.ContextSearch.SearchModel.AggModel
{
    public class PercentilesMetricAggregationsResult : AggregationResult<PercentilesMetricAggregationsResult>
    {
        public Dictionary<string, double> Values { get; set; }

        public override PercentilesMetricAggregationsResult GetValueFromJToken(JToken result)
        {
            Values = new Dictionary<string, double>();
            Values = result["values"].ToObject<Dictionary<string, double>>();
            return this;
        }
    }
}