using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
    public class CommonGramsFilter : AnalysisFilterBase
    {
        private List<string> _commonWords;
        private string _commonWordsPath;
        private bool _commonWordsPathSet;
        private bool _commonWordsSet;
        private bool _ignoreCase;
        private bool _ignoreCaseSet;
        private bool _queryMode;
        private bool _queryModeSet;

        /// <summary>
        ///     Token filter that generates bigrams for frequently occuring terms. Single terms are still indexed. It can be used
        ///     as an alternative to the Stop Token Filter when we don’t want to completely ignore common terms. For example, the
        ///     text "the quick brown is a fox" will be tokenized as
        ///     "the", "the_quick", "quick", "brown", "brown_is", "is_a", "a_fox", "fox". Assuming "the", "is" and "a" are common
        ///     words. When query_mode is enabled, the token filter removes common words and single terms followed by a common
        ///     word. This parameter should be enabled in the search
        ///     analyzer. For example, the query "the quick brown is a fox" will be tokenized as "the_quick", "quick", "brown_is",
        ///     "is_a", "a_fox", "fox".
        /// </summary>
        /// <param name="name"> name for the custom filter </param>
        public CommonGramsFilter(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenFilters.CommonGrams;
        }

        /// <summary>
        ///     common_words A list of common words to use.
        /// </summary>
        public List<string> CommonWords
        {
            get => _commonWords;
            set
            {
                _commonWords = value;
                _commonWordsSet = true;
            }
        }

        /// <summary>
        ///     common_words_path A path (either relative to config location, or absolute) to a list of common words. Each word
        ///     should be in its own "line" (separated by a line break). The file must be UTF-8 encoded.
        /// </summary>
        public string CommonWordsPath
        {
            get => _commonWordsPath;
            set
            {
                _commonWordsPath = value;
                _commonWordsPathSet = true;
            }
        }

        /// <summary>
        ///     ignore_case If true, common words matching will be case insensitive (defaults to false).
        /// </summary>
        public bool IgnoreCase
        {
            get => _ignoreCase;
            set
            {
                _ignoreCase = value;
                _ignoreCaseSet = true;
            }
        }

        /// <summary>
        ///     query_mode Generates bigrams then removes common words and single terms followed by a common word (defaults to
        ///     false).
        /// </summary>
        public bool QueryMode
        {
            get => _queryMode;
            set
            {
                _queryMode = value;
                _queryModeSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteListValue("common_words", _commonWords, elasticCrudJsonWriter, _commonWordsSet);
            JsonHelper.WriteValue("common_words_path", _commonWordsPath, elasticCrudJsonWriter, _commonWordsPathSet);
            JsonHelper.WriteValue("ignore_case", _ignoreCase, elasticCrudJsonWriter, _ignoreCaseSet);
            JsonHelper.WriteValue("query_mode", _queryMode, elasticCrudJsonWriter, _queryModeSet);
        }
    }
}