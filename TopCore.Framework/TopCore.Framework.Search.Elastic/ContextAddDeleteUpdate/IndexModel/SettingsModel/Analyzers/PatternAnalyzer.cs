using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers
{
    public class PatternAnalyzer : BaseStopAnalyzer
    {
        private string _flags;
        private bool _flagsSet;
        private bool _lowercase;
        private bool _lowercaseSet;
        private string _pattern;
        private bool _patternSet;

        public PatternAnalyzer(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultAnalyzers.Pattern;
        }

        /// <summary>
        ///     lowercase Should terms be lowercased or not. Defaults to true. 
        /// </summary>
        public bool Lowercase
        {
            get => _lowercase;
            set
            {
                _lowercase = value;
                _lowercaseSet = true;
            }
        }

        /// <summary>
        ///     pattern The regular expression pattern, defaults to \W+. 
        /// </summary>
        public string Pattern
        {
            get => _pattern;
            set
            {
                _pattern = value;
                _patternSet = true;
            }
        }

        /// <summary>
        ///     flags The regular expression flags.
        ///     IMPORTANT: The regular expression should match the token separators, not the tokens
        ///                themselves. Flags should be pipe-separated, eg
        ///                "CASE_INSENSITIVE|COMMENTS". Check Java Pattern API for more details about
        ///                flags options.
        /// </summary>
        public string Flags
        {
            get => _flags;
            set
            {
                _flags = value;
                _flagsSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteCommonValues(elasticCrudJsonWriter);

            JsonHelper.WriteValue("lowercase", _lowercase, elasticCrudJsonWriter, _lowercaseSet);
            JsonHelper.WriteValue("pattern", _pattern, elasticCrudJsonWriter, _patternSet);
            JsonHelper.WriteValue("flags", _flags, elasticCrudJsonWriter, _flagsSet);
        }
    }
}