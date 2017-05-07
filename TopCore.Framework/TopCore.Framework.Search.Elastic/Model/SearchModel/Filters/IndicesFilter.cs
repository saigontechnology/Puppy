using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Filters
{
	/// <summary>
	///     The indices query can be used when executed across multiple indices, allowing to have a query that executes only when executed on an index that matches a specific list of indices, and another filter that executes when it is executed on an index that does not match the listed indices. The
	///     fields order is important: if the indices are provided before filter or no_match_query, the related filters get parsed only against the indices that they are going to be executed on. This is useful to avoid parsing queries when it is not necessary and prevent potential mapping errors.
	/// </summary>
	public class IndicesFilter : IFilter
    {
        private readonly IFilter _filter;
        private readonly List<string> _indices;
        private IFilter _noMatchFilter;
        private bool _noMatchFilterNone;
        private bool _noMatchFilterNoneSet;
        private bool _noMatchFilterSet;

        public IndicesFilter(List<string> indices, IFilter filter)
        {
            _indices = indices;
            _filter = filter;
        }

	    /// <summary>
	    ///     no_match_filter 
	    /// </summary>
	    public IFilter NoMatchFilter
        {
            get => _noMatchFilter;
            set
            {
                _noMatchFilter = value;
                _noMatchFilterSet = true;
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

            elasticCrudJsonWriter.JsonWriter.WritePropertyName("filter");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            _filter.WriteJson(elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();

            if (_noMatchFilterSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("no_match_filter");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                _noMatchFilter.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }
            else if (!_noMatchFilterSet && _noMatchFilterNoneSet)
            {
                JsonHelper.WriteValue("no_match_filter", "none", elasticCrudJsonWriter);
            }

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}