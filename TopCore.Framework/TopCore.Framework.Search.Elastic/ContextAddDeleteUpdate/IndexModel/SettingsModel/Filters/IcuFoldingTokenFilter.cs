using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
    public class IcuFoldingTokenFilter : AnalysisFilterBase
    {
        private string _unicodeSetFilter;
        private bool _unicodeSetFilterSet;

        public IcuFoldingTokenFilter(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenFilters.IcuFolding;
        }

        /// <summary>
        ///   unicodeSetFilter 
        /// </summary>
        public string UnicodeSetFilter
        {
            get => _unicodeSetFilter;
            set
            {
                _unicodeSetFilter = value;
                _unicodeSetFilterSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteValue("unicodeSetFilter", _unicodeSetFilter, elasticCrudJsonWriter, _unicodeSetFilterSet);
        }
    }
}