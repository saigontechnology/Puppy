using System;
using System.Collections;
using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers
{
    /// <summary>
    ///     The standard analyzer, which is the default analyzer used for full-text fields, is a good choice for most Western languages. It consists of the following: The standard tokenizer, which splits the input text on word boundaries The standard token filter, which is intended to tidy up the
    ///     tokens emitted by the tokenizer (but currently does nothing) The lowercase token filter, which converts all tokens into lowercase The stop token filter, which removes stopwords—common words that have little impact on search relevance, such as a, the, and, is. By default, the stopwords
    ///     filter is disabled. You can enable it by creating a custom analyzer based on the standard analyzer and setting the stopwords parameter. Either provide a list of stopwords or tell it to use a predefined stopwords list from a particular language. "analyzer" : { "blocks_analyzer" : { "type"
    ///     : "custom", "tokenizer" : "whitespace", "filter" : ["lowercase", "blocks_filter", "shingle"] } }
    /// </summary>
    public class CustomAnalyzer : AnalyzerBase
    {
        private List<string> _charFilter;
        private bool _charFilterSet;
        private List<string> _filter;
        private bool _filterSet;

        public CustomAnalyzer(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultAnalyzers.Custom;
        }

        public List<string> Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                _filterSet = true;
            }
        }

        public List<string> CharFilter
        {
            get => _charFilter;
            set
            {
                _charFilter = value;
                _charFilterSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteSpecificJson);
        }

        private void WriteSpecificJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            if (AnalyzerSet)
            {
                JsonHelper.WriteListValue("filter", _filter, elasticCrudJsonWriter, _filterSet);
                JsonHelper.WriteListValue("char_filter", _charFilter, elasticCrudJsonWriter, _charFilterSet);
            }
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}