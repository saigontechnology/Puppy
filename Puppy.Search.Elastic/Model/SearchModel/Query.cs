using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.Model.SearchModel
{
    /// <summary>
    ///     As a general rule, queries should be used instead of filters:
    ///     - for full text search
    ///     - where the result depends on a relevance score
    /// </summary>
    public class Query : IQueryHolder
    {
        private readonly IQuery _query;
        private string _name;
        private bool _nameSet;

        public Query(IQuery query)
        {
            _query = query;
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                _nameSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("query");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            _query.WriteJson(elasticCrudJsonWriter);
            JsonHelper.WriteValue("_name", _name, elasticCrudJsonWriter, _nameSet);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }

        public override string ToString()
        {
            var elasticCrudJsonWriter = new ElasticJsonWriter();
            WriteJson(elasticCrudJsonWriter);
            return elasticCrudJsonWriter.GetJsonString();
        }
    }
}