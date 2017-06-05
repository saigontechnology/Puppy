using Puppy.Search.Elastic.Utils;
using System.Collections.Generic;

namespace Puppy.Search.Elastic.Model.SearchModel.Queries.FunctionQuery
{
    /// <summary>
    ///     The function_score allows you to modify the score of documents that are retrieved by a
    ///     query. This can be useful if, for example, a score function is computationally expensive
    ///     and it is sufficient to compute the score on a filtered set of documents. function_score
    ///     provides the same functionality that custom_boost_factor, custom_score and
    ///     custom_filters_score provided but with additional capabilities such as distance and
    ///     recency scoring
    /// </summary>
    public class FunctionScoreQuery : IQuery
    {
        private readonly IFilter _filter;
        private readonly bool _filterSet;
        private readonly List<BaseScoreFunction> _functions;
        private readonly IQuery _query;
        private readonly bool _querySet;
        private double _boost;
        private FunctionScoreQueryBoostMode _boostMode;
        private bool _boostModeSet;
        private bool _boostSet;
        private double _maxBoost;
        private bool _maxBoostSet;
        private FunctionScoreQueryScoreMode _scoreMode;
        private bool _scoreModeSet;

        public FunctionScoreQuery(IQuery query, List<BaseScoreFunction> functions)
        {
            _query = query;
            _functions = functions;
            _querySet = true;
        }

        public FunctionScoreQuery(IFilter filter, List<BaseScoreFunction> functions)
        {
            _filter = filter;
            _functions = functions;
            _filterSet = true;
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

        public double MaxBoost
        {
            get => _maxBoost;
            set
            {
                _maxBoost = value;
                _maxBoostSet = true;
            }
        }

        /// <summary>
        ///     score_mode If no filter is given with a function this is equivalent to specifying
        ///     "match_all": {} First, each document is scored by the defined functions. The
        ///     parameter score_mode specifies how the computed scores are combined. Because scores
        ///     can be on different scales (for example, between 0 and 1 for decay functions but
        ///     arbitrary for field_value_factor) and also because sometimes a different impact of
        ///     functions on the score is desirable, the score of each function can be adjusted with
        ///     a user defined weight ( [1.4.0.Beta1] Added in 1.4.0.Beta1.). The weight can be
        ///     defined per function in the functions array (example above) and is multiplied with
        ///     the score computed by the respective function. If weight is given without any other
        ///     function declaration, weight acts as a function that simply returns the weight.
        /// </summary>
        public FunctionScoreQueryScoreMode ScoreMode
        {
            get => _scoreMode;
            set
            {
                _scoreMode = value;
                _scoreModeSet = true;
            }
        }

        /// <summary>
        ///     The new score can be restricted to not exceed a certain limit by setting the
        ///     max_boost parameter. The default for max_boost is FLT_MAX. Finally, the newly
        ///     computed score is combined with the score of the query. The parameter boost_mode
        ///     defines how.
        /// </summary>
        public FunctionScoreQueryBoostMode BoostMode
        {
            get => _boostMode;
            set
            {
                _boostMode = value;
                _boostModeSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("function_score");
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

            elasticCrudJsonWriter.JsonWriter.WritePropertyName("functions");
            elasticCrudJsonWriter.JsonWriter.WriteStartArray();
            foreach (var function in _functions)
            {
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                function.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }
            elasticCrudJsonWriter.JsonWriter.WriteEndArray();

            JsonHelper.WriteValue("max_boost", _maxBoost, elasticCrudJsonWriter, _maxBoostSet);
            JsonHelper.WriteValue("score_mode", _scoreMode.ToString(), elasticCrudJsonWriter, _scoreModeSet);
            JsonHelper.WriteValue("boost_mode", _boostMode.ToString(), elasticCrudJsonWriter, _boostModeSet);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}