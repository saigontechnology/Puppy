using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Analyzers
{
    public class StandardAnaylzer : BaseStopAnalyzer
    {
        private int _maxTokenLength;
        private bool _maxTokenLengthSet;

        public StandardAnaylzer(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultAnalyzers.Standard;
        }

        /// <summary>
        ///   max_token_length The maximum token length. If a token is seen that exceeds this length then it is discarded. Defaults to 255. 
        /// </summary>
        public int MaxTokenLength
        {
            get => _maxTokenLength;
            set
            {
                _maxTokenLength = value;
                _maxTokenLengthSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteCommonValues(elasticCrudJsonWriter);
            JsonHelper.WriteValue("max_token_length", _maxTokenLength, elasticCrudJsonWriter, _maxTokenLengthSet);
        }
    }
}