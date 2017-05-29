using System.Collections.Generic;
using Puppy.Search.Elastic.Model.SearchModel.Sorting;
using Puppy.Search.Elastic.Model.Units;
using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.Model.SearchModel
{
    /// <summary>
    ///     The search request can be executed with a search DSL, which includes the Query DSL,
    ///     within its body.
    /// </summary>
    public class Search
    {
        private List<IAggs> _aggs;
        private bool _aggsSet;
        private IFilterHolder _filter;
        private bool _filterSet;
        private int _from;
        private bool _fromSet;
        private Highlight _highlight;
        private bool _highlightSet;
        private IQueryHolder _query;
        private bool _querySet;
        private List<Rescore> _rescore;
        private bool _rescoreSet;
        private int _size;
        private bool _sizeSet;
        private ISortHolder _sortHolder;
        private bool _sortSet;
        private int _terminateAfter;
        private bool _terminateAfterSet;
        private TimeUnit _timeout;
        private bool _timeoutSet;

        /// <summary>
        ///     timeout A search timeout, bounding the search request to be executed within the
        ///     specified time value and bail with the hits accumulated up to that point when
        ///     expired. Defaults to no timeout. See the section called “Time unitsedit”.
        /// </summary>
        public TimeUnit Timeout
        {
            get => _timeout;
            set
            {
                _timeout = value;
                _timeoutSet = true;
            }
        }

        /// <summary>
        ///     from The starting from index of the hits to return. Defaults to 0. 
        /// </summary>
        public int From
        {
            get => _from;
            set
            {
                _from = value;
                _fromSet = true;
            }
        }

        /// <summary>
        ///     size The number of hits to return. Defaults to 10. 
        /// </summary>
        public int Size
        {
            get => _size;
            set
            {
                _size = value;
                _sizeSet = true;
            }
        }

        /// <summary>
        ///     terminate_after [1.4.0.Beta1] Added in 1.4.0.Beta1. The maximum number of documents
        ///     to collect for each shard, upon reaching which the query execution will terminate
        ///     early. If set, the response will have a boolean field terminated_early to indicate
        ///     whether the query execution has actually terminated_early. Defaults to no terminate_after.
        /// </summary>
        public int TerminateAfter
        {
            get => _terminateAfter;
            set
            {
                _terminateAfter = value;
                _terminateAfterSet = true;
            }
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

        public IFilterHolder Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                _filterSet = true;
            }
        }

        public ISortHolder Sort
        {
            get => _sortHolder;
            set
            {
                _sortHolder = value;
                _sortSet = true;
            }
        }

        public Highlight Highlight
        {
            get => _highlight;
            set
            {
                _highlight = value;
                _highlightSet = true;
            }
        }

        public List<Rescore> Rescore
        {
            get => _rescore;
            set
            {
                _rescore = value;
                _rescoreSet = true;
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

            if (_timeout != null)
                JsonHelper.WriteValue("timeout", _timeout.GetTimeUnit(), elasticCrudJsonWriter, _timeoutSet);
            JsonHelper.WriteValue("from", _from, elasticCrudJsonWriter, _fromSet);
            JsonHelper.WriteValue("size", _size, elasticCrudJsonWriter, _sizeSet);
            JsonHelper.WriteValue("terminate_after", _terminateAfter, elasticCrudJsonWriter, _terminateAfterSet);

            if (_querySet)
                _query.WriteJson(elasticCrudJsonWriter);

            if (_filterSet)
                _filter.WriteJson(elasticCrudJsonWriter);

            if (_sortSet)
                _sortHolder.WriteJson(elasticCrudJsonWriter);

            if (_aggsSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("aggs");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                foreach (var item in _aggs)
                    item.WriteJson(elasticCrudJsonWriter);

                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }

            if (_highlightSet)
                _highlight.WriteJson(elasticCrudJsonWriter);

            if (_rescoreSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("rescore");
                elasticCrudJsonWriter.JsonWriter.WriteStartArray();

                foreach (var rescore in _rescore)
                    rescore.WriteJson(elasticCrudJsonWriter);

                elasticCrudJsonWriter.JsonWriter.WriteEndArray();
            }

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