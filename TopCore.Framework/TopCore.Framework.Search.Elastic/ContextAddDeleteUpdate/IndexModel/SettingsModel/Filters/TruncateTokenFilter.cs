using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class TruncateTokenFilter : AnalysisFilterBase
    {
        private int _length;
        private bool _lengthSet;

	    /// <summary>
	    ///     The truncate token filter can be used to truncate tokens into a specific length. This can come in handy with keyword (single token) based mapped fields that are used for sorting in order to reduce memory usage. 
	    /// </summary>
	    /// <param name="name"> name for the custom filter </param>
	    public TruncateTokenFilter(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenFilters.Truncate;
        }

	    /// <summary>
	    ///     length It accepts a length parameter which control the number of characters to truncate to, defaults to 10. 
	    /// </summary>
	    public int Length
        {
            get => _length;
            set
            {
                _length = value;
                _lengthSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteValue("length", _length, elasticCrudJsonWriter, _lengthSet);
        }
    }
}