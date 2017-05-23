using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Sorting
{
    public class SortStandard : ISort
    {
        private readonly string _field;
        private SortMissing _missing;
        private bool _missingSet;
        private SortMode _mode;
        private bool _modeSet;
        private IFilter _nestedFilter;
        private bool _nestedFilterSet;
        private string _unmappedType;
        private bool _unmappedTypeSet;

        public SortStandard(string field)
        {
            _field = field;
            Order = OrderEnum.asc;
        }

        public OrderEnum Order { get; set; }

        /// <summary>
        ///     mode Elastic supports sorting by array or multi-valued fields. The mode option
        ///     controls what array value is picked for sorting the document it belongs to. The mode
        ///     option can have the following values: SortMode enum: min, max, sum, avg
        /// </summary>
        public SortMode Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                _modeSet = true;
            }
        }

        /// <summary>
        ///     The missing parameter specifies how docs which are missing the field should be
        ///     treated: The missing value can be set to _last, _first, or a custom value (that will
        ///     be used for missing docs as the sort value).
        /// </summary>
        public SortMissing Missing
        {
            get => _missing;
            set
            {
                _missing = value;
                _missingSet = true;
            }
        }

        /// <summary>
        ///     "nested_filter" 
        /// </summary>
        public IFilter NestedFilter
        {
            get => _nestedFilter;
            set
            {
                _nestedFilter = value;
                _nestedFilterSet = true;
            }
        }

        /// <summary>
        ///     unmapped_type By default, the search request will fail if there is no mapping
        ///     associated with a field. The unmapped_type option allows to ignore fields that have
        ///     no mapping and not sort by them. The value of this parameter is used to determine
        ///     what sort values to emit. If any of the indices that are queried doesn’t have a
        ///     mapping for price then Elastic will handle it as if there was a mapping of type long,
        ///     with all documents in this index having no value for this field.
        /// </summary>
        public string UnmappedType
        {
            get => _unmappedType;
            set
            {
                _unmappedType = value;
                _unmappedTypeSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_field);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("order", Order.ToString(), elasticCrudJsonWriter);
            JsonHelper.WriteValue("mode", _mode.ToString(), elasticCrudJsonWriter, _modeSet);
            JsonHelper.WriteValue("missing", _missing.ToString(), elasticCrudJsonWriter, _missingSet);
            JsonHelper.WriteValue("unmapped_type", _unmappedType, elasticCrudJsonWriter, _unmappedTypeSet);

            if (_nestedFilterSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("nested_filter");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                _nestedFilter.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }

    public enum SortMissing
    {
        _last,
        _first
    }

    public enum SortMode
    {
        /// <summary>
        ///     Pick the lowest value. 
        /// </summary>
        min,

        /// <summary>
        ///     Pick the highest value. 
        /// </summary>
        max,

        /// <summary>
        ///     Use the sum of all values as sort value. Only applicable for number based array fields. 
        /// </summary>
        sum,

        /// <summary>
        ///     Use the average of all values as sort value. Only applicable for number based array fields.
        /// </summary>
        avg
    }
}