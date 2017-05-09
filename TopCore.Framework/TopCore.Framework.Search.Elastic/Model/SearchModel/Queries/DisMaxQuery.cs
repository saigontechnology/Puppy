using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Queries
{
    /// <summary>
    ///     A query that generates the union of documents produced by its subqueries, and that scores each document with the maximum score for that document as produced by any subquery, plus a tie breaking increment for any additional matching subqueries. This is useful when searching for a word in
    ///     multiple fields with different boost factors (so that the fields cannot be combined equivalently into a single search field). We want the primary score to be the one associated with the highest boost, not the sum of the field scores (as Boolean Query would give). If the query is "albino
    ///     elephant" this ensures that "albino" matching one field and "elephant" matching another gets a higher score than "albino" matching both fields. To get this result, use both Boolean Query and DisjunctionMax Query: for each term a DisjunctionMaxQuery searches for it in each field, while the
    ///     set of these DisjunctionMaxQuery’s is combined into a BooleanQuery. The tie breaker capability allows results that include the same term in multiple fields to be judged better than results that include this term in only the best of those multiple fields, without confusing this with the
    ///     better case of two different terms in the multiple fields.The default tie_breaker is 0.0. This query maps to Lucene DisjunctionMaxQuery { "dis_max" : { "tie_breaker" : 0.7, "boost" : 1.2, "queries" : [ { "term" : { "age" : 34 } }, { "term" : { "age" : 35 } } ] } }
    /// </summary>
    public class DisMaxQuery : IQuery
    {
        private double _boost;
        private bool _boostSet;
        private List<IQuery> _queries;
        private bool _queriesSet;
        private double _tieBreaker;
        private bool _tieBreakerSet;

        public double Boost
        {
            get => _boost;
            set
            {
                _boost = value;
                _boostSet = true;
            }
        }

        /// <summary>
        ///     tie_breaker By default, each per-term blended query will use the best score returned by any field in a group, then these scores are added together to give the final score. The tie_breaker parameter can change the default behaviour of the per-term blended queries. It accepts: 0.0 Take
        ///     the single best score out of (eg) first_name:will and last_name:will (default) 1.0 Add together the scores for (eg) first_name:will and last_name:will 0.0 between 1.0 Take the single best score plus tie_breaker multiplied by each of the scores from other matching fields.
        /// </summary>
        public double TieBreaker
        {
            get => _tieBreaker;
            set
            {
                _tieBreaker = value;
                if (value < 0) throw new ElasticException("TieBreakermust be larger than 0");
                if (value > 1) throw new ElasticException("TieBreaker must be equal or smaller than 1.0");
                _tieBreakerSet = true;
            }
        }

        public List<IQuery> Queries
        {
            get => _queries;
            set
            {
                _queries = value;
                _queriesSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("dis_max");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("tie_breaker", _tieBreaker, elasticCrudJsonWriter, _tieBreakerSet);
            JsonHelper.WriteValue("boost", _boost, elasticCrudJsonWriter, _boostSet);
            WriteQueriesList(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }

        private void WriteQueriesList(ElasticJsonWriter elasticCrudJsonWriter)
        {
            if (_queriesSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("queries");
                elasticCrudJsonWriter.JsonWriter.WriteStartArray();

                foreach (var shouldItem in _queries)
                {
                    elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                    shouldItem.WriteJson(elasticCrudJsonWriter);
                    elasticCrudJsonWriter.JsonWriter.WriteEndObject();
                }

                elasticCrudJsonWriter.JsonWriter.WriteEndArray();
            }
        }
    }
}