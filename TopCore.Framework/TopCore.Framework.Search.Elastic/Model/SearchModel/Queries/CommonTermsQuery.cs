using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Queries
{
    /// <summary>
    ///   The common terms query is a modern alternative to stopwords which improves the precision and recall of search results (by taking stopwords
    ///   into account), without sacrificing performance. The problem Every term in a query has a cost. A search for "The brown fox" requires three
    ///   term queries, one for each of "the", "brown" and "fox", all of which are executed against all documents in the index. The query for "the" is
    ///   likely to match many documents and thus has a much smaller impact on relevance than the other two terms. Previously, the solution to this
    ///   problem was to ignore terms with high frequency. By treating "the" as a stopword, we reduce the index size and reduce the number of term
    ///   queries that need to be executed. The problem with this approach is that, while stopwords have a small impact on relevance, they are still
    ///   important. If we remove stopwords, we lose precision, (eg we are unable to distinguish between "happy" and "not happy") and we lose recall
    ///   (eg text like "The The" or "To be or not to be" would simply not exist in the index). The solution The common terms query divides the query
    ///   terms into two groups: more important (ie low frequency terms) and less important (ie high frequency terms which would previously have been
    ///   stopwords). First it searches for documents which match the more important terms. These are the terms which appear in fewer documents and
    ///   have a greater impact on relevance. Then, it executes a second query for the less important terms — terms which appear frequently and have a
    ///   low impact on relevance. But instead of calculating the relevance score for all matching documents, it only calculates the _score for
    ///   documents already matched by the first query. In this way the high frequency terms can improve the relevance calculation without paying the
    ///   cost of poor performance. If a query consists only of high frequency terms, then a single query is executed as an AND (conjunction) query, in
    ///   other words all terms are required. Even though each individual term will match many documents, the combination of terms narrows down the
    ///   resultset to only the most relevant. The single query can also be executed as an OR with a specific minimum_should_match, in this case a high
    ///   enough value should probably be used. Terms are allocated to the high or low frequency groups based on the cutoff_frequency, which can be
    ///   specified as an absolute frequency ( &gt;=1) or as a relative frequency (0.0 .. 1.0). Perhaps the most interesting property of this query is
    ///   that it adapts to domain specific stopwords automatically. For example, on a video hosting site, common terms like "clip" or "video" will
    ///   automatically behave as stopwords without the need to maintain a manual list.
    /// </summary>
    public class CommonTermsQuery : IQuery
    {
        private readonly double _cutOffFrequency;
        private readonly string _field;
        private readonly string _query;
        private uint _highFreq;
        private QueryDefaultOperator _highFreqOperator;
        private bool _highFreqOperatorSet;
        private bool _highFreqSet;
        private uint _lowFreq;
        private QueryDefaultOperator _lowFreqOperator;
        private bool _lowFreqOperatorSet;
        private bool _lowFreqSet;
        private uint _minimumShouldMatch;
        private bool _minimumShouldMatchSet;

        public CommonTermsQuery(string field, string query, double cutOffFrequency)
        {
            _query = query;
            _field = field;
            _cutOffFrequency = cutOffFrequency;
        }

        /// <summary>
        ///   low_freq_operator 
        /// </summary>
        public QueryDefaultOperator LowFreqOperator
        {
            get => _lowFreqOperator;
            set
            {
                _lowFreqOperator = value;
                _lowFreqOperatorSet = true;
            }
        }

        /// <summary>
        ///   high_freq_operator 
        /// </summary>
        public QueryDefaultOperator HighFreqOperator
        {
            get => _highFreqOperator;
            set
            {
                _highFreqOperator = value;
                _highFreqOperatorSet = true;
            }
        }

        /// <summary>
        ///   minimum_should_match This can be set or the high_freq, low_freq values. If minimum_should_match is set, the other values will be ignored 
        /// </summary>
        public uint MinimumShouldMatch
        {
            get => _minimumShouldMatch;
            set
            {
                _minimumShouldMatch = value;
                _minimumShouldMatchSet = true;
            }
        }

        public uint LowFreq
        {
            get => _lowFreq;
            set
            {
                _lowFreq = value;
                _lowFreqSet = true;
            }
        }

        public uint HighFreq
        {
            get => _highFreq;
            set
            {
                _highFreq = value;
                _highFreqSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("common");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_field);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("query", _query, elasticCrudJsonWriter);
            JsonHelper.WriteValue("cutoff_frequency", _cutOffFrequency, elasticCrudJsonWriter);

            JsonHelper.WriteValue("low_freq_operator", _lowFreqOperator.ToString().ToLower(), elasticCrudJsonWriter,
                _lowFreqOperatorSet);
            JsonHelper.WriteValue("high_freq_operator", _highFreqOperator.ToString().ToLower(), elasticCrudJsonWriter,
                _highFreqOperatorSet);

            if (_minimumShouldMatchSet)
            {
                JsonHelper.WriteValue("minimum_should_match", _minimumShouldMatch, elasticCrudJsonWriter);
            }
            else if (_lowFreqSet || _highFreqSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("minimum_should_match");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                JsonHelper.WriteValue("low_freq", _lowFreq, elasticCrudJsonWriter, _lowFreqSet);
                JsonHelper.WriteValue("high_freq", _highFreq, elasticCrudJsonWriter, _highFreqSet);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}