using Puppy.Search.Elastic.Utils;
using System.Collections.Generic;

namespace Puppy.Search.Elastic.Model.SearchModel.Queries
{
    /// <summary>
    ///     A query that matches documents matching boolean combinations of other queries. The bool
    ///     query maps to Lucene BooleanQuery. It is built using one or more boolean clauses, each
    ///     clause with a typed occurrence. { "query":{ "bool" : { "must" : [ { "term" : { "details"
    ///     : "different" } }, { "term" : { "details" : "data" } } ], "must_not" : [ { "range" : {
    ///     "id" : { "from" : 7, "to" : 20 } } } ], "should" : [ { "term" : { "details" : "data" } },
    ///     { "term" : { "details" : "alone" } } ], "minimum_should_match" : 1, "boost" : 3.0 } } }
    /// </summary>
    public class BoolQuery : IQuery
    {
        private double _boost;
        private bool _boostSet;
        private bool _disableCoord;
        private bool _disableCoordSet;
        private string _minimumShouldMatch;
        private bool _minimumShouldMatchSet;
        private List<IQuery> _must;
        private List<IQuery> _mustNot;
        private bool _mustNotSet;
        private bool _mustSet;
        private List<IQuery> _should;
        private bool _shouldSet;

        public BoolQuery()
        {
        }

        public BoolQuery(IQuery must, IQuery mustNot = null)
        {
            Must = new List<IQuery> { must };

            if (mustNot != null)
                MustNot = new List<IQuery> { mustNot };
        }

        public List<IQuery> Must
        {
            get => _must;
            set
            {
                _must = value;
                _mustSet = true;
            }
        }

        public List<IQuery> MustNot
        {
            get => _must;
            set
            {
                _mustNot = value;
                _mustNotSet = true;
            }
        }

        public List<IQuery> Should
        {
            get => _should;
            set
            {
                _should = value;
                _shouldSet = true;
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

        /// <summary>
        ///     minimum_should_match The minimum_should_match parameter possible values: Integer :
        ///     Indicates a fixed value regardless of the number of optional clauses. Negative
        ///     integer : Indicates that the total number of optional clauses, minus this number
        ///     should be mandatory. Percentage 75% : Indicates that this percent of the total number
        ///     of optional clauses are necessary. The number computed from the percentage is rounded
        ///     down and used as the minimum. Negative percentage -25% Indicates that this percent of
        ///     the total number of optional clauses can be missing. The number computed from the
        ///     percentage is rounded down, before being subtracted from the total to determine the
        ///     minimum. Combination : A positive integer, followed by the less-than symbol, followed
        ///     by any of the previously mentioned specifiers is a conditional specification. It
        ///     indicates that if the number of optional clauses is equal to (or less than) the
        ///     integer, they are all required, but if it’s greater than the integer, the
        ///     specification applies. In this example: if there are 1 to 3 clauses they are all
        ///     required, but for 4 or more clauses only 90% are required. Multiple combinations :
        ///     Multiple conditional specifications can be separated by spaces, each one only being
        ///     valid for numbers greater than the one before it. In this example: if there are 1 or
        ///     2 clauses both are required, if there are 3-9 clauses all but 25% are required, and
        ///     if there are more than 9 clauses, all but three are required. NOTE: When dealing with
        ///     percentages, negative values can be used to get different behavior in edge cases. 75%
        ///     and -25% mean the same thing when dealing with 4 clauses, but when dealing with 5
        ///     clauses 75% means 3 are required, but
        ///     -25% means 4 are required. If the calculations based on the specification determine
        ///      that no optional clauses are needed, the usual rules about BooleanQueries still
        ///      apply at search time (a BooleanQuery containing no required clauses must still match
        ///      at least one optional
        ///     clause) No matter what number the calculation arrives at, a value greater than the
        ///             number of optional clauses, or a value less than 1 will never be used. (ie:
        ///             no matter how low or how high the result of the calculation result is, the
        ///             minimum number of required matches will never be lower than 1 or greater than
        ///             the number of clauses.
        /// </summary>
        public string MinimumShouldMatch
        {
            get => _minimumShouldMatch;
            set
            {
                _minimumShouldMatch = value;
                _minimumShouldMatchSet = true;
            }
        }

        /// <summary>
        ///     disable_coord The bool query also supports disable_coord parameter (defaults to
        ///     false). Basically the coord similarity computes a score factor based on the fraction
        ///     of all query terms that a document contains.
        /// </summary>
        public bool DisableCoord
        {
            get => _disableCoord;
            set
            {
                _disableCoord = value;
                _disableCoordSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("bool");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            WriteMustQueryList(elasticCrudJsonWriter);
            WriteMustNotQueryList(elasticCrudJsonWriter);
            WriteShouldQueryList(elasticCrudJsonWriter);

            JsonHelper.WriteValue("boost", _boost, elasticCrudJsonWriter, _boostSet);
            JsonHelper.WriteValue("minimum_should_match", _minimumShouldMatch, elasticCrudJsonWriter,
                _minimumShouldMatchSet);
            JsonHelper.WriteValue("disable_coord", _disableCoord, elasticCrudJsonWriter, _disableCoordSet);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }

        private void WriteShouldQueryList(ElasticJsonWriter elasticCrudJsonWriter)
        {
            if (_shouldSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("should");
                elasticCrudJsonWriter.JsonWriter.WriteStartArray();

                foreach (var shouldItem in _should)
                {
                    elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                    shouldItem.WriteJson(elasticCrudJsonWriter);
                    elasticCrudJsonWriter.JsonWriter.WriteEndObject();
                }

                elasticCrudJsonWriter.JsonWriter.WriteEndArray();
            }
        }

        private void WriteMustNotQueryList(ElasticJsonWriter elasticCrudJsonWriter)
        {
            if (_mustNotSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("must_not");
                elasticCrudJsonWriter.JsonWriter.WriteStartArray();

                foreach (var mustNotItem in _mustNot)
                {
                    elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                    mustNotItem.WriteJson(elasticCrudJsonWriter);
                    elasticCrudJsonWriter.JsonWriter.WriteEndObject();
                }

                elasticCrudJsonWriter.JsonWriter.WriteEndArray();
            }
        }

        private void WriteMustQueryList(ElasticJsonWriter elasticCrudJsonWriter)
        {
            if (_mustSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("must");
                elasticCrudJsonWriter.JsonWriter.WriteStartArray();

                foreach (var mustItem in _must)
                {
                    elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                    mustItem.WriteJson(elasticCrudJsonWriter);
                    elasticCrudJsonWriter.JsonWriter.WriteEndObject();
                }

                elasticCrudJsonWriter.JsonWriter.WriteEndArray();
            }
        }
    }
}