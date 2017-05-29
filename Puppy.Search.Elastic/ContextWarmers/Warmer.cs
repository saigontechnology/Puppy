using System.Collections.Generic;
using Puppy.Search.Elastic.Model.SearchModel;
using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.ContextWarmers
{
    public class Warmer
    {
        private List<IAggs> _aggs;
        private bool _aggsSet;
        private IQueryHolder _query;
        private bool _queryCache;
        private bool _queryCacheSet;
        private bool _querySet;

        public Warmer(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public IQueryHolder Query
        {
            get => _query;
            set
            {
                _query = value;
                _querySet = true;
            }
        }

        /// <summary>
        ///     query_cache 
        /// </summary>
        public bool QueryCache
        {
            get => _queryCache;
            set
            {
                _queryCache = value;
                _queryCacheSet = true;
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

            JsonHelper.WriteValue("query_cache", _queryCache, elasticCrudJsonWriter, _queryCacheSet);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }

        public override string ToString()
        {
            var elasticCrudJsonWriter = new ElasticJsonWriter();
            WriteJson(elasticCrudJsonWriter);
            return elasticCrudJsonWriter.GetJsonString();
        }
    }
}