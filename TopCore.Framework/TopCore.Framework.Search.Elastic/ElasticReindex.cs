using System;
using System.Diagnostics;
using TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel;
using TopCore.Framework.Search.Elastic.ContextSearch;
using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Model.Units;
using TopCore.Framework.Search.Elastic.Tracing;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic
{
    public class ElasticReindex<TOld, TNew>
        where TNew : class
        where TOld : class
    {
        private readonly ElasticContext _context;
        private readonly IndexTypeDescription _newIndexTypeDescription;
        private readonly IndexTypeDescription _oldIndexTypeDescription;

        private ITraceProvider _traceProvider = new NullTraceProvider();

        /// <summary>
        ///   Scan ans scroll settings for the reindex. You should keep this small, otherwise the response request will be too larger. Default is 5
        ///   seconds with a size of 500 documents pro shard.
        /// </summary>
        public ScanAndScrollConfiguration ScanAndScrollConfiguration =
            new ScanAndScrollConfiguration(new TimeUnitSecond(5), 500);

        /// <summary>
        ///   Reindex constructor. The class reuires a index and a type for the old index and also the new index. The old index can then be converteds
        ///   or reindexed to the new index.
        /// </summary>
        /// <param name="oldIndexTypeDescription"> index and index type parameters for the old index </param>
        /// <param name="newIndexTypeDescription"> index and index type parameters for the new index </param>
        /// <param name="connectionString">        Elastic connection string </param>
        public ElasticReindex(IndexTypeDescription oldIndexTypeDescription,
            IndexTypeDescription newIndexTypeDescription, string connectionString)
        {
            _oldIndexTypeDescription = oldIndexTypeDescription;
            _newIndexTypeDescription = newIndexTypeDescription;
            IElasticMappingResolver elasticMappingResolver = new ElasticMappingResolver();
            elasticMappingResolver.AddElasticSearchMappingForEntityType(typeof(TNew),
                MappingUtils.GetElasticMapping<TNew>(newIndexTypeDescription));
            elasticMappingResolver.AddElasticSearchMappingForEntityType(typeof(TOld),
                MappingUtils.GetElasticMapping<TOld>(oldIndexTypeDescription));
            _context = new ElasticContext(connectionString, elasticMappingResolver);
        }

        /// <summary>
        ///   TraceProvider if logging or tracing is used 
        /// </summary>
        public ITraceProvider TraceProvider
        {
            get => _traceProvider;
            set
            {
                _context.TraceProvider = value;
                _traceProvider = value;
            }
        }

        /// <summary>
        ///   Resets the alias from the old index to the new index. Assumes that a alias is used for the old indeex. This way, reindex can be done live.
        /// </summary>
        /// <param name="alias"> alias string used for the index. </param>
        public void SwitchAliasfromOldToNewIndex(string alias)
        {
            _context.AliasReplaceIndex(alias, _oldIndexTypeDescription.Index, _newIndexTypeDescription.Index);
        }

        /// <summary>
        ///   This method is used to reindex one index to a new index using a query, and two Functions. 
        /// </summary>
        /// <param name="jsonContent">          Json content for the search query </param>
        /// <param name="getKeyMethod">         Func is require to define the _id required for the new index </param>
        /// <param name="convertMethod">       
        ///   Func used to map the old index to the new old, whatever your required mapping/conversion logic is
        /// </param>
        /// <param name="getRoutingDefinition"> Function to get the RoutingDefinition of the document </param>
        public void Reindex(string jsonContent, Func<TOld, object> getKeyMethod, Func<TOld, TNew> convertMethod,
            Func<TOld, RoutingDefinition> getRoutingDefinition = null)
        {
            var result = _context.SearchCreateScanAndScroll<TOld>(jsonContent, ScanAndScrollConfiguration);

            var scrollId = result.PayloadResult.ScrollId;
            TraceProvider.Trace(TraceEventType.Information, "ElasticReindex: Reindex: Total SearchResult in scan: {0}",
                result.PayloadResult.Hits.Total);

            var indexProccessed = 0;
            while (result.PayloadResult.Hits.Total > indexProccessed)
            {
                TraceProvider.Trace(TraceEventType.Information,
                    "ElasticReindex: Reindex: creating new documents, indexProccessed: {0} SearchResult: {1}",
                    indexProccessed, result.PayloadResult.Hits.Total);

                var resultCollection = _context.SearchScanAndScroll<TOld>(scrollId, ScanAndScrollConfiguration);
                scrollId = resultCollection.PayloadResult.ScrollId;

                foreach (var item in resultCollection.PayloadResult.Hits.HitsResult)
                {
                    indexProccessed++;
                    if (getRoutingDefinition != null)
                        _context.AddUpdateDocument(convertMethod(item.Source), getKeyMethod(item.Source),
                            getRoutingDefinition(item.Source));
                    else
                        _context.AddUpdateDocument(convertMethod(item.Source), getKeyMethod(item.Source));
                }
                _context.SaveChanges();
            }
        }
    }
}