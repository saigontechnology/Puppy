using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.SimilarityCustom;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
	public class Similarities
    {
        private List<SimilarityBase> _customSimilarities;
        private bool _customSimilaritiesSet;

        public List<SimilarityBase> CustomSimilarities
        {
            get => _customSimilarities;
            set
            {
                _customSimilarities = value;
                _customSimilaritiesSet = true;
            }
        }

        public virtual void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            if (_customSimilaritiesSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("similarity");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();

                foreach (var item in _customSimilarities)
                    item.WriteJson(elasticCrudJsonWriter);

                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }
        }
    }
}