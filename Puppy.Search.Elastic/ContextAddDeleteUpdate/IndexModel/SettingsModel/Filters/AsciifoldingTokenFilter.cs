using Puppy.Search.Elastic.Model;
using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
    public class AsciifoldingTokenFilter : AnalysisFilterBase
    {
        private bool _preserveOriginal;
        private bool _preserveOriginalSet;

        /// <summary>
        ///     A token filter of type asciifolding that converts alphabetic, numeric, and symbolic
        ///     Unicode characters which are not in the first 127 ASCII characters (the "Basic Latin"
        ///     Unicode block) into their ASCII equivalents, if one exists.
        /// </summary>
        public AsciifoldingTokenFilter(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenFilters.Asciifolding;
        }

        /// <summary>
        ///     Accepts preserve_original setting which defaults to false but if true will keep the
        ///     original token as well as emit the folded token.
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
            JsonHelper.WriteValue("preserve_original", _preserveOriginal, elasticCrudJsonWriter, _preserveOriginalSet);
        }
    }
}