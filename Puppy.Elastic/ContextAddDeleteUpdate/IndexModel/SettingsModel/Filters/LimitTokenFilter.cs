using Puppy.Elastic.Model;
using Puppy.Elastic.Utils;

namespace Puppy.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
    public class LimitTokenFilter : AnalysisFilterBase
    {
        private bool _consumeAllTokens;
        private bool _consumeAllTokensSet;
        private int _maxTokenCount;
        private bool _maxTokenCountSet;

        /// <summary>
        ///     Limits the number of tokens that are indexed per document and field. 
        /// </summary>
        /// <param name="name"> name for the custom filter </param>
        public LimitTokenFilter(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenFilters.Limit;
        }

        /// <summary>
        ///     max_token_count The maximum number of tokens that should be indexed per document and
        ///     field. The default is 1
        /// </summary>
        public int MaxTokenCount
        {
            get => _maxTokenCount;
            set
            {
                _maxTokenCount = value;
                _maxTokenCountSet = true;
            }
        }

        /// <summary>
        ///     consume_all_tokens If set to true the filter exhaust the stream even if
        ///     max_token_count tokens have been consumed already. The default is false.
        /// </summary>
        public bool ConsumeAllTokens
        {
            get => _consumeAllTokens;
            set
            {
                _consumeAllTokens = value;
                _consumeAllTokensSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteValue("max_token_count", _maxTokenCount, elasticCrudJsonWriter, _maxTokenCountSet);
            JsonHelper.WriteValue("consume_all_tokens", _consumeAllTokens, elasticCrudJsonWriter, _consumeAllTokensSet);
        }
    }
}