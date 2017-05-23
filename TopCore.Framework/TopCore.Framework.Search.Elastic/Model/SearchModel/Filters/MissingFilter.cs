using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Filters
{
    public class MissingFilter : IFilter
    {
        private readonly string _field;
        private bool _existence;
        private bool _existenceSet;
        private bool _nullValue;
        private bool _nullValueSet;

        public MissingFilter(string field)
        {
            _field = field;
        }

        /// <summary>
        ///     existence When the existence parameter is set to true (the default), the missing
        ///     filter will include documents where the field has no values
        /// </summary>
        public bool Existence
        {
            get => _existence;
            set
            {
                _existence = value;
                _existenceSet = true;
            }
        }

        /// <summary>
        ///     null_value When the null_value parameter is set to true, the missing filter will
        ///     include documents where the field contains a null value
        /// </summary>
        public bool NullValue
        {
            get => _nullValue;
            set
            {
                _nullValue = value;
                _nullValueSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("missing");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            JsonHelper.WriteValue("field", _field, elasticCrudJsonWriter);
            JsonHelper.WriteValue("existence", _existence, elasticCrudJsonWriter, _existenceSet);
            JsonHelper.WriteValue("null_value", _nullValue, elasticCrudJsonWriter, _nullValueSet);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}