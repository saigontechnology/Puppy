using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Queries
{
    /// <summary>
    ///     The has_child filter accepts a query and the child type to run against, and results in
    ///     parent documents that have child docs matching the query. The type is the child type to
    ///     query against. The parent type to return is automatically detected based on the mappings.
    ///     The way that the filter is implemented is by first running the child query, doing the
    ///     matching up to the parent doc for each document matched. The has_child filter allows you
    ///     to specify that a minimum and/or maximum number of children are required to match for the
    ///     parent doc to be considered a match:
    /// </summary>
    public class HasChildQuery : IQuery
    {
        private readonly IQuery _query;
        private readonly string _type;
        private InnerHits _innerHits;
        private bool _innerHitsSet;
        private uint _maxChildren;
        private bool _maxChildrenSet;
        private uint _minChildren;
        private bool _minChildrenSet;
        private ScoreMode _scoreMode;
        private bool _scoreModeSet;

        public HasChildQuery(string type, IQuery query)
        {
            _type = type;
            _query = query;
        }

        /// <summary>
        ///     min_children 
        /// </summary>
        public uint MinChildren
        {
            get => _minChildren;
            set
            {
                _minChildren = value;
                _minChildrenSet = true;
            }
        }

        /// <summary>
        ///     max_children 
        /// </summary>
        public uint MaxChildren
        {
            get => _maxChildren;
            set
            {
                _maxChildren = value;
                _maxChildrenSet = true;
            }
        }

        /// <summary>
        ///     score_mode The score_mode allows to set how inner children matching affects scoring
        ///     of parent. It defaults to avg, but can be sum, max and none.
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
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("has_child");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("type", _type, elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("query");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            _query.WriteJson(elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();

            JsonHelper.WriteValue("min_children", _minChildren, elasticCrudJsonWriter, _minChildrenSet);
            JsonHelper.WriteValue("max_children", _maxChildren, elasticCrudJsonWriter, _maxChildrenSet);
            JsonHelper.WriteValue("score_mode", _scoreMode.ToString(), elasticCrudJsonWriter, _scoreModeSet);

            if (_innerHitsSet)
                _innerHits.WriteJson(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}