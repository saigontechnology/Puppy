using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
    public class StemmerOverrideTokenFilter : AnalysisFilterBase
    {
        private List<string> _rules;
        private string _rulesPath;
        private bool _rulesPathSet;
        private bool _rulesSet;

        /// <summary>
        ///     Overrides stemming algorithms, by applying a custom mapping, then protecting these terms from being modified by stemmers. Must be placed before any stemming filters. Rules are separated by =&gt; 
        /// </summary>
        /// <param name="name"> name for the custom filter </param>
        public StemmerOverrideTokenFilter(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenFilters.StemmerOverride;
        }

        /// <summary>
        ///     rules A list of mapping rules to use. 
        /// </summary>
        public List<string> Rules
        {
            get => _rules;
            set
            {
                _rules = value;
                _rulesSet = true;
            }
        }

        /// <summary>
        ///     rules_path A path (either relative to config location, or absolute) to a list of mappings. 
        /// </summary>
        public string RulesPath
        {
            get => _rulesPath;
            set
            {
                _rulesPath = value;
                _rulesPathSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteListValue("rules", _rules, elasticCrudJsonWriter, _rulesSet);
            JsonHelper.WriteValue("rules_path", _rulesPath, elasticCrudJsonWriter, _rulesPathSet);
        }
    }
}