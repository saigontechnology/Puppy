using Puppy.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.CharFilters;
using System.Collections.Generic;

namespace Puppy.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
    //"char_filter" : {
    //	   "my_mapping" : {
    //		   "type" : "mapping",
    //		   "mappings" : ["ph=>f", "qu=>k"]
    //	   }
    //},
    public class AnalysisCharFilter
    {
        private List<AnalysisCharFilterBase> _customCharFilters;
        private bool _customCharFiltersSet;

        public List<AnalysisCharFilterBase> CustomFilters
        {
            get => _customCharFilters;
            set
            {
                _customCharFilters = value;
                _customCharFiltersSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            if (_customCharFiltersSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("char_filter");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();

                foreach (var item in _customCharFilters)
                    item.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }
        }
    }
}