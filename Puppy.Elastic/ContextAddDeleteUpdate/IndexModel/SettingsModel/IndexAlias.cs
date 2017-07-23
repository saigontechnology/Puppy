using Puppy.Elastic.Utils;

namespace Puppy.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
    /// <summary>
    ///     This model is only used when creating an index. If it is required that an index is added
    ///     later, use the alias api.
    /// </summary>
    public class IndexAlias
    {
        private readonly string _alias;
        private string _filter;
        private bool _filterSet;
        private string _routing;
        private bool _routingSet;

        public IndexAlias(string alias)
        {
            MappingUtils.GuardAgainstBadIndexName(alias);

            _alias = alias;
        }

        /// <summary>
        ///     It is possible to associate routing values with aliases. This feature can be used
        ///     together with filtering aliases in order to avoid unnecessary shard operations.
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
        ///     An optional filter that can be associated with an alias. TODO replace this raw json
        ///     string with a filter object once the filter class has been created.
        /// </summary>
        public string Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                _filterSet = true;
            }
        }

        //"aliases" : {
        //  "april_2014" : {},
        //  "year_2014" : {}
        //},
        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_alias);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("routing", _routing, elasticCrudJsonWriter, _routingSet);
            if (_filterSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("filter");
                elasticCrudJsonWriter.JsonWriter.WriteRawValue(_filter);
            }

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}