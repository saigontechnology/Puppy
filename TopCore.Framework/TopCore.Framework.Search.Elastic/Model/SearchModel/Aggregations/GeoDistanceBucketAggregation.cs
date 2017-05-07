using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Model.GeoModel;
using TopCore.Framework.Search.Elastic.Model.SearchModel.Aggregations.RangeParam;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Aggregations
{
	public class GeoDistanceBucketAggregation : BaseBucketAggregation
    {
        private readonly string _field;
        private readonly GeoPoint _origin;
        private readonly List<RangeAggregationParameter<uint>> _ranges;
        private DistanceType _distanceType;
        private bool _distanceTypeSet;
        private bool _keyed;
        private bool _keyedSet;
        private List<ScriptParameter> _params;
        private bool _paramsSet;

        private string _script;
        private bool _scriptSet;
        private DistanceUnitEnum _unit;
        private bool _unitSet;

        public GeoDistanceBucketAggregation(string name, string field, GeoPoint origin,
            List<RangeAggregationParameter<uint>> ranges)
            : base("geo_distance", name)
        {
            _field = field;
            _origin = origin;
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

        public DistanceUnitEnum Unit
        {
            get => _unit;
            set
            {
                _unit = value;
                _unitSet = true;
            }
        }

	    /// <summary>
	    ///     distance_type How to compute the distance. Can either be sloppy_arc (default), arc (slighly more precise but significantly
	    ///     slower) or plane (faster, but inaccurate on long distances and close to the poles).
	    /// </summary>
	    public DistanceType DistanceType
        {
            get => _distanceType;
            set
            {
                _distanceType = value;
                _distanceTypeSet = true;
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

            elasticCrudJsonWriter.JsonWriter.WritePropertyName("origin");
            _origin.WriteJson(elasticCrudJsonWriter);

            JsonHelper.WriteValue("unit", _unit.ToString(), elasticCrudJsonWriter, _unitSet);
            JsonHelper.WriteValue("distance_type", _distanceType.ToString(), elasticCrudJsonWriter, _distanceTypeSet);
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