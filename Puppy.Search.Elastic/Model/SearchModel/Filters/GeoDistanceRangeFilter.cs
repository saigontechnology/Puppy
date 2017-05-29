using Puppy.Search.Elastic.Model.GeoModel;
using Puppy.Search.Elastic.Model.Units;
using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.Model.SearchModel.Filters
{
    /// <summary>
    ///     Filters documents that include only hits that exists within a specific distance from a
    ///     geo point.
    /// </summary>
    public class GeoDistanceRangeFilter : IFilter
    {
        private readonly string _field;
        private readonly DistanceUnit _from;
        private readonly GeoPoint _location;
        private readonly DistanceUnit _to;
        private string _greaterThan;
        private string _greaterThanOrEqualTo;
        private bool _greaterThanOrEqualToSet;
        private bool _greaterThanSet;
        private bool _includeLower;
        private bool _includeLowerSet;
        private bool _includeUpper;
        private bool _includeUpperSet;
        private string _lessThan;
        private string _lessThanOrEqualTo;
        private bool _lessThanOrEqualToSet;
        private bool _lessThanSet;

        /// <summary>
        ///     Filters documents that include only hits that exists within a specific distance from
        ///     a geo point.
        /// </summary>
        /// <param name="field">    name of the field used for the geo point </param>
        /// <param name="location"> GeoPoint location </param>
        /// <param name="from">     from in distance units </param>
        /// <param name="to">       to in distance units </param>
        public GeoDistanceRangeFilter(string field, GeoPoint location, DistanceUnit from, DistanceUnit to)
        {
            _field = field;
            _location = location;
            _from = from;
            _to = to;
        }

        /// <summary>
        ///     gte Greater-than or equal to 
        /// </summary>
        public string GreaterThanOrEqualTo
        {
            get => _greaterThanOrEqualTo;
            set
            {
                _greaterThanOrEqualTo = value;
                _greaterThanOrEqualToSet = true;
            }
        }

        /// <summary>
        ///     gt Greater-than 
        /// </summary>
        public string GreaterThan
        {
            get => _greaterThan;
            set
            {
                _greaterThan = value;
                _greaterThanSet = true;
            }
        }

        /// <summary>
        ///     lte Less-than or equal to 
        /// </summary>
        public string LessThanOrEqualTo
        {
            get => _lessThanOrEqualTo;
            set
            {
                _lessThanOrEqualTo = value;
                _lessThanOrEqualToSet = true;
            }
        }

        /// <summary>
        ///     lt Less-than 
        /// </summary>
        public string LessThan
        {
            get => _lessThan;
            set
            {
                _lessThan = value;
                _lessThanSet = true;
            }
        }

        /// <summary>
        ///     include_lower 
        /// </summary>
        public bool IncludeLower
        {
            get => _includeLower;
            set
            {
                _includeLower = value;
                _includeLowerSet = true;
            }
        }

        /// <summary>
        ///     include_upper 
        /// </summary>
        public bool IncludeUpper
        {
            get => _includeUpper;
            set
            {
                _includeUpper = value;
                _includeUpperSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("geo_distance_range");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_field);
            _location.WriteJson(elasticCrudJsonWriter);
            JsonHelper.WriteValue("from", _from.GetDistanceUnit(), elasticCrudJsonWriter);
            JsonHelper.WriteValue("to", _to.GetDistanceUnit(), elasticCrudJsonWriter);
            JsonHelper.WriteValue("gte", _greaterThanOrEqualTo, elasticCrudJsonWriter, _greaterThanOrEqualToSet);
            JsonHelper.WriteValue("gt", _greaterThan, elasticCrudJsonWriter, _greaterThanSet);
            JsonHelper.WriteValue("lte", _lessThanOrEqualTo, elasticCrudJsonWriter, _lessThanOrEqualToSet);
            JsonHelper.WriteValue("lt", _lessThan, elasticCrudJsonWriter, _lessThanSet);
            JsonHelper.WriteValue("include_lower", _includeLower, elasticCrudJsonWriter, _includeLowerSet);
            JsonHelper.WriteValue("include_upper", _includeUpper, elasticCrudJsonWriter, _includeUpperSet);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}