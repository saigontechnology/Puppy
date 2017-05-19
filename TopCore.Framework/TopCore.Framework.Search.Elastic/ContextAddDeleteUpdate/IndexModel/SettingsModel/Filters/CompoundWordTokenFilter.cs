using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
    public class DictionaryDecompounderTokenFilter : CompoundWordTokenFilter
    {
        public DictionaryDecompounderTokenFilter(string name) : base(name, DefaultTokenFilters.DictionaryDecompounder)
        {
        }
    }

    public class HyphenationDecompounderTokenFilter : CompoundWordTokenFilter
    {
        public HyphenationDecompounderTokenFilter(string name)
            : base(name, DefaultTokenFilters.HyphenationDecompounder)
        {
        }
    }

    public abstract class CompoundWordTokenFilter : AnalysisFilterBase
    {
        private string _hyphenationPatternsPath;
        private bool _hyphenationPatternsPathSet;
        private int _maxSubwordSize;
        private bool _maxSubwordSizeSet;
        private int _minSubwordSize;
        private bool _minSubwordSizeSet;
        private int _minWordSize;
        private bool _minWordSizeSet;
        private bool _onlyLongestMatch;
        private bool _onlyLongestMatchSet;
        private List<string> _wordList;
        private string _wordListPath;
        private bool _wordListPathSet;
        private bool _wordListSet;

        protected CompoundWordTokenFilter(string name, string type)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = type;
        }

        /// <summary>
        ///   word_list A list of words to use. 
        /// </summary>
        public List<string> WordList
        {
            get => _wordList;
            set
            {
                _wordList = value;
                _wordListSet = true;
            }
        }

        /// <summary>
        ///   word_list_path A path (either relative to config location, or absolute) to a list of words. 
        /// </summary>
        public string WordListPath
        {
            get => _wordListPath;
            set
            {
                _wordListPath = value;
                _wordListPathSet = true;
            }
        }

        /// <summary>
        ///   hyphenation_patterns_path A path (either relative to config location, or absolute) to a FOP XML hyphenation pattern file. (See
        ///   http://offo.sourceforge.net/hyphenation/) Required for hyphenation_decompounder.
        /// </summary>
        public string HyphenationPatternsPath
        {
            get => _hyphenationPatternsPath;
            set
            {
                _hyphenationPatternsPath = value;
                _hyphenationPatternsPathSet = true;
            }
        }

        /// <summary>
        ///   min_word_size Minimum word size(Integer). Defaults to 5. 
        /// </summary>
        public int MinWordSize
        {
            get => _minWordSize;
            set
            {
                _minWordSize = value;
                _minWordSizeSet = true;
            }
        }

        /// <summary>
        ///   min_subword_size Minimum subword size(Integer). Defaults to 2. 
        /// </summary>
        public int MinSubwordSize
        {
            get => _minSubwordSize;
            set
            {
                _minSubwordSize = value;
                _minSubwordSizeSet = true;
            }
        }

        /// <summary>
        ///   max_subword_size Maximum subword size(Integer). Defaults to 15. 
        /// </summary>
        public int MaxSubwordSize
        {
            get => _maxSubwordSize;
            set
            {
                _maxSubwordSize = value;
                _maxSubwordSizeSet = true;
            }
        }

        /// <summary>
        ///   only_longest_match Only matching the longest(Boolean). Defaults to false 
        /// </summary>
        public bool OnlyLongestMatch
        {
            get => _onlyLongestMatch;
            set
            {
                _onlyLongestMatch = value;
                _onlyLongestMatchSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteListValue("word_list", _wordList, elasticCrudJsonWriter, _wordListSet);
            JsonHelper.WriteValue("word_list_path", _wordListPath, elasticCrudJsonWriter, _wordListPathSet);
            JsonHelper.WriteValue("hyphenation_patterns_path", _hyphenationPatternsPath, elasticCrudJsonWriter,
                _hyphenationPatternsPathSet);
            JsonHelper.WriteValue("min_word_size", _minWordSize, elasticCrudJsonWriter, _minWordSizeSet);
            JsonHelper.WriteValue("min_subword_size", _minSubwordSize, elasticCrudJsonWriter, _minSubwordSizeSet);
            JsonHelper.WriteValue("max_subword_size", _maxSubwordSize, elasticCrudJsonWriter, _maxSubwordSizeSet);
            JsonHelper.WriteValue("only_longest_match", _onlyLongestMatch, elasticCrudJsonWriter, _onlyLongestMatchSet);
        }
    }
}