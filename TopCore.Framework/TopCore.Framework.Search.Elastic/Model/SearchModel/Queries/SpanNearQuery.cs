using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Queries
{
    /// <summary>
    ///     Matches spans which are near one another. One can specify slop, the maximum number of intervening unmatched
    ///     positions, as well as whether matches are required to be in-order. The span near query maps to Lucene
    ///     SpanNearQuery. The clauses element is a list of one or more other span type
    ///     queries and the slop controls the maximum number of intervening unmatched positions permitted.
    /// </summary>
    public class SpanNearQuery : ISpanQuery
    {
        private readonly List<SpanTermQuery> _queries;
        private readonly uint _slop;
        private bool _collectPayloads;
        private bool _collectPayloadsSet;
        private bool _inOrder;
        private bool _inOrderSet;

        public SpanNearQuery(List<SpanTermQuery> queries, uint slop)
        {
            if (queries == null)
                throw new ElasticException("parameter List<ISpanQuery> queries cannot be null");
            if (queries.Count < 0)
                throw new ElasticException("parameter List<ISpanQuery> queries should have at least one element");
            _queries = queries;
            _slop = slop;
        }

        /// <summary>
        ///     in_order
        /// </summary>
        public bool InOrder
        {
            get => _inOrder;
            set
            {
                _inOrder = value;
                _inOrderSet = true;
            }
        }

        /// <summary>
        ///     collect_payloads
        /// </summary>
        public bool CollectPayloads
        {
            get => _collectPayloads;
            set
            {
                _collectPayloads = value;
                _collectPayloadsSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("span_near");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("clauses");
            elasticCrudJsonWriter.JsonWriter.WriteStartArray();

            foreach (var item in _queries)
            {
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                item.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }

            elasticCrudJsonWriter.JsonWriter.WriteEndArray();

            JsonHelper.WriteValue("slop", _slop, elasticCrudJsonWriter);
            JsonHelper.WriteValue("in_order", _inOrder, elasticCrudJsonWriter, _inOrderSet);
            JsonHelper.WriteValue("collect_payloads", _collectPayloads, elasticCrudJsonWriter, _collectPayloadsSet);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}