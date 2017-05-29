using Puppy.Search.Elastic.Model;
using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
    public class PatternReplaceTokenFilter : AnalysisFilterBase
    {
        private string _pattern;
        private bool _patternSet;
        private string _replacement;
        private bool _replacementSet;

        /// <summary>
        ///     The pattern_replace token filter allows to easily handle string replacements based on
        ///     a regular expression. The regular expression is defined using the pattern parameter,
        ///     and the replacement string can be provided using the replacement parameter
        /// </summary>
        /// <param name="name"> name for the custom filter </param>
        public PatternReplaceTokenFilter(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenFilters.PatternReplace;
        }

        public string Pattern
        {
            get => _pattern;
            set
            {
                _pattern = value;
                _patternSet = true;
            }
        }

        public string Replacement
        {
            get => _replacement;
            set
            {
                _replacement = value;
                _replacementSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteValue("pattern", _pattern, elasticCrudJsonWriter, _patternSet);
            JsonHelper.WriteValue("replacement", _replacement, elasticCrudJsonWriter, _replacementSet);
        }
    }
}