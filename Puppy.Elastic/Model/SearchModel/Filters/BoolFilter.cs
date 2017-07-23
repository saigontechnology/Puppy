using Puppy.Elastic.Utils;
using System.Collections.Generic;

namespace Puppy.Elastic.Model.SearchModel.Filters
{
    /// <summary>
    ///     A query that matches documents matching boolean combinations of other queries. The bool
    ///     query maps to Lucene BooleanQuery. It is built using one or more boolean clauses, each
    ///     clause with a typed occurrence. { "query":{ "bool" : { "must" : [ { "term" : { "details"
    ///     : "different" } }, { "term" : { "details" : "data" } } ], "must_not" : [ { "range" : {
    ///     "id" : { "from" : 7, "to" : 20 } } } ], "should" : [ { "term" : { "details" : "data" } },
    ///     { "term" : { "details" : "alone" } } ] } } }
    /// </summary>
    public class BoolFilter : IFilter
    {
        private bool _cache;
        private bool _cacheSet;
        private List<IFilter> _must;
        private List<IFilter> _mustNot;
        private bool _mustNotSet;
        private bool _mustSet;
        private List<IFilter> _should;
        private bool _shouldSet;

        public BoolFilter()
        {
        }

        public BoolFilter(IFilter must, IFilter mustNot = null)
        {
            Must = new List<IFilter> { must };

            if (mustNot != null)
                MustNot = new List<IFilter> { mustNot };
        }

        public List<IFilter> Must
        {
            get => _must;
            set
            {
                _must = value;
                _mustSet = true;
            }
        }

        public List<IFilter> MustNot
        {
            get => _must;
            set
            {
                _mustNot = value;
                _mustNotSet = true;
            }
        }

        public List<IFilter> Should
        {
            get => _should;
            set
            {
                _should = value;
                _shouldSet = true;
            }
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
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("bool");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            WriteMustQueryList(elasticCrudJsonWriter);
            WriteMustNotQueryList(elasticCrudJsonWriter);
            WriteShouldQueryList(elasticCrudJsonWriter);

            JsonHelper.WriteValue("_cache", _cache, elasticCrudJsonWriter, _cacheSet);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }

        private void WriteShouldQueryList(ElasticJsonWriter elasticCrudJsonWriter)
        {
            if (_shouldSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("should");
                elasticCrudJsonWriter.JsonWriter.WriteStartArray();

                foreach (var shouldItem in _should)
                {
                    elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                    shouldItem.WriteJson(elasticCrudJsonWriter);
                    elasticCrudJsonWriter.JsonWriter.WriteEndObject();
                }

                elasticCrudJsonWriter.JsonWriter.WriteEndArray();
            }
        }

        private void WriteMustNotQueryList(ElasticJsonWriter elasticCrudJsonWriter)
        {
            if (_mustNotSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("must_not");
                elasticCrudJsonWriter.JsonWriter.WriteStartArray();

                foreach (var mustNotItem in _mustNot)
                {
                    elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                    mustNotItem.WriteJson(elasticCrudJsonWriter);
                    elasticCrudJsonWriter.JsonWriter.WriteEndObject();
                }

                elasticCrudJsonWriter.JsonWriter.WriteEndArray();
            }
        }

        private void WriteMustQueryList(ElasticJsonWriter elasticCrudJsonWriter)
        {
            if (_mustSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("must");
                elasticCrudJsonWriter.JsonWriter.WriteStartArray();

                foreach (var mustItem in _must)
                {
                    elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                    mustItem.WriteJson(elasticCrudJsonWriter);
                    elasticCrudJsonWriter.JsonWriter.WriteEndObject();
                }

                elasticCrudJsonWriter.JsonWriter.WriteEndArray();
            }
        }
    }
}