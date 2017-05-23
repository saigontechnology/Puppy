using TopCore.Framework.Search.Elastic.Model.GeoModel;
using TopCore.Framework.Search.Elastic.Model.Units;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Queries.FunctionQuery
{
    public abstract class GeoDecayBaseScoreFunction : BaseScoreFunction
    {
        private readonly string _decayType;
        private readonly string _field;

        /// <summary>
        ///     origin The point of origin used for calculating distance. Must be given as a number
        ///     for numeric field, date for date fields and geo point for geo fields. Required for
        ///     geo and numeric field. For date fields the default is now. Date math (for example
        ///     now-1h) is supported for origin.
        /// </summary>
        private readonly GeoPoint _origin;

        /// <summary>
        ///     scale Required for all types. Defines the distance from origin at which the computed
        ///     score will equal decay parameter. For geo fields: Can be defined as number+unit (1km,
        ///     12m,…). Default unit is meters. For date fields: Can to be defined as a number+unit
        ///     ("1h", "10d",…). Default unit is milliseconds. For numeric field: Any number.
        /// </summary>
        private readonly DistanceUnit _scale;

        private double _decay;
        private bool _decaySet;

        private uint _offset;
        private bool _offsetSet;

        protected GeoDecayBaseScoreFunction(string field, GeoPoint origin, DistanceUnit scale, string decayType)
        {
            _field = field;
            _origin = origin;
            _scale = scale;
            _decayType = decayType;
        }

        /// <summary>
        ///     offset If an offset is defined, the decay function will only compute the decay
        ///     function for documents with a distance greater that the defined offset. The default
        ///     is 0.
        /// </summary>
        public uint Offset
        {
            get => _offset;
            set
            {
                _offset = value;
                _offsetSet = true;
            }
        }

        /// <summary>
        ///     decay The decay parameter defines how documents are scored at the distance given at
        ///     scale. If no decay is defined, documents at the distance scale will be scored 0.5.
        /// </summary>
        public double Decay
        {
            get => _decay;
            set
            {
                _decay = value;
                _decaySet = true;
            }
        }

        protected void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_decayType);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_field);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName("origin");
            _origin.WriteJson(elasticCrudJsonWriter);
            JsonHelper.WriteValue("offset", _offset, elasticCrudJsonWriter, _offsetSet);
            JsonHelper.WriteValue("scale", _scale.GetDistanceUnit(), elasticCrudJsonWriter);
            JsonHelper.WriteValue("decay", _decay, elasticCrudJsonWriter, _decaySet);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}