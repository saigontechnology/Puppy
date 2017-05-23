namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Queries
{
    /// <summary>
    ///     The span_multi query allows you to wrap a multi term query (one of fuzzy, prefix, term
    ///     range or regexp query) as a span query, so it can be nested. Example:
    /// </summary>
    public class SpanMultiQuery : ISpanQuery
    {
        private readonly IQuery _query;

        public SpanMultiQuery(FuzzyQuery query)
        {
            _query = query;
        }

        public SpanMultiQuery(PrefixQuery query)
        {
            _query = query;
        }

        public SpanMultiQuery(RegExpQuery query)
        {
            _query = query;
        }

        public SpanMultiQuery(RangeQuery query)
        {
            _query = query;
        }

        public SpanMultiQuery(WildcardQuery query)
        {
            _query = query;
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("span_multi");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName("match");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            _query.WriteJson(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}