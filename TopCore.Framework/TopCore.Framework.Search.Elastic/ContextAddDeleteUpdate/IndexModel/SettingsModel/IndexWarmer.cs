using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Model.SearchModel;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
    public class IndexWarmer
    {
        private readonly string _name;
        private List<IAggs> _aggs;
        private bool _aggsSet;
        private List<string> _indexTypes;
        private bool _indexTypesSet;
        private IQueryHolder _query;
        private bool _querySet;

        public IndexWarmer(string name)
        {
            _name = name;
        }

        public IQueryHolder Query
        {
            get => _query;
            set
            {
                _query = value;
                _querySet = true;
            }
        }

        public List<string> IndexTypes
        {
            get => _indexTypes;
            set
            {
                _indexTypes = value;
                _indexTypesSet = true;
            }
        }

        /// <summary>
        ///     aggregations request 
        /// </summary>
        public List<IAggs> Aggs
        {
            get => _aggs;
            set
            {
                _aggs = value;
                _aggsSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_name);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteListValue("types", _indexTypes, elasticCrudJsonWriter, _indexTypesSet);

            elasticCrudJsonWriter.JsonWriter.WritePropertyName("source");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            if (_querySet)
                _query.WriteJson(elasticCrudJsonWriter);

            if (_aggsSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("aggs");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                foreach (var item in _aggs)
                    item.WriteJson(elasticCrudJsonWriter);

                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}