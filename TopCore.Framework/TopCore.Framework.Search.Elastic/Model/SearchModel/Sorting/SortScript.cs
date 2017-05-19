using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Sorting
{
    /// <summary>
    ///   Allow to sort based on custom scripts "sort" : { "_script" : { "script" : "doc['field_name'].value * factor", "type" : "number", "params" : {
    ///   "factor" : 1.1 }, "order" : "asc" } }
    /// </summary>
    public class SortScript : ISortHolder
    {
        private readonly string _script;
        private List<ScriptParameter> _params;
        private bool _paramsSet;
        private string _scriptType;
        private bool _scriptTypeSet;

        public SortScript(string script)
        {
            _script = script;
            Order = OrderEnum.asc;
        }

        public OrderEnum Order { get; set; }

        public string ScriptType
        {
            get => _scriptType;
            set
            {
                _scriptType = value;
                _scriptTypeSet = true;
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

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("sort");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("_script");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName("script");
            elasticCrudJsonWriter.JsonWriter.WriteRawValue("\"" + _script + "\"");
            JsonHelper.WriteValue("order", Order.ToString(), elasticCrudJsonWriter);
            JsonHelper.WriteValue("type", _scriptType, elasticCrudJsonWriter, _scriptTypeSet);
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
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}