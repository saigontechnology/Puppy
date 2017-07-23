using Puppy.Elastic.Utils;
using System.Collections.Generic;

namespace Puppy.Elastic.Model.SearchModel.Queries
{
    public class IndicesQuery : IQuery
    {
        private readonly List<string> _indices;
        private readonly IQuery _query;
        private bool _noMatchFilterNone;
        private bool _noMatchFilterNoneSet;
        private IQuery _noMatchQuery;
        private bool _noMatchQuerySet;

        public IndicesQuery(List<string> indices, IQuery query)
        {
            _indices = indices;
            _query = query;
        }

        /// <summary>
        ///     no_match_query 
        /// </summary>
        public IQuery NoMatchQuery
        {
            get => _noMatchQuery;
            set
            {
                _noMatchQuery = value;
                _noMatchQuerySet = true;
            }
        }

        public bool NoMatchFilterNone
        {
            get => _noMatchFilterNone;
            set
            {
                _noMatchFilterNone = value;
                _noMatchFilterNoneSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("indices");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteListValue("indices", _indices, elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WritePropertyName("query");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            _query.WriteJson(elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();

            if (_noMatchQuerySet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("no_match_query");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                _noMatchQuery.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }
            else if (!_noMatchQuerySet && _noMatchFilterNoneSet)
            {
                JsonHelper.WriteValue("no_match_query", "none", elasticCrudJsonWriter);
            }

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}