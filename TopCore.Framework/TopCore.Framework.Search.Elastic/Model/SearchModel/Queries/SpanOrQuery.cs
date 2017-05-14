using System.Collections.Generic;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Queries
{
    /// <summary>
    ///     Matches the union of its span clauses. The span or query maps to Lucene SpanOrQuery.
    /// </summary>
    public class SpanOrQuery : ISpanQuery
    {
        private readonly List<ISpanQuery> _queries;

        public SpanOrQuery(List<ISpanQuery> queries)
        {
            if (queries == null)
                throw new ElasticException("parameter List<ISpanQuery> queries cannot be null");
            if (queries.Count < 0)
                throw new ElasticException("parameter List<ISpanQuery> queries should have at least one element");
            _queries = queries;
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("span_or");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("clauses");
            elasticCrudJsonWriter.JsonWriter.WriteStartArray();

            foreach (var item in _queries)
            {
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                item.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }

            elasticCrudJsonWriter.JsonWriter.WriteEndArray();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}