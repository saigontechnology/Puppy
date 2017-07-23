using Puppy.Elastic.Utils;

namespace Puppy.Elastic.Model.SearchModel
{
    public class Document
    {
        private object _id;
        private bool _idSet;
        private string _index;
        private bool _indexSet;
        private string _routing;
        private bool _routingSet;
        private string _type;
        private bool _typeSet;

        public string Index
        {
            get => _index;
            set
            {
                _index = value;
                _indexSet = true;
            }
        }

        public string Type
        {
            get => _type;
            set
            {
                _type = value;
                _typeSet = true;
            }
        }

        public object Id
        {
            get => _id;
            set
            {
                _id = value;
                _idSet = true;
            }
        }

        public string Routing
        {
            get => _routing;
            set
            {
                _routing = value;
                _routingSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("_index", _index, elasticCrudJsonWriter, _indexSet);
            JsonHelper.WriteValue("_type", _type, elasticCrudJsonWriter, _typeSet);
            JsonHelper.WriteValue("_id", _id, elasticCrudJsonWriter, _idSet);
            JsonHelper.WriteValue("_routing", _routing, elasticCrudJsonWriter, _routingSet);
        }
    }
}