using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Filters
{
    public class RangeFilter : IFilter
    {
        private readonly string _field;
        private object _greaterThan;
        private object _greaterThanOrEqualTo;
        private bool _greaterThanOrEqualToSet;
        private bool _greaterThanSet;
        private bool _includeLower;
        private bool _includeLowerSet;
        private bool _includeUpper;
        private bool _includeUpperSet;
        private object _lessThan;
        private object _lessThanOrEqualTo;
        private bool _lessThanOrEqualToSet;
        private bool _lessThanSet;
        private string _timeZone;
        private bool _timeZoneSet;

        public RangeFilter(string field)
        {
            _field = field;
        }

        /// <summary>
        ///     gte Greater-than or equal to 
        /// </summary>
        public object GreaterThanOrEqualTo
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
        public object GreaterThan
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
        public object LessThanOrEqualTo
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
        public object LessThan
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

        /// <summary>
        ///     time_zone When applied on date fields the range filter accepts also a time_zone
        ///     parameter. The time_zone parameter will be applied to your input lower and upper
        ///     bounds and will move them to UTC time based date if you give a date with a timezone
        ///     explicitly defined and use the time_zone parameter, time_zone will be ignored. For
        ///     example, setting gte to 2012-01-01T00:00:00+01:00 with "time_zone":"+10:00" will
        ///     still use +01:00 time zone.
        /// </summary>
        public string TimeZone
        {
            get => _timeZone;
            set
            {
                _timeZone = value;
                _timeZoneSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("range");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_field);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("gte", _greaterThanOrEqualTo, elasticCrudJsonWriter, _greaterThanOrEqualToSet);
            JsonHelper.WriteValue("gt", _greaterThan, elasticCrudJsonWriter, _greaterThanSet);
            JsonHelper.WriteValue("lte", _lessThanOrEqualTo, elasticCrudJsonWriter, _lessThanOrEqualToSet);
            JsonHelper.WriteValue("lt", _lessThan, elasticCrudJsonWriter, _lessThanSet);
            JsonHelper.WriteValue("time_zone", _timeZone, elasticCrudJsonWriter, _timeZoneSet);
            JsonHelper.WriteValue("include_lower", _includeLower, elasticCrudJsonWriter, _includeLowerSet);
            JsonHelper.WriteValue("include_upper", _includeUpper, elasticCrudJsonWriter, _includeUpperSet);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}