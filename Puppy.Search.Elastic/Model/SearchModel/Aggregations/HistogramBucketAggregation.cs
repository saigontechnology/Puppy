using Puppy.Search.Elastic.Utils;
using System.Collections.Generic;

namespace Puppy.Search.Elastic.Model.SearchModel.Aggregations
{
    /// <summary>
    ///     A multi-bucket values source based aggregation that can be applied on numeric values
    ///     extracted from the documents. It dynamically builds fixed size (a.k.a. interval) buckets
    ///     over the values. For example, if the documents have a field that holds a price (numeric),
    ///     we can configure this aggregation to dynamically build buckets with interval 5 (in case
    ///     of price it may represent $5). When the aggregation executes, the price field of every
    ///     document will be evaluated and will be rounded down to its closest bucket - for example,
    ///     if the price is 32 and the bucket size is 5 then the rounding will yield 30 and thus the
    ///     document will "fall" into the bucket that is associated withe the key 30
    /// </summary>
    public class HistogramBucketAggregation : BaseBucketAggregation
    {
        private readonly string _field;
        private readonly uint _interval;
        private ExtendedBounds _extendedBounds;
        private bool _extendedBoundsSet;
        private bool _keyed;
        private bool _keyedSet;
        private uint _minDocCount;
        private bool _minDocCountSet;
        private OrderAgg _order;
        private bool _orderSet;
        private List<ScriptParameter> _params;
        private bool _paramsSet;

        private string _script;
        private bool _scriptSet;

        public HistogramBucketAggregation(string name, string field, uint interval) : base("histogram", name)
        {
            _field = field;
            _interval = interval;
        }

        public OrderAgg Order
        {
            get => _order;
            set
            {
                _order = value;
                _orderSet = true;
            }
        }

        /// <summary>
        ///     min_doc_count Terms are collected and ordered on a shard level and merged with the
        ///     terms collected from other shards in a second step. However, the shard does not have
        ///     the information about the global document count available. The decision if a term is
        ///     added to a candidate list depends only on the order computed on the shard using local
        ///     shard frequencies. The min_doc_count criterion is only applied after merging local
        ///     terms statistics of all shards. In a way the decision to add the term as a candidate
        ///     is made without being very certain about if the term will actually reach the required
        ///     min_doc_count. This might cause many (globally) high frequent terms to be missing in
        ///     the final result if low frequent terms populated the candidate lists. To avoid this,
        ///     the shard_size parameter can be increased to allow more candidate terms on the
        ///     shards. However, this increases memory consumption and network traffic.
        /// </summary>
        public uint MinDocCount
        {
            get => _minDocCount;
            set
            {
                _minDocCount = value;
                _minDocCountSet = true;
            }
        }

        public ExtendedBounds ExtendedBounds
        {
            get => _extendedBounds;
            set
            {
                _extendedBounds = value;
                _extendedBoundsSet = true;
            }
        }

        /// <summary>
        ///     If this value is set, the buckets are returned with id classes. 
        /// </summary>
        public bool Keyed
        {
            get => _keyed;
            set
            {
                _keyed = value;
                _keyedSet = true;
            }
        }

        public string Script
        {
            get => _script;
            set
            {
                _script = value;
                _scriptSet = true;
            }
        }

        public List<ScriptParameter> Params
        {
            get => _params;
            set
            {
                _params = value;
                _paramsSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("field", _field, elasticCrudJsonWriter);
            JsonHelper.WriteValue("interval", _interval, elasticCrudJsonWriter);
            JsonHelper.WriteValue("keyed", _keyed, elasticCrudJsonWriter, _keyedSet);
            if (_orderSet)
                _order.WriteJson(elasticCrudJsonWriter);
            JsonHelper.WriteValue("min_doc_count", _minDocCount, elasticCrudJsonWriter, _minDocCountSet);

            if (_extendedBoundsSet)
                _extendedBounds.WriteJson(elasticCrudJsonWriter);

            if (_scriptSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("script");
                elasticCrudJsonWriter.JsonWriter.WriteRawValue("\"" + _script + "\"");
                if (_paramsSet)
                {
                    elasticCrudJsonWriter.JsonWriter.WritePropertyName("params");
                    elasticCrudJsonWriter.JsonWriter.WriteStartObject();

                    foreach (var item in _params)
                    {
                        elasticCrudJsonWriter.JsonWriter.WritePropertyName(item.ParameterName);
                        elasticCrudJsonWriter.JsonWriter.WriteValue(item.ParameterValue);
                    }
                    elasticCrudJsonWriter.JsonWriter.WriteEndObject();
                }
            }
        }
    }

    public class ExtendedBounds
    {
        public uint Min { get; set; }
        public uint Max { get; set; }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("extended_bounds");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("min", Min, elasticCrudJsonWriter);
            JsonHelper.WriteValue("max", Max, elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}