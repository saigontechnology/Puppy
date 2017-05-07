using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers
{
	public abstract class BaseStopAnalyzer : AnalyzerBase
    {
        private string _stopwords;
        private List<string> _stopwordsList;
        private bool _stopwordsListSet;
        private bool _stopwordsSet;

	    /// <summary>
	    ///     A list of stopwords to initialize the stop filter with. Defaults to the english stop words. Use stopwords: _none_ to explicitly specify an empty stopword list. 
	    /// </summary>
	    public string Stopwords
        {
            get => _stopwords;
            set
            {
                _stopwords = value;
                _stopwordsSet = true;
            }
        }

        public List<string> StopwordsList
        {
            get => _stopwordsList;
            set
            {
                _stopwordsList = value;
                _stopwordsListSet = true;
            }
        }

        protected void WriteCommonValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            //JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            if (_stopwordsListSet)
                JsonHelper.WriteListValue("stopwords", _stopwordsList, elasticCrudJsonWriter, _stopwordsListSet);
            else
                JsonHelper.WriteValue("stopwords", _stopwords, elasticCrudJsonWriter, _stopwordsSet);
        }
    }
}