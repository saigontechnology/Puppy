using Puppy.Elastic.Model;
using Puppy.Elastic.Utils;
using System.Collections.Generic;

namespace Puppy.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
    public class KeywordMarkerTokenFilter : AnalysisFilterBase
    {
        private bool _ignoreCase;
        private bool _ignoreCaseSet;
        private List<string> _keywords;
        private string _keywordsPath;
        private bool _keywordsPathSet;
        private bool _keywordsSet;

        /// <summary>
        ///     Protects words from being modified by stemmers. Must be placed before any stemming filters.
        /// </summary>
        /// <param name="name"> name for the custom filter </param>
        public KeywordMarkerTokenFilter(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenFilters.KeywordMarker;
        }

        /// <summary>
        ///     keywords A list of mapping rules to use. 
        /// </summary>
        public List<string> Keywords
        {
            get => _keywords;
            set
            {
                _keywords = value;
                _keywordsSet = true;
            }
        }

        /// <summary>
        ///     rules_path A path (either relative to config location, or absolute) to a list of words.
        /// </summary>
        public string KeywordsPath
        {
            get => _keywordsPath;
            set
            {
                _keywordsPath = value;
                _keywordsPathSet = true;
            }
        }

        /// <summary>
        ///     ignore_case Set to true to lower case all words first. Defaults to false. 
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

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteListValue("keywords", _keywords, elasticCrudJsonWriter, _keywordsSet);
            JsonHelper.WriteValue("keywords_path", _keywordsPath, elasticCrudJsonWriter, _keywordsPathSet);
            JsonHelper.WriteValue("ignore_case", _ignoreCase, elasticCrudJsonWriter, _ignoreCaseSet);
        }
    }
}