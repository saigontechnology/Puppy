using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Queries
{
	/// <summary>
	///     Matches spans near the beginning of a field. The span first query maps to Lucene SpanFirstQuery. The match clause can be any other span type query. The end controls the maximum end position permitted in a match. 
	/// </summary>
	public class SpanFirstQuery : IQuery
    {
        private readonly int _end;
        private readonly ISpanQuery _spanQuery;

        public SpanFirstQuery(ISpanQuery spanQuery, int end)
        {
            _spanQuery = spanQuery;
            _end = end;
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("span_first");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName("match");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            _spanQuery.WriteJson(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();

            JsonHelper.WriteValue("end", _end, elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}