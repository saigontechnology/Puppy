using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
    public class PhoneticTokenFilter : AnalysisFilterBase
    {
        private PhoneticEncoder _encoder;
        private bool _encoderSet;
        private bool _replace;
        private bool _replaceSet;

        /// <summary>
        ///     https://github.com/elastic/elastic-analysis-phonetic A phonetic token filter that can be configured with different encoder types: metaphone, doublemetaphone, soundex, refinedsoundex, caverphone1, caverphone2, cologne, nysiis, koelnerphonetik, haasephonetik, beidermorse The replace
        ///     parameter (defaults to true) controls if the token processed should be replaced with the encoded one (set it to true), or added (set it to false).
        /// </summary>
        /// <param name="name"></param>
        public PhoneticTokenFilter(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenFilters.Phonetic;
        }

        /// <summary>
        ///     A phonetic token filter that can be configured with different encoder types: metaphone, doublemetaphone, soundex, refinedsoundex, caverphone1, caverphone2, cologne, nysiis, koelnerphonetik, haasephonetik, beidermorse 
        /// </summary>
        public PhoneticEncoder Encoder
        {
            get => _encoder;
            set
            {
                _encoder = value;
                _encoderSet = true;
            }
        }

        /// <summary>
        ///     The replace parameter (defaults to true) controls if the token processed should be replaced with the encoded one (set it to true), or added (set it to false). 
        /// </summary>
        public bool Replace
        {
            get => _replace;
            set
            {
                _replace = value;
                _replaceSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteValue("encoder", _encoder.ToString(), elasticCrudJsonWriter, _encoderSet);
            JsonHelper.WriteValue("replace", _replace, elasticCrudJsonWriter, _replaceSet);
        }
    }

    public enum PhoneticEncoder
    {
        metaphone,
        doublemetaphone,
        soundex,
        refinedsoundex,
        caverphone1,
        caverphone2,
        cologne,
        nysiis,
        koelnerphonetik,
        haasephonetik,
        beidermorse
    }
}