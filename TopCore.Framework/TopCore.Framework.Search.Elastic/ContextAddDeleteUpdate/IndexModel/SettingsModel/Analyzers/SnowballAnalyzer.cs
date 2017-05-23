using TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters;
using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers
{
    public class SnowballAnalyzer : BaseStopAnalyzer
    {
        private SnowballLanguage _language;
        private bool _languageSet;

        /// <summary>
        ///     An analyzer of type snowball that uses the standard tokenizer, with standard filter,
        ///     lowercase filter, stop filter, and snowball filter. The Snowball Analyzer is a
        ///     stemming analyzer from Lucene that is originally based on the snowball project from snowball.tartarus.org.
        /// </summary>
        /// <param name="name"></param>
        public SnowballAnalyzer(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultAnalyzers.Snowball;
        }

        public SnowballLanguage Language
        {
            get => _language;
            set
            {
                _language = value;
                _languageSet = true;
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
                WriteCommonValues(elasticCrudJsonWriter);
                JsonHelper.WriteValue("language", _language.ToString(), elasticCrudJsonWriter, _languageSet);
            }
        }
    }
}