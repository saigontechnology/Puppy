using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Queries.FunctionQuery
{
	public class ScriptScoreFunction : BaseScoreFunction
    {
        private readonly string _script;
        private string _lang;
        private bool _langSet;
        private List<ScriptParameter> _params;
        private bool _paramsSet;

        public ScriptScoreFunction(string script)
        {
            _script = script;
        }

        public string Lang
        {
            get => _lang;
            set
            {
                _lang = value;
                _langSet = true;
            }
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

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("script_score");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("script", _script, elasticCrudJsonWriter);
            JsonHelper.WriteValue("lang", _lang, elasticCrudJsonWriter, _langSet);
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

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}