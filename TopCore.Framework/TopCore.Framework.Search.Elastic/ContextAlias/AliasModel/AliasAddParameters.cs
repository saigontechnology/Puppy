using TopCore.Framework.Search.Elastic.Model.SearchModel;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAlias.AliasModel
{
    /// <summary>
    ///   APIs in elastic accept an index name when working against a specific index, and several indices when applicable. The index aliases API allow
    ///   to alias an index with a name, with all APIs automatically converting the alias name to the actual index name. An alias can also be mapped to
    ///   more than one index, and when specifying it, the alias will automatically expand to the aliases indices. An alias can also be associated with
    ///   a filter that will automatically be applied when searching, and routing values.
    /// </summary>
    public class AliasAddParameters : AliasBaseParameters
    {
        private IFilter _filter;
        private bool _filterSet;
        private string _routing;
        private bool _routingSet;

        public AliasAddParameters(string alias, string index) : base(alias, index)
        {
        }

        /// <summary>
        ///   It is possible to associate routing values with aliases. This feature can be used together with filtering aliases in order to avoid
        ///   unnecessary shard operations.
        /// </summary>
        public string Routing
        {
            get => _routing;
            set
            {
                _routing = value;
                _routingSet = true;
            }
        }

        /// <summary>
        ///   An optional filter that can be associated with an alias. 
        /// </summary>
        public IFilter Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                _filterSet = true;
            }
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("routing", _routing, elasticCrudJsonWriter, _routingSet);
            if (_filterSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("filter");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                _filter.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteInternalJson(elasticCrudJsonWriter, AliasAction.add, WriteValues);
        }
    }
}