using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Filters
{
    /// <summary>
    ///     Scripts are compiled and cached for faster execution. If the same script can be used, just with different
    ///     parameters provider, it is preferable to use the ability to pass parameters to the script itself
    /// </summary>
    public class ScriptFilter : IFilter
    {
        private readonly string _script;
        private List<ScriptParameter> _params;
        private bool _paramsSet;

        public ScriptFilter(string script)
        {
            _script = script;
        }

        public List<ScriptParameter> Params
        {
            get => _params;
            set
            {
                _params = value;
                _paramsSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("script");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName("script");
            elasticCrudJsonWriter.JsonWriter.WriteRawValue("\"" + _script + "\"");
            if (_paramsSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("params");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();

                foreach (var item in _params)
                {
                    elasticCrudJsonWriter.JsonWriter.WritePropertyName(item.ParameterName);
                    elasticCrudJsonWriter.JsonWriter.WriteValue(item.ParameterValue);
                }
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }

            JsonHelper.WriteValue("lang", "groovy", elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}