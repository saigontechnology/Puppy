using System.Collections.Generic;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.MappingModel
{
    public class MappingSource
    {
        private bool _enabled;
        private bool _enabledSet;
        private List<string> _excludes;
        private bool _excludesSet;

        private List<string> _includes;
        private bool _includesSet;

        /// <summary>
        ///   The _source field is an automatically generated field that stores the actual JSON that was used as the indexed document. It is not
        ///   indexed (searchable), just stored. When executing "fetch" requests, like get or search, the _source field is returned by default. Though
        ///   very handy to have around, the source field does incur storage overhead within the index. For this reason, it can be disabled. { "tweet"
        ///   : { "_source" : {"enabled" : false} } }
        /// </summary>

        public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                _enabledSet = true;
            }
        }

        /// <summary>
        ///   { "my_type" : { "_source" : { "includes" : ["path1.*", "path2.*"], "excludes" : ["path3.*"] } } } 
        /// </summary>
        public List<string> Includes
        {
            get => _includes;
            set
            {
                _includes = value;
                _includesSet = true;
            }
        }

        public List<string> Excludes
        {
            get => _excludes;
            set
            {
                _excludes = value;
                _excludesSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteSourceEnabledIfSet(elasticCrudJsonWriter);
        }

        private void WriteSourceEnabledIfSet(ElasticJsonWriter elasticCrudJsonWriter)
        {
            if (_enabledSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("_source");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("enabled");
                elasticCrudJsonWriter.JsonWriter.WriteValue(_enabled);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }
            else if (_includesSet || _excludesSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("_source");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();

                WriteListValue("includes", _includes, elasticCrudJsonWriter, _includesSet);
                WriteListValue("excludes", _excludes, elasticCrudJsonWriter, _excludesSet);

                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }
        }

        private void WriteListValue(string key, IEnumerable<string> valueObj, ElasticJsonWriter elasticCrudJsonWriter,
            bool writeValue = true)
        {
            if (writeValue)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName(key);
                elasticCrudJsonWriter.JsonWriter.WriteStartArray();

                foreach (var obj in valueObj)
                    elasticCrudJsonWriter.JsonWriter.WriteValue(obj);
                elasticCrudJsonWriter.JsonWriter.WriteEndArray();
            }
        }
    }
}