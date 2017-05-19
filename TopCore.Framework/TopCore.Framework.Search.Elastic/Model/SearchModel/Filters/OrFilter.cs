using System.Collections.Generic;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Filters
{
    /// <summary>
    ///   A filter that matches documents using the OR boolean operator on other filters. Can be placed within queries that accept a filter. 
    /// </summary>
    public class OrFilter : IFilter
    {
        private readonly List<IFilter> _or;

        public OrFilter(List<IFilter> or)
        {
            _or = or;
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("or");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            WriteOrFilterList(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }

        private void WriteOrFilterList(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("filters");
            elasticCrudJsonWriter.JsonWriter.WriteStartArray();

            foreach (var or in _or)
            {
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                or.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }

            elasticCrudJsonWriter.JsonWriter.WriteEndArray();
        }
    }
}