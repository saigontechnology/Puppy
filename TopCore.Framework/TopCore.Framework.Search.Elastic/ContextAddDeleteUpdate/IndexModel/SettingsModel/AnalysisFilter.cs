using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
	// "filter" : { "blocks_filter" : { "type" : "word_delimiter", "preserve_original": "true" }, "shingle":{ "type":"shingle", "max_shingle_size":5, "min_shingle_size":2, "output_unigrams":"true" }, "filter_stop":{ "type":"stop", "enable_position_increments":"false" } },
	public class AnalysisFilter
    {
        private List<AnalysisFilterBase> _customFilters;
        private bool _customFiltersSet;

        public List<AnalysisFilterBase> CustomFilters
        {
            get => _customFilters;
            set
            {
                _customFilters = value;
                _customFiltersSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            if (_customFiltersSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("filter");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();

                foreach (var item in _customFilters)
                    item.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }
        }
    }
}