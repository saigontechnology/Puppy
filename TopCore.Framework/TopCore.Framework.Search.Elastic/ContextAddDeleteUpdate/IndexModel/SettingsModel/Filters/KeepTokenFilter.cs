using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
    public class KeepTokenFilter : AnalysisFilterBase
    {
        private List<string> _keepWords;
        private bool _keepWordsCase;
        private bool _keepWordsCaseSet;
        private string _keepWordsPath;
        private bool _keepWordsPathSet;
        private bool _keepWordsSet;

        /// <summary>
        ///     A token filter of type keep that only keeps tokens with text contained in a predefined set of words. The set of
        ///     words can be defined in the settings or loaded from a text file containing one word per line.
        /// </summary>
        /// <param name="name"> name for the custom filter </param>
        public KeepTokenFilter(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenFilters.Keep;
        }

        /// <summary>
        ///     keep_words a list of words to keep
        /// </summary>
        public List<string> KeepWords
        {
            get => _keepWords;
            set
            {
                _keepWords = value;
                _keepWordsSet = true;
            }
        }

        /// <summary>
        ///     keep_words_path a path to a words file
        /// </summary>
        public string KeepWordsPath
        {
            get => _keepWordsPath;
            set
            {
                _keepWordsPath = value;
                _keepWordsPathSet = true;
            }
        }

        /// <summary>
        ///     keep_words_case a boolean indicating whether to lower case the words (defaults to false)
        /// </summary>
        public bool KeepWordsCase
        {
            get => _keepWordsCase;
            set
            {
                _keepWordsCase = value;
                _keepWordsCaseSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteListValue("keep_words", _keepWords, elasticCrudJsonWriter, _keepWordsSet);
            JsonHelper.WriteValue("keep_words_path", _keepWordsPath, elasticCrudJsonWriter, _keepWordsPathSet);
            JsonHelper.WriteValue("keep_words_case", _keepWordsCase, elasticCrudJsonWriter, _keepWordsCaseSet);
        }
    }
}