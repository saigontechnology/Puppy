using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Queries
{
    /// <summary>
    ///   Nested query allows to query nested objects / docs (see nested mapping). The query is executed against the nested objects / docs as if they
    ///   were indexed as separate docs (they are, internally) and resulting in the root parent doc (or parent nested mapping).
    /// </summary>
    public class NestedQuery : IQuery
    {
        private readonly string _path;
        private readonly IQuery _query;
        private InnerHits _innerHits;
        private bool _innerHitsSet;
        private ScoreMode _scoreMode;
        private bool _scoreModeSet;

        public NestedQuery(IQuery query, string path)
        {
            _query = query;
            _path = path;
        }

        /// <summary>
        ///   score_mode The score_mode allows to set how inner children matching affects scoring of parent. It defaults to avg, but can be sum, max
        ///   and none.
        /// </summary>
        public ScoreMode ScoreMode
        {
            get => _scoreMode;
            set
            {
                _scoreMode = value;
                _scoreModeSet = true;
            }
        }

        public InnerHits InnerHits
        {
            get => _innerHits;
            set
            {
                _innerHits = value;
                _innerHitsSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("nested");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("path", _path, elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("query");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            _query.WriteJson(elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();

            JsonHelper.WriteValue("score_mode", _scoreMode.ToString(), elasticCrudJsonWriter, _scoreModeSet);

            if (_innerHitsSet)
                _innerHits.WriteJson(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}