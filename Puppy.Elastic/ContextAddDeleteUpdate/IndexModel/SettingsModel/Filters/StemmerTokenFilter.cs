using Puppy.Elastic.Model;
using Puppy.Elastic.Utils;

namespace Puppy.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
    public class StemmerTokenFilter : AnalysisFilterBase
    {
        private Stemmer _stemmerName;
        private bool _stemmerNameSet;

        /// <summary>
        ///     A filter that provides access to (almost) all of the available stemming token filters
        ///     through a single unified interface
        /// </summary>
        /// <param name="name"></param>
        public StemmerTokenFilter(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenFilters.Stemmer;
        }

        /// <summary>
        ///     Either front or back. Defaults to front. 
        /// </summary>
        public Stemmer StemmerName
        {
            get => _stemmerName;
            set
            {
                _stemmerName = value;
                _stemmerNameSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteValue("name", _stemmerName.ToString(), elasticCrudJsonWriter, _stemmerNameSet);
        }
    }

    public enum Stemmer
    {
        arabic,
        armenian,
        basque,
        brazilian,
        bulgarian,
        catalan,
        czech,
        danish,
        dutch,
        dutch_kp,
        english,
        light_english,
        minimal_english,
        possessive_english,
        porter2,
        lovins,
        finnish,
        light_finnish,
        french,
        light_french,
        minimal_french,
        galician,
        minimal_galician,
        german,
        german2,
        light_german,
        minimal_german,
        greek,
        hindi,
        hungarian,
        light_hungarian,
        indonesian,
        irish,
        italian,
        light_italian,
        sorani,
        latvian,
        norwegian,
        light_norwegian,
        minimal_norwegian,
        light_nynorsk,
        minimal_nynorsk,
        portuguese,
        light_portuguese,
        minimal_portuguese,
        portuguese_rslp,
        romanian,
        russian,
        light_russian,
        spanish,
        light_spanish,
        swedish,
        light_swedish,
        turkish
    }
}