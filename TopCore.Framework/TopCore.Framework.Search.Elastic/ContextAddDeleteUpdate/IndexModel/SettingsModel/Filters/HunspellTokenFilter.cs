using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
    public class HunspellTokenFilter : AnalysisFilterBase
    {
        private bool _dedup;
        private bool _dedupSet;
        private string _dictionary;
        private bool _dictionarySet;
        private string _locale;
        private bool _localeSet;
        private bool _longestOnly;
        private bool _longestOnlySet;

        /// <summary>
        ///     Basic support for hunspell stemming. Hunspell dictionaries will be picked up from a
        ///     dedicated hunspell directory on the filesystem (defaults to path.conf/hunspell). Each
        ///     dictionary is expected to have its own directory named after its associated locale
        ///     (language). This dictionary directory is expected to hold a single *.aff and one or more
        ///     *.dic files (all of which will automatically be picked up).
        /// </summary>
        /// <param name="name"> name for the custom filter </param>
        public HunspellTokenFilter(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenFilters.Hunspell;
        }

        /// <summary>
        ///     locale A locale for this filter. If this is unset, the lang or language are used
        ///     instead - so one of these has to be set.
        /// </summary>
        public string Locale
        {
            get => _locale;
            set
            {
                _locale = value;
                _localeSet = true;
            }
        }

        /// <summary>
        ///     dictionary The name of a dictionary. The path to your hunspell dictionaries should be
        ///     configured via indices.analysis.hunspell.dictionary.location before.
        /// </summary>
        public string Dictionary
        {
            get => _dictionary;
            set
            {
                _dictionary = value;
                _dictionarySet = true;
            }
        }

        /// <summary>
        ///     dedup If only unique terms should be returned, this needs to be set to true. Defaults
        ///     to true.
        /// </summary>
        public bool Dedup
        {
            get => _dedup;
            set
            {
                _dedup = value;
                _dedupSet = true;
            }
        }

        /// <summary>
        ///     longest_only If only the longest term should be returned, set this to true. Defaults
        ///     to false: all possible stems are returned.
        /// </summary>
        public bool LongestOnly
        {
            get => _longestOnly;
            set
            {
                _longestOnly = value;
                _longestOnlySet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteValue("locale", _locale, elasticCrudJsonWriter, _localeSet);
            JsonHelper.WriteValue("dictionary", _dictionary, elasticCrudJsonWriter, _dictionarySet);
            JsonHelper.WriteValue("dedup", _dedup, elasticCrudJsonWriter, _dedupSet);
            JsonHelper.WriteValue("longest_only", _longestOnly, elasticCrudJsonWriter, _longestOnlySet);
        }
    }
}