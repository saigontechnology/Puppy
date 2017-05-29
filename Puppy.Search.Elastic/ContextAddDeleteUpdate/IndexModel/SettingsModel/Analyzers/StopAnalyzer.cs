using Puppy.Search.Elastic.Model;
using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers
{
    public class StopAnalyzer : BaseStopAnalyzer
    {
        private string _stopwordsPath;
        private bool _stopwordsPathSet;

        /// <summary>
        ///     An analyzer of type stop that is built using a Lower Case Tokenizer, with Stop Token Filter.
        /// </summary>
        /// <param name="name"> name of the analyzer </param>
        public StopAnalyzer(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultAnalyzers.Stop;
        }

        /// <summary>
        ///     stopwords_path A path (either relative to config location, or absolute) to a
        ///     stopwords file configuration.
        /// </summary>
        public string StopwordsPath
        {
            get => _stopwordsPath;
            set
            {
                _stopwordsPath = value;
                _stopwordsPathSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteCommonValues(elasticCrudJsonWriter);
            JsonHelper.WriteValue("stopwords_path", _stopwordsPath, elasticCrudJsonWriter, _stopwordsPathSet);
        }
    }
}