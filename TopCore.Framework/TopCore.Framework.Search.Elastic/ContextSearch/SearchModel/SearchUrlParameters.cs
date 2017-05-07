using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextSearch.SearchModel
{
	/// <summary>
	///     This class is used to set routing or pretty search for the url parameters. All other options can be sent in the body 
	/// </summary>
	public class SearchUrlParameters
    {
        private bool _pretty;
        private bool _prettySet;
        private bool _queryCache;
        private bool _queryCacheSet;
        private string _routing;
        private bool _routingSet;
        private SeachType _seachType;
        private bool _seachTypeSet;

        public bool Pretty
        {
            get => _pretty;
            set
            {
                _pretty = value;
                _prettySet = true;
            }
        }

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
	    ///     search_type The type of the search operation to perform. Can be dfs_query_then_fetch, dfs_query_and_fetch, query_then_fetch, query_and_fetch. Defaults to query_then_fetch. See Search Type for more. count and scan 
	    /// </summary>
	    public SeachType SeachType
        {
            get => _seachType;
            set
            {
                _seachType = value;
                _seachTypeSet = true;
            }
        }

	    /// <summary>
	    ///     query_cache [1.4.0.Beta1] Added in 1.4.0.Beta1. Set to true or false to enable or disable the caching of search results for requests where ?search_type=count, ie aggregations and suggestions. See Shard query cache. 
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

        public string GetUrlParameters()
        {
            var parameters = new ParameterCollection();
            parameters.Add("routing", _routing, _routingSet);
            parameters.Add("pretty", "true", _prettySet);
            parameters.Add("search_type", _seachType.ToString(), _seachTypeSet);
            parameters.Add("query_cache", _queryCache.ToString().ToLower(), _queryCacheSet);

            return parameters.ToString();
        }
    }
}