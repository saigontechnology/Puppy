using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Tokenizers
{
    public abstract class BaseTokenizer : AnalysisTokenizerBase
    {
        private int _maxTokenLength;
        private bool _maxTokenLengthSet;

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
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteValue("max_token_length", _maxTokenLength, elasticCrudJsonWriter, _maxTokenLengthSet);
        }
    }
}