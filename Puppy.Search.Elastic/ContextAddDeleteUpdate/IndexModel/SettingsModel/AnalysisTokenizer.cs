using Puppy.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Tokenizers;
using System.Collections.Generic;

namespace Puppy.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
    public class AnalysisTokenizer
    {
        private bool _customFiltersSet;
        private List<AnalysisTokenizerBase> _customTokenizers;

        public List<AnalysisTokenizerBase> CustomTokenizers
        {
            get => _customTokenizers;
            set
            {
                _customTokenizers = value;
                _customFiltersSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            if (_customFiltersSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("tokenizer");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();

                foreach (var item in _customTokenizers)
                    item.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }
        }
    }
}