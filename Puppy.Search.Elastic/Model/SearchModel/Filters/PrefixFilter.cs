using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.Model.SearchModel.Filters
{
    public class PrefixFilter : IFilter
    {
        private readonly string _field;
        private readonly object _prefix;

        public PrefixFilter(string field, object prefix)
        {
            _field = field;
            _prefix = prefix;
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("prefix");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            JsonHelper.WriteValue(_field, _prefix, elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}