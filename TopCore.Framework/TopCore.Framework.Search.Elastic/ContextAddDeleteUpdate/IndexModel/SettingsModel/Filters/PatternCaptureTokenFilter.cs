using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
    public class PatternCaptureTokenFilter : AnalysisFilterBase
    {
        private List<string> _patterns;
        private bool _patternsSet;
        private bool _preserveOriginal;
        private bool _preserveOriginalSet;

        /// <summary>
        ///     The pattern_capture token filter, unlike the pattern tokenizer, emits a token for every capture group in the
        ///     regular expression. Patterns are not anchored to the beginning and end of the string, so each pattern can match
        ///     multiple times, and matches are allowed to overlap.
        /// </summary>
        /// <param name="name"> name for the custom filter </param>
        public PatternCaptureTokenFilter(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenFilters.PatternCapture;
        }

        public List<string> Patterns
        {
            get => _patterns;
            set
            {
                _patterns = value;
                _patternsSet = true;
            }
        }

        /// <summary>
        ///     preserve_original
        /// </summary>
        public bool PreserveOriginal
        {
            get => _preserveOriginal;
            set
            {
                _preserveOriginal = value;
                _preserveOriginalSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteListValue("patterns", _patterns, elasticCrudJsonWriter, _patternsSet);
            JsonHelper.WriteValue("preserve_original", _preserveOriginal, elasticCrudJsonWriter, _preserveOriginalSet);
        }
    }
}