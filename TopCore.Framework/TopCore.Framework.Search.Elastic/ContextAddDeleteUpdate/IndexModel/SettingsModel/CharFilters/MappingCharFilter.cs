using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.CharFilters
{
	public class MappingCharFilter : AnalysisCharFilterBase
    {
        private List<string> _mappings;
        private string _mappingsPath;
        private bool _mappingsPathSet;
        private bool _mappingsSet;

	    /// <summary>
	    ///     A char filter of type mapping replacing characters of an analyzed text with given mapping. "char_filter" : { "my_mapping" : { "type" : "mapping", "mappings" : ["ph=&gt;f", "qu=&gt;k"] } }, 
	    /// </summary>
	    /// <param name="name"> name for the custom mapping char filter </param>
	    public MappingCharFilter(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultCharFilters.Mapping;
        }

	    /// <summary>
	    ///     mappings 
	    /// </summary>
	    public List<string> Mappings
        {
            get => _mappings;
            set
            {
                _mappings = value;
                _mappingsSet = true;
            }
        }

	    /// <summary>
	    ///     mappings_path 
	    /// </summary>
	    public string MappingsPath
        {
            get => _mappingsPath;
            set
            {
                _mappingsPath = value;
                _mappingsPathSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            if (_mappingsSet)
                JsonHelper.WriteListValue("mappings", _mappings, elasticCrudJsonWriter, _mappingsSet);
            else
                JsonHelper.WriteValue("mappings_path", _mappingsPath, elasticCrudJsonWriter, _mappingsPathSet);
        }
    }
}