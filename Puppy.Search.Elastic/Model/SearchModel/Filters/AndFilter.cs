using System.Collections.Generic;
using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.Model.SearchModel.Filters
{
    /// <summary>
    ///     A filter that matches documents using the AND boolean operator on other filters. Can be
    ///     placed within queries that accept a filter.
    /// </summary>
    public class AndFilter : IFilter
    {
        private readonly List<IFilter> _and;
        private bool _cache;
        private bool _cacheSet;

        public AndFilter(List<IFilter> and)
        {
            _and = and;
        }

        public bool Cache
        {
            get => _cache;
            set
            {
                _cache = value;
                _cacheSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("and");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            WriteAndFilterList(elasticCrudJsonWriter);

            JsonHelper.WriteValue("_cache", _cache, elasticCrudJsonWriter, _cacheSet);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }

        private void WriteAndFilterList(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("filters");
            elasticCrudJsonWriter.JsonWriter.WriteStartArray();

            foreach (var and in _and)
            {
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                and.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }

            elasticCrudJsonWriter.JsonWriter.WriteEndArray();
        }
    }
}