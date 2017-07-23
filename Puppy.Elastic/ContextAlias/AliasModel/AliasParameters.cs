using System.Collections.Generic;

namespace Puppy.Elastic.ContextAlias.AliasModel
{
    // "actions" : [ { "remove" : { "index" : "test1", "alias" : "alias1" } }, { "add" : { "index" :
    // "test1", "alias" : "alias2" } } ]
    public class AliasParameters
    {
        private List<AliasBaseParameters> _actions;
        private bool _actionsSet;

        public List<AliasBaseParameters> Actions
        {
            get => _actions;
            set
            {
                _actions = value;
                _actionsSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            if (_actionsSet)
            {
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("actions");
                elasticCrudJsonWriter.JsonWriter.WriteStartArray();

                foreach (var item in _actions)
                {
                    elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                    item.WriteJson(elasticCrudJsonWriter);
                    elasticCrudJsonWriter.JsonWriter.WriteEndObject();
                }
                elasticCrudJsonWriter.JsonWriter.WriteEndArray();
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }
        }

        public override string ToString()
        {
            var elasticCrudJsonWriter = new ElasticJsonWriter();
            WriteJson(elasticCrudJsonWriter);
            return elasticCrudJsonWriter.GetJsonString();
        }
    }
}