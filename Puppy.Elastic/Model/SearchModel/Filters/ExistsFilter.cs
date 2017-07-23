using Puppy.Elastic.Utils;

namespace Puppy.Elastic.Model.SearchModel.Filters
{
    /// <summary>
    ///     Returns documents that have at least one non-null value in the original field: Always cached
    /// </summary>
    public class ExistsFilter : IFilter
    {
        private readonly string _field;

        public ExistsFilter(string field)
        {
            _field = field;
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("exists");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            JsonHelper.WriteValue("field", _field, elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}