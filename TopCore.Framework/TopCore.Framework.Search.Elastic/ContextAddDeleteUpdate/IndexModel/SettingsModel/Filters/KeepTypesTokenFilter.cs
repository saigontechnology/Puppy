using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
    public class KeepTypesTokenFilter : AnalysisFilterBase
    {
        private List<string> _types;
        private bool _typesSet;

        /// <summary>
        ///     A token filter of type keep_types that only keeps tokens with a token type contained in a predefined set. 
        /// </summary>
        /// <param name="name"> name for the custom filter </param>
        public KeepTypesTokenFilter(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenFilters.KeepTypes;
        }

        /// <summary>
        ///     A list of types to keep 
        /// </summary>
        public List<string> Types
        {
            get => _types;
            set
            {
                _types = value;
                _typesSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteListValue("types", _types, elasticCrudJsonWriter, _typesSet);
        }
    }
}