﻿using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Aggregations
{
    /// <summary>
    ///     A multi-value metrics aggregation that calculates one or more percentile ranks over
    ///     numeric values extracted from the aggregated documents. These values can be extracted
    ///     either from specific numeric fields in the documents, or be generated by a provided script.
    /// </summary>
    public class PercentileRanksMetricAggregation : BaseMetricAggregation
    {
        private readonly List<double> _values;

        public PercentileRanksMetricAggregation(string name, string field, List<double> values) : base(
            "percentile_ranks", name, field)
        {
            _values = values;
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteListValue("values", _values, elasticCrudJsonWriter);
        }
    }
}