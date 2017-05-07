using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
	public class DelimitedPayloadFilterTokenFilter : AnalysisFilterBase
    {
        private string _delimiter;
        private bool _delimiterSet;
        private EncodingDelimitedPayloadFilter _encoding;
        private bool _encodingSet;

	    /// <summary>
	    ///     Named delimited_payload_filter. Splits tokens into tokens and payload whenever a delimiter character is found.
	    ///     Example: "the|1 quick|2 fox|3" is split per default int to tokens fox, quick and the with payloads 1, 2 and 3 respectively.
	    /// </summary>
	    /// <param name="name"> name for the custom filter </param>
	    public DelimitedPayloadFilterTokenFilter(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenFilters.DelimitedPayloadFilter;
        }

	    /// <summary>
	    ///     delimiter Character used for splitting the tokens. Default is |. 
	    /// </summary>
	    public string Delimiter
        {
            get => _delimiter;
            set
            {
                _delimiter = value;
                _delimiterSet = true;
            }
        }

	    /// <summary>
	    ///     encoding The type of the payload. int for integer, float for float and identity for characters. Default is float. 
	    /// </summary>
	    public EncodingDelimitedPayloadFilter Encoding
        {
            get => _encoding;
            set
            {
                _encoding = value;
                _encodingSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteValue("delimiter", _delimiter, elasticCrudJsonWriter, _delimiterSet);
            JsonHelper.WriteValue("encoding", _encoding.ToString(), elasticCrudJsonWriter, _encodingSet);
        }
    }

    public enum EncodingDelimitedPayloadFilter
    {
        @int,
        @float,
        identity
    }
}