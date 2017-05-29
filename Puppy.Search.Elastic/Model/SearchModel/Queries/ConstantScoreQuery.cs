using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.Model.SearchModel.Queries
{
    /// <summary>
    ///     A query that wraps a filter or another query and simply returns a constant score equal to
    ///     the query boost for every document in the filter. Maps to Lucene ConstantScoreQuery. The
    ///     filter object can hold only filter elements, not queries. Filters can be much faster
    ///     compared to queries since they don’t perform any scoring, especially when they are cached.
    /// </summary>
    public class ConstantScoreQuery : IQuery
    {
        private double _boost;
        private bool _boostSet;
        private IFilter _filter;
        private bool _filterSet;
        private IQuery _query;
        private bool _querySet;

        public ConstantScoreQuery(IQuery query)
        {
            Query = query;
        }

        public ConstantScoreQuery(IFilter filter)
        {
            Filter = filter;
        }

        /// <summary>
        ///     positive 
        /// </summary>
        public IQuery Query
        {
            get => _query;
            set
            {
                _query = value;
                _querySet = true;
            }
        }

        public IFilter Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                _filterSet = true;
            }
        }

        public double Boost
        {
            get => _boost;
            set
            {
                _boost = value;
                _boostSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("constant_score");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            if (_querySet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("query");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                _query.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }

            if (_filterSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("filter");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                _filter.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }

            JsonHelper.WriteValue("boost", _boost, elasticCrudJsonWriter, _boostSet);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}