using Puppy.Search.Elastic.Model;
using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
    public class LowercaseTokenFilter : AnalysisFilterBase
    {
        private string _language;
        private bool _languageSet;

        /// <summary>
        ///     A token filter of type lowercase that normalizes token text to lower case. Lowercase
        ///     token filter supports Greek, Irish, and Turkish lowercase token filters through the
        ///     language parameter.
        /// </summary>
        /// <param name="name"> name for the custom filter </param>
        public LowercaseTokenFilter(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenFilters.Lowercase;
        }

        public string Language
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
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteValue("language", _language, elasticCrudJsonWriter, _languageSet);
        }
    }
}