using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Filters
{
    /// <summary>
    ///     Filters documents matching the provided document / mapping type. Note, this filter can work even when the _type field is not indexed (using the _uid field). 
    /// </summary>
    public class TypeFilter : IFilter
    {
        private readonly string _type;

        public TypeFilter(string type)
        {
            _type = type;
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("type");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            JsonHelper.WriteValue("value", _type, elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}