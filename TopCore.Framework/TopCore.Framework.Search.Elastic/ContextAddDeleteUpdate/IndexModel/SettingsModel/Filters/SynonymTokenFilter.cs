using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
    public class SynonymTokenFilter : AnalysisFilterBase
    {
        private bool _expand;
        private bool _expandSet;
        private bool _ignoreCase;
        private bool _ignoreCaseSet;
        private List<string> _synonyms;
        private string _synonymsPath;
        private bool _synonymsPathSet;
        private bool _synonymsSet;

        /// <summary>
        ///   The synonym token filter allows to easily handle synonyms during the analysis process. Synonyms are configured using a configuration
        ///   file. Additional settings are: ignore_case (defaults to false), and expand (defaults to true). The tokenizer parameter controls the
        ///   tokenizers that will be used to tokenize the synonym, and defaults to the whitespace tokenizer.
        /// </summary>
        /// <param name="name"> name for the custom filter </param>
        public SynonymTokenFilter(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenFilters.Synonym;
        }

        /// <summary>
        ///   synonyms_path 
        /// </summary>
        public string SynonymsPath
        {
            get => _synonymsPath;
            set
            {
                _synonymsPath = value;
                _synonymsPathSet = true;
            }
        }

        public bool IgnoreCase
        {
            get => _ignoreCase;
            set
            {
                _ignoreCase = value;
                _ignoreCaseSet = true;
            }
        }

        public bool Expand
        {
            get => _expand;
            set
            {
                _expand = value;
                _expandSet = true;
            }
        }

        /// <summary>
        ///   Two synonym formats are supported: Solr, WordNet. These can be defined directly with this parameter. 
        /// </summary>
        public List<string> Synonyms
        {
            get => _synonyms;
            set
            {
                _synonyms = value;
                _synonymsSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteValue("synonyms_path", _synonymsPath, elasticCrudJsonWriter, _synonymsPathSet);
            JsonHelper.WriteValue("ignore_case", _ignoreCase, elasticCrudJsonWriter, _ignoreCaseSet);
            JsonHelper.WriteValue("expand", _expand, elasticCrudJsonWriter, _expandSet);
            JsonHelper.WriteListValue("synonyms", _synonyms, elasticCrudJsonWriter, _synonymsSet);
        }
    }
}