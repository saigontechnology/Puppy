using Puppy.Search.Elastic.Model.SearchModel.Aggregations.RangeParam;
using Puppy.Search.Elastic.Utils;
using System.Collections.Generic;

namespace Puppy.Search.Elastic.Model.SearchModel.Aggregations
{
    public class RangeBucketAggregation<T> : BaseBucketAggregation
    {
        private readonly string _field;
        private readonly List<RangeAggregationParameter<T>> _ranges;
        private bool _keyed;
        private bool _keyedSet;
        private List<ScriptParameter> _params;
        private bool _paramsSet;

        private string _script;
        private bool _scriptSet;

        public RangeBucketAggregation(string name, string field,
            List<RangeAggregationParameter<T>> ranges) : base("range", name)
        {
            _field = field;
            _ranges = ranges;
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
            JsonHelper.WriteValue("keyed", _keyed, elasticCrudJsonWriter, _keyedSet);

            elasticCrudJsonWriter.JsonWriter.WritePropertyName("ranges");
            elasticCrudJsonWriter.JsonWriter.WriteStartArray();
            foreach (var rangeAggregationParameter in _ranges)
                rangeAggregationParameter.WriteJson(elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndArray();

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
}