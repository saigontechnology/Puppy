namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Queries
{
    /// <summary>
    ///     A family of match queries that accept text/numerics/dates, analyzes it, and constructs a query out of it. For
    ///     example:
    /// </summary>
    public class MatchQuery : MatchBase, IQuery
    {
        private readonly string _field;

        public MatchQuery(string field, string text) : base(text)
        {
            _field = field;
        }

        //{
        // "query" : {
        //	  "match" : {
        //		"name" : {
        //			"query" : "group"
        //		}
        //	  }
        //  }
        //}
        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("match");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_field);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            WriteBasePropertiesJson(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}