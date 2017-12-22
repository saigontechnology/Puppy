using Puppy.Core.EnvironmentUtils;
using Puppy.Elastic.ContentExists;
using Puppy.Elastic.ContextAddDeleteUpdate;
using Puppy.Elastic.ContextAddDeleteUpdate.IndexModel;
using Puppy.Elastic.ContextAddDeleteUpdate.IndexModel.MappingModel;
using Puppy.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel;
using Puppy.Elastic.ContextAlias;
using Puppy.Elastic.ContextAlias.AliasModel;
using Puppy.Elastic.ContextClearCache;
using Puppy.Elastic.ContextCount;
using Puppy.Elastic.ContextDeleteByQuery;
using Puppy.Elastic.ContextGet;
using Puppy.Elastic.ContextSearch;
using Puppy.Elastic.ContextSearch.SearchModel;
using Puppy.Elastic.ContextWarmers;
using Puppy.Elastic.Model;
using Puppy.Elastic.Tracing;
using Puppy.Elastic.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Puppy.Elastic
{
    /// <summary>
    ///     Context for crud operations. 
    /// </summary>
    public class ElasticContext : IDisposable, IElasticContext
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly HttpClient _client = new HttpClient();
        private readonly string _connectionString;
        private readonly ElasticSerializerConfiguration _elasticSerializerConfiguration;
        private readonly List<EntityContextInfo> _entityPendingChanges = new List<EntityContextInfo>();

        private ElasticContextAddDeleteUpdate _elasticContextAddDeleteUpdate;
        private ElasticContextAlias _elasticContextAlias;
        private ElasticContextClearCache _elasticContextClearCache;
        private ElasticContextCount _elasticContextCount;
        private ElasticContextDeleteByQuery _elasticContextDeleteByQuery;

        private ElasticContextExists _elasticContextExists;
        private ElasticContextGet _elasticContextGet;

        private ElasticContextIndexMapping _elasticContextIndexMapping;
        private ElasticContextSearch _elasticContextSearch;
        private ElasticContextWarmer _elasticContextWarmer;
        private SearchRequest _searchRequest;

        /// <summary>
        ///     TraceProvider for all logs, trace etc. This can be replaced with any TraceProvider implementation.
        /// </summary>
        private ITraceProvider _traceProvider = new NullTraceProvider();

        /// <summary>
        ///     Create Elastic Context 
        /// </summary>
        /// <param name="connectionString">              
        ///     Connection string which is used as the base URL for the HttpClient
        /// </param>
        /// <param name="elasticSerializerConfiguration">
        ///     Configuration class for the context. The default settings can be oset here. This
        ///     class contains an IElasticMappingResolver
        /// </param>
        /// <param name="credentials">                   
        ///     Basic auth credentials for logging into the server.
        /// </param>
        /// <param name="isAllowDeleteForIndex">         </param>
        public ElasticContext(string connectionString, ElasticSerializerConfiguration elasticSerializerConfiguration, NetworkCredential credentials = null, bool isAllowDeleteForIndex = false)
        {
            if (credentials != null)
            {
                SetBasicAuthHeader(credentials);
            }

            _elasticSerializerConfiguration = elasticSerializerConfiguration;

            _connectionString = connectionString;

            if (EnvironmentHelper.IsDevelopment())
            {
                TraceProvider = new ConsoleTraceProvider();
                TraceProvider.Trace(TraceEventType.Verbose, "{1}: new ElasticContext with connection string: {0}", connectionString, "ElasticContext");
            }

            AllowDeleteForIndex = isAllowDeleteForIndex;

            InitialContext();
        }

        /// <inheritdoc />
        /// <summary>
        ///     Create Elastic Context 
        /// </summary>
        /// <param name="connectionString">      
        ///     Connection string which is used as the base URL for the HttpClient
        /// </param>
        /// <param name="elasticMappingResolver">
        ///     Resolver used for getting the index and type of a document type. The default
        ///     ElasticSerializerConfiguration is used in this ctor.
        /// </param>
        /// <param name="credentials">           
        ///     Basic auth credentials for logging into the server.
        /// </param>
        /// <param name="isAllowDeleteForIndex"> </param>
        public ElasticContext(string connectionString, IElasticMappingResolver elasticMappingResolver, NetworkCredential credentials = null, bool isAllowDeleteForIndex = false) : this(connectionString, new ElasticSerializerConfiguration(elasticMappingResolver), credentials, isAllowDeleteForIndex)
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Create Elastic Context by ElasticConfig load from appsettings.json 
        /// </summary>
        public ElasticContext() : this(ElasticConfig.ConnectionString, new ElasticSerializerConfiguration(new ElasticMappingResolver()), null, true)
        {
        }

        public ITraceProvider TraceProvider
        {
            get => _traceProvider;
            set
            {
                _traceProvider = value;
                InitialContext();
            }
        }

        /// <summary>
        ///     This bool needs to be set to true if you want to delete an index. Per default this is false.
        /// </summary>
        public bool AllowDeleteForIndex { get; set; }

        /// <inheritdoc />
        /// <summary>
        ///     Dispose used to clean the HttpClient 
        /// </summary>
        public void Dispose()
        {
            _client?.Dispose();
        }

        /// <summary>
        ///     Sets the HttpClient Basic Auth header. 
        /// </summary>
        /// <param name="credentials"> Basic auth credentials for logging into the server. </param>
        private void SetBasicAuthHeader(NetworkCredential credentials)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(
                    Encoding.ASCII.GetBytes(string.Format("{0}:{1}", credentials.UserName, credentials.Password))));
        }

        /// <inheritdoc />
        public void UpsertDocument(object document, object id, RoutingDefinition routingDefinition = null)
        {
            TraceProvider.Trace(TraceEventType.Verbose, "{2}: Adding document: {0}, {1} to pending list",
                document.GetType().Name, id, "ElasticContext");
            var data = new EntityContextInfo { Id = id, EntityType = document.GetType(), Document = document };
            if (routingDefinition != null)
                data.RoutingDefinition = routingDefinition;

            _entityPendingChanges.Add(data);
        }

        /// <inheritdoc />
        public void DeleteDocument<T>(object id, RoutingDefinition routingDefinition = null)
        {
            if (routingDefinition == null)
                routingDefinition = new RoutingDefinition();
            TraceProvider.Trace(TraceEventType.Verbose,
                "{2}: Request to delete document with id: {0}, Type: {1}, adding to pending list", id, typeof(T).Name,
                "ElasticContext");
            _entityPendingChanges.Add(new EntityContextInfo
            {
                Id = id.ToString(),
                DeleteDocument = true,
                EntityType = typeof(T),
                Document = null,
                RoutingDefinition = routingDefinition
            });
        }

        /// <inheritdoc />
        public ResultDetails<string> SaveChangesAndInitMappings()
        {
            return _elasticContextAddDeleteUpdate.SaveChanges(_entityPendingChanges, true);
        }

        /// <inheritdoc />
        public ResultDetails<string> SaveChanges()
        {
            return _elasticContextAddDeleteUpdate.SaveChanges(_entityPendingChanges, false);
        }

        /// <inheritdoc />
        public async Task<ResultDetails<string>> SaveChangesAsync()
        {
            return await _elasticContextAddDeleteUpdate.SaveChangesAsync(_entityPendingChanges);
        }

        /// <inheritdoc />
        public ResultDetails<OptimizeResult> IndexOptimize(string index = null, OptimizeParameters optimizeParameters = null)
        {
            return _elasticContextIndexMapping.IndexOptimize(index, optimizeParameters);
        }

        /// <inheritdoc />
        public async Task<ResultDetails<OptimizeResult>> IndexOptimizeAsync(string index = null,
            OptimizeParameters optimizeParameters = null)
        {
            return await _elasticContextIndexMapping.IndexOptimizeAsync(index, optimizeParameters);
        }

        /// <inheritdoc />
        public ResultDetails<bool> IndexClose(string index)
        {
            return _elasticContextIndexMapping.CloseIndex(index);
        }

        /// <inheritdoc />
        public async Task<ResultDetails<bool>> IndexCloseAsync(string index)
        {
            return await _elasticContextIndexMapping.CloseIndexAsync(index);
        }

        /// <inheritdoc />
        public ResultDetails<bool> IndexOpen(string index)
        {
            return _elasticContextIndexMapping.OpenIndex(index);
        }

        /// <inheritdoc />
        public async Task<ResultDetails<bool>> IndexOpenAsync(string index)
        {
            return await _elasticContextIndexMapping.OpenIndexAsync(index);
        }

        /// <inheritdoc />
        public ResultDetails<string> IndexUpdateSettings(IndexUpdateSettings indexUpdateSettings, string index = null)
        {
            return _elasticContextIndexMapping.UpdateIndexSettings(indexUpdateSettings, index);
        }

        /// <inheritdoc />
        public async Task<ResultDetails<string>> IndexUpdateSettingsAsync(IndexUpdateSettings indexUpdateSettings,
            string index = null)
        {
            return await _elasticContextIndexMapping.UpdateIndexSettingsAsync(indexUpdateSettings, index);
        }

        /// <inheritdoc />
        public ResultDetails<string> IndexCreate(string index, IndexSettings indexSettings = null,
            IndexAliases indexAliases = null, IndexWarmers indexWarmers = null)
        {
            return _elasticContextIndexMapping.CreateIndex(index, indexSettings, indexAliases, indexWarmers);
        }

        /// <inheritdoc />
        public async Task<ResultDetails<string>> IndexCreateAsync(string index, IndexSettings indexSettings = null,
            IndexAliases indexAliases = null, IndexWarmers indexWarmers = null)
        {
            return await _elasticContextIndexMapping.CreateIndexAsync(index, indexSettings, indexAliases, indexWarmers);
        }

        /// <inheritdoc />
        public ResultDetails<string> IndexCreate<T>(IndexDefinition indexDefinition = null)
        {
            return _elasticContextIndexMapping.CreateIndexWithMapping<T>(indexDefinition);
        }

        /// <inheritdoc />
        public async Task<ResultDetails<string>> IndexCreateAsync<T>(IndexDefinition indexDefinition = null)
        {
            return await _elasticContextIndexMapping.CreateIndexWithMappingAsync<T>(indexDefinition);
        }

        /// <inheritdoc />
        public ResultDetails<string> IndexCreateTypeMapping<T>(MappingDefinition mappingDefinition)
        {
            return _elasticContextIndexMapping.CreateTypeMappingForIndex<T>(mappingDefinition);
        }

        /// <inheritdoc />
        public async Task<ResultDetails<string>> IndexCreateTypeMappingAsync<T>(MappingDefinition mappingDefinition)
        {
            return await _elasticContextIndexMapping.CreateTypeMappingForIndexAsync<T>(mappingDefinition);
        }

        /// <inheritdoc />
        public T GetDocument<T>(object documentId, RoutingDefinition routingDefinition = null)
        {
            return _elasticContextGet.GetDocument<T>(documentId, routingDefinition);
        }

        /// <inheritdoc />
        public T SearchById<T>(object documentId, SearchUrlParameters searchUrlParameters = null)
        {
            return _elasticContextSearch.SearchById<T>(documentId, searchUrlParameters);
        }

        /// <inheritdoc />
        public GetResult Get(Uri uri)
        {
            return _elasticContextGet.Get(uri);
        }

        /// <inheritdoc />
        public async Task<ResultDetails<T>> SearchByIdAsync<T>(object documentId,
            SearchUrlParameters searchUrlParameters = null)
        {
            return await _elasticContextSearch.SearchByIdAsync<T>(documentId, searchUrlParameters);
        }

        /// <inheritdoc />
        public ResultDetails<SearchResult<T>> Search<T>(string searchJsonParameters,
            SearchUrlParameters searchUrlParameters = null)
        {
            return _searchRequest.PostSearch<T>(searchJsonParameters, null, null, searchUrlParameters);
        }

        /// <inheritdoc />
        public ResultDetails<SearchResult<T>> Search<T>(Model.SearchModel.Search search,
            SearchUrlParameters searchUrlParameters = null)
        {
            return _searchRequest.PostSearch<T>(search.ToString(), null, null, searchUrlParameters);
        }

        /// <inheritdoc />
        public async Task<ResultDetails<SearchResult<T>>> SearchAsync<T>(string searchJsonParameters,
            SearchUrlParameters searchUrlParameters = null)
        {
            return await _searchRequest.PostSearchAsync<T>(searchJsonParameters, null, null, searchUrlParameters);
        }

        public async Task<ResultDetails<SearchResult<T>>> SearchAsync<T>(Model.SearchModel.Search search,
            SearchUrlParameters searchUrlParameters = null)
        {
            return await _searchRequest.PostSearchAsync<T>(search.ToString(), null, null, searchUrlParameters);
        }

        /// <inheritdoc />
        public ResultDetails<SearchResult<T>> SearchScanAndScroll<T>(string scrollId,
            ScanAndScrollConfiguration scanAndScrollConfiguration)
        {
            if (string.IsNullOrEmpty(scrollId))
                throw new ElasticException("scrollId must have a value");

            if (scanAndScrollConfiguration == null)
                throw new ElasticException("scanAndScrollConfiguration not defined");

            return _searchRequest.PostSearch<T>("", scrollId, scanAndScrollConfiguration, null);
        }

        /// <inheritdoc />
        public async Task<ResultDetails<SearchResult<T>>> SearchScanAndScrollAsync<T>(string scrollId,
            ScanAndScrollConfiguration scanAndScrollConfiguration)
        {
            if (string.IsNullOrEmpty(scrollId))
                throw new ElasticException("scrollId must have a value");

            if (scanAndScrollConfiguration == null)
                throw new ElasticException("scanAndScrollConfiguration not defined");

            return await _searchRequest.PostSearchAsync<T>("", scrollId, scanAndScrollConfiguration, null);
        }

        /// <inheritdoc />
        public bool SearchExists<T>(string searchJsonParameters, string routing = null)
        {
            SearchUrlParameters searchUrlParameters = null;
            if (!string.IsNullOrEmpty(routing))
                searchUrlParameters = new SearchUrlParameters
                {
                    Routing = routing
                };
            return _searchRequest.PostSearchExists<T>(searchJsonParameters, searchUrlParameters);
        }

        /// <inheritdoc />
        public bool SearchExists<T>(Model.SearchModel.Search search, string routing = null)
        {
            SearchUrlParameters searchUrlParameters = null;
            if (!string.IsNullOrEmpty(routing))
                searchUrlParameters = new SearchUrlParameters
                {
                    Routing = routing
                };
            return _searchRequest.PostSearchExists<T>(search.ToString(), searchUrlParameters);
        }

        /// <inheritdoc />
        public async Task<ResultDetails<bool>> SearchExistsAsync<T>(string searchJsonParameters, string routing = null)
        {
            SearchUrlParameters searchUrlParameters = null;
            if (!string.IsNullOrEmpty(routing))
                searchUrlParameters = new SearchUrlParameters
                {
                    Routing = routing
                };
            return await _searchRequest.PostSearchExistsAsync<T>(searchJsonParameters, searchUrlParameters);
        }

        /// <inheritdoc />
        public async Task<ResultDetails<bool>> SearchExistsAsync<T>(Model.SearchModel.Search search, string routing = null)
        {
            SearchUrlParameters searchUrlParameters = null;
            if (!string.IsNullOrEmpty(routing))
                searchUrlParameters = new SearchUrlParameters
                {
                    Routing = routing
                };
            return await _searchRequest.PostSearchExistsAsync<T>(search.ToString(), searchUrlParameters);
        }

        /// <inheritdoc />
        public ResultDetails<SearchResult<T>> SearchCreateScanAndScroll<T>(string jsonContent, ScanAndScrollConfiguration scanAndScrollConfiguration)
        {
            return _searchRequest.PostSearchCreateScanAndScroll<T>(jsonContent, scanAndScrollConfiguration);
        }

        /// <inheritdoc />
        public ResultDetails<SearchResult<T>> SearchCreateScanAndScroll<T>(Model.SearchModel.Search search, ScanAndScrollConfiguration scanAndScrollConfiguration)
        {
            return _searchRequest.PostSearchCreateScanAndScroll<T>(search.ToString(), scanAndScrollConfiguration);
        }

        /// <inheritdoc />
        public async Task<ResultDetails<SearchResult<T>>> SearchCreateScanAndScrollAsync<T>(string jsonContent, ScanAndScrollConfiguration scanAndScrollConfiguration)
        {
            return await _searchRequest.PostSearchCreateScanAndScrollAsync<T>(jsonContent, scanAndScrollConfiguration);
        }

        /// <inheritdoc />
        public async Task<ResultDetails<SearchResult<T>>> SearchCreateScanAndScrollAsync<T>(Model.SearchModel.Search search, ScanAndScrollConfiguration scanAndScrollConfiguration)
        {
            return await _searchRequest.PostSearchCreateScanAndScrollAsync<T>(search.ToString(),
                scanAndScrollConfiguration);
        }

        /// <inheritdoc />
        public long Count<T>(string jsonContent = "")
        {
            return _elasticContextCount.PostCount<T>(jsonContent).PayloadResult;
        }

        /// <inheritdoc />
        public long Count<T>(Model.SearchModel.Search search)
        {
            return _elasticContextCount.PostCount<T>(search.ToString()).PayloadResult;
        }

        /// <inheritdoc />
        public async Task<ResultDetails<long>> CountAsync<T>(string jsonContent = "")
        {
            return await _elasticContextCount.PostCountAsync<T>(jsonContent);
        }

        /// <inheritdoc />
        public async Task<ResultDetails<long>> CountAsync<T>(Model.SearchModel.Search search)
        {
            return await _elasticContextCount.PostCountAsync<T>(search.ToString());
        }

        /// <inheritdoc />
        public async Task<ResultDetails<bool>> DeleteByQueryAsync<T>(string jsonContent)
        {
            if (string.IsNullOrEmpty(jsonContent))
                throw new ElasticException("Context: you must supply a json query for DeleteByQueryAsync");

            return await _elasticContextDeleteByQuery.DeleteByQueryAsync<T>(jsonContent);
        }

        /// <inheritdoc />
        public ResultDetails<bool> DeleteByQuery<T>(string jsonContent)
        {
            if (string.IsNullOrEmpty(jsonContent))
                throw new ElasticException("Context: you must supply a json query for DeleteByQueryAsync");

            return _elasticContextDeleteByQuery.SendDeleteByQuery<T>(jsonContent);
        }

        /// <inheritdoc />
        public ResultDetails<bool> DeleteByQuery<T>(Model.SearchModel.Search search)
        {
            if (search == null)
                throw new ElasticException("Context: you must supply a json query for DeleteByQueryAsync");

            return _elasticContextDeleteByQuery.SendDeleteByQuery<T>(search.ToString());
        }

        /// <inheritdoc />
        public async Task<ResultDetails<T>> GetDocumentAsync<T>(object documentId, RoutingDefinition routingDefinition = null)
        {
            return await _elasticContextGet.GetDocumentAsync<T>(documentId, routingDefinition);
        }

        /// <inheritdoc />
        public bool DocumentExists<T>(object documentId, RoutingDefinition routingDefinition = null)
        {
            return _elasticContextExists.Exists(
                _elasticContextExists.DocumentExistsAsync<T>(documentId, routingDefinition));
        }

        /// <inheritdoc />
        public async Task<ResultDetails<bool>> DocumentExistsAsync<T>(object documentId, RoutingDefinition routingDefinition = null)
        {
            return await _elasticContextExists.DocumentExistsAsync<T>(documentId, routingDefinition);
        }

        /// <inheritdoc />
        public bool Exists(Uri uri)
        {
            return _elasticContextExists.Exists(_elasticContextExists.ExistsAsync(uri));
        }

        /// <inheritdoc />
        public async Task<ResultDetails<bool>> ExistsAsync(Uri uri)
        {
            return await _elasticContextExists.ExistsAsync(uri);
        }

        /// <inheritdoc />
        public bool IndexExists<T>()
        {
            var syncExecutor = new SyncExecute(_traceProvider);
            return syncExecutor.Execute(() => _elasticContextExists.IndexExistsAsync<T>());
        }

        /// <inheritdoc />
        public async Task<ResultDetails<bool>> IndexExistsAsync<T>()
        {
            return await _elasticContextExists.IndexExistsAsync<T>();
        }

        /// <inheritdoc />
        public bool IndexTypeExists<T>()
        {
            return _elasticContextExists.Exists(_elasticContextExists.IndexTypeExistsAsync<T>());
        }

        /// <inheritdoc />
        public async Task<ResultDetails<bool>> IndexTypeExistsAsync<T>()
        {
            return await _elasticContextExists.IndexTypeExistsAsync<T>();
        }

        /// <inheritdoc />
        public bool AliasExistsForIndex<T>(string alias)
        {
            return _elasticContextExists.Exists(_elasticContextExists.AliasExistsForIndexAsync<T>(alias));
        }

        /// <inheritdoc />
        public async Task<ResultDetails<bool>> AliasExistsForIndexAsync<T>(string alias)
        {
            return await _elasticContextExists.AliasExistsForIndexAsync<T>(alias);
        }

        /// <inheritdoc />
        public bool AliasExists(string alias)
        {
            return _elasticContextExists.Exists(_elasticContextExists.AliasExistsAsync(alias));
        }

        /// <inheritdoc />
        public async Task<ResultDetails<bool>> AliasExistsAsync(string alias)
        {
            return await _elasticContextExists.AliasExistsAsync(alias);
        }

        /// <inheritdoc />
        public bool IndexClearCache<T>()
        {
            return _elasticContextClearCache.ClearCacheForIndex<T>();
        }

        /// <inheritdoc />
        public bool IndexClearCache(string index)
        {
            return _elasticContextClearCache.ClearCacheForIndex(index);
        }

        /// <inheritdoc />
        public async Task<ResultDetails<bool>> IndexClearCacheAsync<T>()
        {
            return await _elasticContextClearCache.ClearCacheForIndexAsync<T>();
        }

        /// <inheritdoc />
        public bool AliasCreateForIndex(string alias, string index)
        {
            var aliasParameters = new AliasParameters
            {
                Actions = new List<AliasBaseParameters>
                {
                    new AliasAddParameters(alias, index)
                }
            };

            return _elasticContextAlias.SendAliasCommand(aliasParameters.ToString());
        }

        /// <inheritdoc />
        public async Task<ResultDetails<bool>> AliasCreateForIndexAsync(string alias, string index)
        {
            var aliasParameters = new AliasParameters
            {
                Actions = new List<AliasBaseParameters>
                {
                    new AliasAddParameters(alias, index)
                }
            };
            return await _elasticContextAlias.SendAliasCommandAsync(aliasParameters.ToString());
        }

        /// <inheritdoc />
        public bool Alias(string jsonContent)
        {
            return _elasticContextAlias.SendAliasCommand(jsonContent);
        }

        /// <inheritdoc />
        public bool Alias(AliasParameters aliasParameters)
        {
            return _elasticContextAlias.SendAliasCommand(aliasParameters.ToString());
        }

        /// <inheritdoc />
        public async Task<ResultDetails<bool>> AliasAsync(string jsonContent)
        {
            return await _elasticContextAlias.SendAliasCommandAsync(jsonContent);
        }

        /// <inheritdoc />
        public bool WarmerCreate(Warmer warmer, string index = "", string type = "")
        {
            return _elasticContextWarmer.SendWarmerCommand(warmer, index, type);
        }

        /// <inheritdoc />
        public async Task<ResultDetails<bool>> WarmerCreateAsync(Warmer warmer, string index = "", string type = "")
        {
            return await _elasticContextWarmer.SendWarmerCommandAsync(warmer, index, type);
        }

        /// <inheritdoc />
        public bool WarmerDelete(string warmerName, string index)
        {
            return _elasticContextWarmer.SendWarmerDeleteCommand(warmerName, index);
        }

        /// <inheritdoc />
        public async Task<ResultDetails<bool>> WarmerDeleteAsync(string warmerName, string index = "")
        {
            return await _elasticContextWarmer.SendWarmerDeleteCommandAsync(warmerName, index);
        }

        /// <inheritdoc />
        public async Task<ResultDetails<bool>> AliasAsync(AliasParameters aliasParameters)
        {
            return await _elasticContextAlias.SendAliasCommandAsync(aliasParameters.ToString());
        }

        /// <inheritdoc />
        public bool AliasRemoveForIndex(string alias, string index)
        {
            var aliasParameters = new AliasParameters
            {
                Actions = new List<AliasBaseParameters>
                {
                    new AliasRemoveParameters(alias, index)
                }
            };
            return _elasticContextAlias.SendAliasCommand(aliasParameters.ToString());
        }

        /// <inheritdoc />
        public async Task<ResultDetails<bool>> AliasRemoveForIndexAsync(string alias, string index)
        {
            var aliasParameters = new AliasParameters
            {
                Actions = new List<AliasBaseParameters>
                {
                    new AliasRemoveParameters(alias, index)
                }
            };
            return await _elasticContextAlias.SendAliasCommandAsync(aliasParameters.ToString());
        }

        /// <inheritdoc />
        public bool AliasReplaceIndex(string alias, string indexOld, string indexNew)
        {
            var aliasParameters = new AliasParameters
            {
                Actions = new List<AliasBaseParameters>
                {
                    new AliasRemoveParameters(alias, indexOld),
                    new AliasAddParameters(alias, indexNew)
                }
            };
            return _elasticContextAlias.SendAliasCommand(aliasParameters.ToString());
        }

        /// <inheritdoc />
        public async Task<ResultDetails<bool>> AliasReplaceIndexAsync(string alias, string indexOld, string indexNew)
        {
            var aliasParameters = new AliasParameters
            {
                Actions = new List<AliasBaseParameters>
                {
                    new AliasRemoveParameters(alias, indexOld),
                    new AliasAddParameters(alias, indexNew)
                }
            };
            return await _elasticContextAlias.SendAliasCommandAsync(aliasParameters.ToString());
        }

        /// <inheritdoc />
        public async Task<ResultDetails<bool>> DeleteIndexAsync<T>()
        {
            return await _elasticContextAddDeleteUpdate.DeleteIndexAsync<T>(AllowDeleteForIndex);
        }

        /// <inheritdoc />
        public bool DeleteIndex<T>()
        {
            return _elasticContextAddDeleteUpdate.DeleteIndexAsync<T>(AllowDeleteForIndex).Result.PayloadResult;
        }

        /// <inheritdoc />
        public bool DeleteIndex(string index)
        {
            return _elasticContextAddDeleteUpdate.DeleteIndexAsync(AllowDeleteForIndex, index).Result.PayloadResult;
        }

        private void InitialContext()
        {
            _elasticContextExists = new ElasticContextExists(
                TraceProvider,
                _cancellationTokenSource,
                _elasticSerializerConfiguration,
                _client,
                _connectionString
            );

            _elasticContextAddDeleteUpdate = new ElasticContextAddDeleteUpdate(
                TraceProvider,
                _cancellationTokenSource,
                _elasticSerializerConfiguration,
                _client,
                _connectionString
            );

            _elasticContextGet = new ElasticContextGet(
                TraceProvider,
                _cancellationTokenSource,
                _elasticSerializerConfiguration,
                _client,
                _connectionString
            );

            _elasticContextSearch = new ElasticContextSearch(
                TraceProvider,
                _cancellationTokenSource,
                _elasticSerializerConfiguration,
                _client,
                _connectionString
            );

            _searchRequest = new SearchRequest(
                TraceProvider,
                _cancellationTokenSource,
                _elasticSerializerConfiguration,
                _client,
                _connectionString
            );

            _elasticContextCount = new ElasticContextCount(
                TraceProvider,
                _cancellationTokenSource,
                _elasticSerializerConfiguration,
                _client,
                _connectionString
            );

            _elasticContextDeleteByQuery = new ElasticContextDeleteByQuery(
                TraceProvider,
                _cancellationTokenSource,
                _elasticSerializerConfiguration,
                _client,
                _connectionString
            );

            _elasticContextClearCache = new ElasticContextClearCache(
                TraceProvider,
                _cancellationTokenSource,
                _elasticSerializerConfiguration,
                _client,
                _connectionString
            );

            _elasticContextAlias = new ElasticContextAlias(
                TraceProvider,
                _cancellationTokenSource,
                _elasticSerializerConfiguration,
                _client,
                _connectionString
            );

            _elasticContextWarmer = new ElasticContextWarmer(
                TraceProvider,
                _cancellationTokenSource,
                _elasticSerializerConfiguration,
                _client,
                _connectionString
            );

            _elasticContextIndexMapping = new ElasticContextIndexMapping(
                TraceProvider,
                _cancellationTokenSource,
                _elasticSerializerConfiguration,
                _client,
                _connectionString
            );
        }
    }
}