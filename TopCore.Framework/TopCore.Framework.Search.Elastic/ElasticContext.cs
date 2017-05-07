using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TopCore.Framework.Search.Elastic.ContentExists;
using TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate;
using TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel;
using TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.MappingModel;
using TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel;
using TopCore.Framework.Search.Elastic.ContextAlias;
using TopCore.Framework.Search.Elastic.ContextAlias.AliasModel;
using TopCore.Framework.Search.Elastic.ContextClearCache;
using TopCore.Framework.Search.Elastic.ContextCount;
using TopCore.Framework.Search.Elastic.ContextDeleteByQuery;
using TopCore.Framework.Search.Elastic.ContextGet;
using TopCore.Framework.Search.Elastic.ContextSearch;
using TopCore.Framework.Search.Elastic.ContextSearch.SearchModel;
using TopCore.Framework.Search.Elastic.ContextWarmers;
using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Tracing;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic
{
	/// <summary>
	///     Context for crud operations. 
	/// </summary>
	public class ElasticContext : IDisposable
	{
		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
		private readonly HttpClient _client = new HttpClient();
		private readonly List<EntityContextInfo> _entityPendingChanges = new List<EntityContextInfo>();
		private readonly string _connectionString;
		private readonly ElasticSerializerConfiguration _elasticSerializerConfiguration;

		private ElasticContextExists _elasticContextExists;
		private ElasticContextAddDeleteUpdate _elasticContextAddDeleteUpdate;
		private ElasticContextGet _elasticContextGet;
		private ElasticContextSearch _elasticContextSearch;
		private SearchRequest _searchRequest;
		private ElasticContextCount _elasticContextCount;
		private ElasticContextDeleteByQuery _elasticContextDeleteByQuery;
		private ElasticContextClearCache _elasticContextClearCache;
		private ElasticContextAlias _elasticContextAlias;
		private ElasticContextWarmer _elasticContextWarmer;

		/// <summary>
		///     TraceProvider for all logs, trace etc. This can be replaced with any TraceProvider implementation. 
		/// </summary>
		private ITraceProvider _traceProvider = new NullTraceProvider();

		private ElasticContextIndexMapping _elasticContextIndexMapping;

		public ITraceProvider TraceProvider
		{
			get { return _traceProvider; }
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

		/// <summary>
		///     Sets the HttpClient Basic Auth header. 
		/// </summary>
		/// <param name="credentials"> Basic auth credentials for logging into the server. </param>
		private void SetBasicAuthHeader(NetworkCredential credentials)
		{
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", credentials.UserName, credentials.Password))));
		}

		/// <summary>
		///     ctor 
		/// </summary>
		/// <param name="connectionString">               Connection string which is used as ther base URL for the HttpClient </param>
		/// <param name="elasticSerializerConfiguration"> Configuration class for the context. The default settings can be oset here. This class contains an IElasticMappingResolver </param>
		/// <param name="credentials">                    Basic auth credentials for logging into the server. </param>
		public ElasticContext(string connectionString, ElasticSerializerConfiguration elasticSerializerConfiguration, NetworkCredential credentials = null)
		{
			if (credentials != null)
			{
				SetBasicAuthHeader(credentials);
			}

			_elasticSerializerConfiguration = elasticSerializerConfiguration;
			_connectionString = connectionString;
			TraceProvider.Trace(TraceEventType.Verbose, "{1}: new ElasticContext with connection string: {0}", connectionString, "ElasticContext");
			InitialContext();
		}

		/// <summary>
		///     ctor 
		/// </summary>
		/// <param name="connectionString">       Connection string which is used as ther base URL for the HttpClient </param>
		/// <param name="elasticMappingResolver"> Resolver used for getting the index and type of a document type. The default ElasticSerializerConfiguration is used in this ctor. </param>
		/// <param name="credentials">            Basic auth credentials for logging into the server. </param>
		public ElasticContext(string connectionString, IElasticMappingResolver elasticMappingResolver, NetworkCredential credentials = null)
		{
			if (credentials != null)
			{
				SetBasicAuthHeader(credentials);
			}

			_elasticSerializerConfiguration = new ElasticSerializerConfiguration(elasticMappingResolver);
			_connectionString = connectionString;
			TraceProvider.Trace(TraceEventType.Verbose, "{1}: new ElasticContext with connection string: {0}", connectionString, "ElasticContext");
			InitialContext();
		}

		/// <summary>
		///     Adds a document to the pending changes list. Nor HTTP request is sent with this method. If the save changes is not called, the document is not added or updated in the search engine 
		/// </summary>
		/// <param name="document">          Document to be added or updated </param>
		/// <param name="id">                document id </param>
		/// <param name="routingDefinition">
		///     parent id of the document. This is only used if the ElasticSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex property is set to true. The document is then saved with the parent routing. It will also be saved even if the parent does not exist.
		/// </param>
		public void AddUpdateDocument(object document, object id, RoutingDefinition routingDefinition = null)
		{
			TraceProvider.Trace(TraceEventType.Verbose, "{2}: Adding document: {0}, {1} to pending list", document.GetType().Name, id, "ElasticContext");
			var data = new EntityContextInfo { Id = id, EntityType = document.GetType(), Document = document };
			if (routingDefinition != null)
			{
				data.RoutingDefinition = routingDefinition;
			}

			_entityPendingChanges.Add(data);
		}

		/// <summary>
		///     Adds a document to the pending changes list to be deletedd. Nor HTTP request is sent with this method. If the save changes is not called, the document is not added or updated in the search engine 
		/// </summary>
		/// <typeparam name="T"> This type is used to get the index and type of the document </typeparam>
		/// <param name="id">                id of the document which will be deleted. </param>
		/// <param name="routingDefinition"></param>
		public void DeleteDocument<T>(object id, RoutingDefinition routingDefinition = null)
		{
			if (routingDefinition == null)
			{
				routingDefinition = new RoutingDefinition();
			}
			TraceProvider.Trace(TraceEventType.Verbose, "{2}: Request to delete document with id: {0}, Type: {1}, adding to pending list", id, typeof(T).Name, "ElasticContext");
			_entityPendingChanges.Add(new EntityContextInfo { Id = id.ToString(), DeleteDocument = true, EntityType = typeof(T), Document = null, RoutingDefinition = routingDefinition });
		}

		/// <summary>
		///     Saves all the changes in the pending list of changes, add, update and delete. It also creates mappings and indexes if the child documents are saved as separate index types. ElasticSerializerConfiguration.ProcessChildDocumentsAsSeparateChildIndex= true 
		/// </summary>
		/// <returns> Returns HTTP response information </returns>
		public ResultDetails<string> SaveChangesAndInitMappings()
		{
			return _elasticContextAddDeleteUpdate.SaveChanges(_entityPendingChanges, true);
		}

		/// <summary>
		///     Saves all the changes in the pending list of changes, add, update and delete. No mappings are created here for child document types. 
		/// </summary>
		/// <returns> Returns HTTP response information </returns>
		public ResultDetails<string> SaveChanges()
		{
			return _elasticContextAddDeleteUpdate.SaveChanges(_entityPendingChanges, false);
		}

		/// <summary>
		///     async Saves all the changes in the pending list of changes, add, update and delete. No mappings are created here for child document types. 
		/// </summary>
		/// <returns> Returns HTTP response information </returns>
		public async Task<ResultDetails<string>> SaveChangesAsync()
		{
			return await _elasticContextAddDeleteUpdate.SaveChangesAsync(_entityPendingChanges);
		}

		/// <summary>
		///     The optimize API allows to optimize one or more indices through an API. The optimize process basically optimizes the index for faster search operations (and relates to the number of segments a Lucene index holds within each shard). The optimize operation allows to reduce the number of
		///     segments by merging them.
		/// </summary>
		/// <param name="index">              index to optimize </param>
		/// <param name="optimizeParameters"> all the possible parameters </param>
		/// <returns> number of successfully optimized </returns>
		public ResultDetails<OptimizeResult> IndexOptimize(string index = null, OptimizeParameters optimizeParameters = null)
		{
			return _elasticContextIndexMapping.IndexOptimize(index, optimizeParameters);
		}

		/// <summary>
		///     Async The optimize API allows to optimize one or more indices through an API. The optimize process basically optimizes the index for faster search operations (and relates to the number of segments a Lucene index holds within each shard). The optimize operation allows to reduce the
		///     number of segments by merging them.
		/// </summary>
		/// <param name="index">              index to optimize </param>
		/// <param name="optimizeParameters"> all the possible parameters </param>
		/// <returns> number of successfully optimized </returns>
		public async Task<ResultDetails<OptimizeResult>> IndexOptimizeAsync(string index = null, OptimizeParameters optimizeParameters = null)
		{
			return await _elasticContextIndexMapping.IndexOptimizeAsync(index, optimizeParameters);
		}

		/// <summary>
		///     The open and close index APIs allow to close an index, and later on opening it. A closed index has almost no overhead on the cluster (except for maintaining its metadata), and is blocked for read/write operations. A closed index can be opened which will then go through the normal
		///     recovery process.
		/// </summary>
		/// <param name="index"> index to be closed </param>
		/// <returns> true ids successfuly </returns>
		public ResultDetails<bool> IndexClose(string index)
		{
			return _elasticContextIndexMapping.CloseIndex(index);
		}

		/// <summary>
		///     Async The open and close index APIs allow to close an index, and later on opening it. A closed index has almost no overhead on the cluster (except for maintaining its metadata), and is blocked for read/write operations. A closed index can be opened which will then go through the
		///     normal recovery process.
		/// </summary>
		/// <param name="index"> index to be closed </param>
		/// <returns> true ids successfuly </returns>
		public async Task<ResultDetails<bool>> IndexCloseAsync(string index)
		{
			return await _elasticContextIndexMapping.CloseIndexAsync(index);
		}

		/// <summary>
		///     The open and close index APIs allow to close an index, and later on opening it. A closed index has almost no overhead on the cluster (except for maintaining its metadata), and is blocked for read/write operations. A closed index can be opened which will then go through the normal
		///     recovery process.
		/// </summary>
		/// <param name="index"> index to be opened </param>
		/// <returns> true ids successfuly </returns>
		public ResultDetails<bool> IndexOpen(string index)
		{
			return _elasticContextIndexMapping.OpenIndex(index);
		}

		/// <summary>
		///     Async The open and close index APIs allow to close an index, and later on opening it. A closed index has almost no overhead on the cluster (except for maintaining its metadata), and is blocked for read/write operations. A closed index can be opened which will then go through the
		///     normal recovery process.
		/// </summary>
		/// <param name="index"> index to be opened </param>
		/// <returns> true ids successfuly </returns>
		public async Task<ResultDetails<bool>> IndexOpenAsync(string index)
		{
			return await _elasticContextIndexMapping.OpenIndexAsync(index);
		}

		/// <summary>
		///     Change specific index level settings in real time Can change a single index or global changes 
		/// </summary>
		/// <param name="indexUpdateSettings"> index settings, see properties doc for details </param>
		/// <param name="index">               index to be updated, if null, updatesa all indices </param>
		/// <returns> string with details </returns>
		public ResultDetails<string> IndexUpdateSettings(IndexUpdateSettings indexUpdateSettings, string index = null)
		{
			return _elasticContextIndexMapping.UpdateIndexSettings(indexUpdateSettings, index);
		}

		/// <summary>
		///     Async Change specific index level settings in real time Can change a single index or global changes 
		/// </summary>
		/// <param name="indexUpdateSettings"> index settings, see properties doc for details </param>
		/// <param name="index">               index to be updated, if null, updatesa all indices </param>
		/// <returns> string with details </returns>
		public async Task<ResultDetails<string>> IndexUpdateSettingsAsync(IndexUpdateSettings indexUpdateSettings, string index = null)
		{
			return await _elasticContextIndexMapping.UpdateIndexSettingsAsync(indexUpdateSettings, index);
		}

		/// <summary>
		///     Creates a new index 
		/// </summary>
		/// <param name="index">         index name to lower string! </param>
		/// <param name="indexSettings"> settings for the new index </param>
		/// <param name="indexAliases">  Define aliases for the index at creation time </param>
		/// <param name="indexWarmers">  Warmers for index or type </param>
		/// <returns> details </returns>
		public ResultDetails<string> IndexCreate(string index, IndexSettings indexSettings = null, IndexAliases indexAliases = null, IndexWarmers indexWarmers = null)
		{
			return _elasticContextIndexMapping.CreateIndex(index, indexSettings, indexAliases, indexWarmers);
		}

		/// <summary>
		///     Async Creates a new index 
		/// </summary>
		/// <param name="index">         index name to lower string! </param>
		/// <param name="indexSettings"> settings for the new index </param>
		/// <param name="indexAliases">  Define aliases for the index at creation time </param>
		/// <param name="indexWarmers">  Warmers for index or type </param>
		/// <returns> details </returns>
		public async Task<ResultDetails<string>> IndexCreateAsync(string index, IndexSettings indexSettings = null, IndexAliases indexAliases = null, IndexWarmers indexWarmers = null)
		{
			return await _elasticContextIndexMapping.CreateIndexAsync(index, indexSettings, indexAliases, indexWarmers);
		}

		/// <summary>
		///     Creates a new index from a Type and also all the property mappings and index definitions
		///     Note: Child objects cannot be an interface if the mapping should be created first.
		/// </summary>
		/// <param name="indexDefinition"> settings for the new index </param>
		/// <returns> details </returns>
		public ResultDetails<string> IndexCreate<T>(IndexDefinition indexDefinition = null)
		{
			return _elasticContextIndexMapping.CreateIndexWithMapping<T>(indexDefinition);
		}

		/// <summary>
		///     Async Creates a new index from a Type and also all the property mappings and index definitions 
		/// </summary>
		/// <param name="indexDefinition"> settings for the new index </param>
		/// <returns> details </returns>
		public async Task<ResultDetails<string>> IndexCreateAsync<T>(IndexDefinition indexDefinition = null)
		{
			return await _elasticContextIndexMapping.CreateIndexWithMappingAsync<T>(indexDefinition);
		}

		/// <summary>
		///     Creates propety mappings for an existing index 
		/// </summary>
		/// <typeparam name="T"> Type for the mapping </typeparam>
		/// <param name="mappingDefinition"> Routing index definitions </param>
		/// <returns> details of the request </returns>
		public ResultDetails<string> IndexCreateTypeMapping<T>(MappingDefinition mappingDefinition)
		{
			return _elasticContextIndexMapping.CreateTypeMappingForIndex<T>(mappingDefinition);
		}

		/// <summary>
		///     Async Creates propety mappings for an existing index 
		/// </summary>
		/// <typeparam name="T"> Type for the mapping </typeparam>
		/// <param name="mappingDefinition"> Routing index definitions </param>
		/// <returns> details of the request </returns>
		public async Task<ResultDetails<string>> IndexCreateTypeMappingAsync<T>(MappingDefinition mappingDefinition)
		{
			return await _elasticContextIndexMapping.CreateTypeMappingForIndexAsync<T>(mappingDefinition);
		}

		/// <summary>
		///     Gets a document by id. Elastic GET API 
		/// </summary>
		/// <typeparam name="T"> type used for the document index and type definition </typeparam>
		/// <param name="documentId">        document id </param>
		/// <param name="routingDefinition">
		///     Parent Id of the document if document is a child document Id the Id is incorrect, you may still recieve the child document (parentId might be saved to the same shard.) If the child is a child document and no parent id is set, no docuemnt will be found.
		/// </param>
		/// <returns> Document type T </returns>
		public T GetDocument<T>(object documentId, RoutingDefinition routingDefinition = null)
		{
			return _elasticContextGet.GetDocument<T>(documentId, routingDefinition);
		}

		/// <summary>
		///     Uses Elastic search API to get the document per id 
		/// </summary>
		/// <typeparam name="T"> type T used to get index anf the type of the document. </typeparam>
		/// <param name="documentId">         </param>
		/// <param name="searchUrlParameters"> add routing or pretty parameters if required </param>
		/// <returns> Returns the document of type T </returns>
		public T SearchById<T>(object documentId, SearchUrlParameters searchUrlParameters = null)
		{
			return _elasticContextSearch.SearchById<T>(documentId, searchUrlParameters);
		}

		public GetResult Get(Uri uri)
		{
			return _elasticContextGet.Get(uri);
		}

		/// <summary>
		///     async Uses Elastic search API to get the document per id 
		/// </summary>
		/// <typeparam name="T"> type T used to get index anf the type of the document. </typeparam>
		/// <param name="documentId">         </param>
		/// <param name="searchUrlParameters"> add routing or pretty parameters if required </param>
		/// <returns> Returns the document of type T in a Task with result details </returns>
		public async Task<ResultDetails<T>> SearchByIdAsync<T>(object documentId, SearchUrlParameters searchUrlParameters = null)
		{
			return await _elasticContextSearch.SearchByIdAsync<T>(documentId, searchUrlParameters);
		}

		/// <summary>
		///     Search API method to search for anything. Any json string which matches the Elastic Search API can be used. Only single index and type search 
		/// </summary>
		/// <typeparam name="T"> Type T used for the index and tpye used in the search </typeparam>
		/// <param name="searchJsonParameters"> JSON string which matches the Elastic Search API </param>
		/// <param name="searchUrlParameters">  add routing or pretty parameters if required </param>
		/// <returns> A collection of documents of type T </returns>
		public ResultDetails<SearchResult<T>> Search<T>(string searchJsonParameters, SearchUrlParameters searchUrlParameters = null)
		{
			return _searchRequest.PostSearch<T>(searchJsonParameters, null, null, searchUrlParameters);
		}

		/// <summary>
		///     Search API method to search for anything. Any json string which matches the Elastic Search API can be used. Only single index and type search 
		/// </summary>
		/// <typeparam name="T"> Type T used for the index and tpye used in the search </typeparam>
		/// <param name="search">              search body for Elastic Search API </param>
		/// <param name="searchUrlParameters"> add routing or pretty parameters if required </param>
		/// <returns> A collection of documents of type T </returns>
		public ResultDetails<SearchResult<T>> Search<T>(Model.SearchModel.Search search, SearchUrlParameters searchUrlParameters = null)
		{
			return _searchRequest.PostSearch<T>(search.ToString(), null, null, searchUrlParameters);
		}

		/// <summary>
		///     async Search API method to search for anything. Any json string which matches the Elastic Search API can be used. Only single index and type search 
		/// </summary>
		/// <typeparam name="T"> Type T used for the index and tpye used in the search </typeparam>
		/// <param name="searchJsonParameters"> JSON string which matches the Elastic Search API </param>
		/// <param name="searchUrlParameters">  add routing or pretty parameters if required </param>
		/// <returns> A collection of documents of type T in a Task </returns>
		public async Task<ResultDetails<SearchResult<T>>> SearchAsync<T>(string searchJsonParameters, SearchUrlParameters searchUrlParameters = null)
		{
			return await _searchRequest.PostSearchAsync<T>(searchJsonParameters, null, null, searchUrlParameters);
		}

		public async Task<ResultDetails<SearchResult<T>>> SearchAsync<T>(Model.SearchModel.Search search, SearchUrlParameters searchUrlParameters = null)
		{
			return await _searchRequest.PostSearchAsync<T>(search.ToString(), null, null, searchUrlParameters);
		}

		/// <summary>
		///     Search API method to search for anything. Any json string which matches the Elastic Search API can be used. Only single index and type search 
		/// </summary>
		/// <typeparam name="T"> Type T used for the index and tpye used in the search </typeparam>
		/// <param name="scrollId">                   If this search is part of a scan and scroll, you can add the scrollId to open the context </param>
		/// <param name="scanAndScrollConfiguration"> Required scroll configuration </param>
		/// <returns> A collection of documents of type T </returns>
		public ResultDetails<SearchResult<T>> SearchScanAndScroll<T>(string scrollId, ScanAndScrollConfiguration scanAndScrollConfiguration)
		{
			if (string.IsNullOrEmpty(scrollId))
			{
				throw new ElasticException("scrollId must have a value");
			}

			if (scanAndScrollConfiguration == null)
			{
				throw new ElasticException("scanAndScrollConfiguration not defined");
			}

			return _searchRequest.PostSearch<T>("", scrollId, scanAndScrollConfiguration, null);
		}

		/// <summary>
		///     async Search API method to search for anything. Any json string which matches the Elastic Search API can be used. Only single index and type search 
		/// </summary>
		/// <typeparam name="T"> Type T used for the index and tpye used in the search </typeparam>
		/// <param name="scrollId">                   If this search is part of a scan and scroll, you can add the scrollId to open the context </param>
		/// <param name="scanAndScrollConfiguration"> Required scroll configuration </param>
		/// <returns> A collection of documents of type T in a Task </returns>
		public async Task<ResultDetails<SearchResult<T>>> SearchScanAndScrollAsync<T>(string scrollId, ScanAndScrollConfiguration scanAndScrollConfiguration)
		{
			if (string.IsNullOrEmpty(scrollId))
			{
				throw new ElasticException("scrollId must have a value");
			}

			if (scanAndScrollConfiguration == null)
			{
				throw new ElasticException("scanAndScrollConfiguration not defined");
			}

			return await _searchRequest.PostSearchAsync<T>("", scrollId, scanAndScrollConfiguration, null);
		}

		/// <summary>
		///     executes a post request to checks if at least one document exists for the search query. 
		/// </summary>
		/// <typeparam name="T"> Type used to define the type and index in elsticsearch </typeparam>
		/// <param name="searchJsonParameters"> json query for elastic </param>
		/// <param name="routing">              routing used for the search </param>
		/// <returns> true if one document exists for the search query </returns>
		public bool SearchExists<T>(string searchJsonParameters, string routing = null)
		{
			SearchUrlParameters searchUrlParameters = null;
			if (!string.IsNullOrEmpty(routing))
			{
				searchUrlParameters = new SearchUrlParameters
				{
					Routing = routing
				};
			}
			return _searchRequest.PostSearchExists<T>(searchJsonParameters, searchUrlParameters);
		}

		/// <summary>
		///     executes a post request to checks if at least one document exists for the search query. 
		/// </summary>
		/// <typeparam name="T"> Type used to define the type and index in elsticsearch </typeparam>
		/// <param name="search">  search body for Elastic Search API </param>
		/// <param name="routing"> routing used for the search </param>
		/// <returns> true if one document exists for the search query </returns>
		public bool SearchExists<T>(Model.SearchModel.Search search, string routing = null)
		{
			SearchUrlParameters searchUrlParameters = null;
			if (!string.IsNullOrEmpty(routing))
			{
				searchUrlParameters = new SearchUrlParameters
				{
					Routing = routing
				};
			}
			return _searchRequest.PostSearchExists<T>(search.ToString(), searchUrlParameters);
		}

		/// <summary>
		///     async executes a post request to checks if at least one document exists for the search query. 
		/// </summary>
		/// <typeparam name="T"> Type used to define the type and index in elsticsearch </typeparam>
		/// <param name="searchJsonParameters"> json query for elastic </param>
		/// <param name="routing">              routing used for the search </param>
		/// <returns> true if one document exists for the search query </returns>
		public async Task<ResultDetails<bool>> SearchExistsAsync<T>(string searchJsonParameters, string routing = null)
		{
			SearchUrlParameters searchUrlParameters = null;
			if (!string.IsNullOrEmpty(routing))
			{
				searchUrlParameters = new SearchUrlParameters
				{
					Routing = routing
				};
			}
			return await _searchRequest.PostSearchExistsAsync<T>(searchJsonParameters, searchUrlParameters);
		}

		/// <summary>
		///     async executes a post request to checks if at least one document exists for the search query. 
		/// </summary>
		/// <typeparam name="T"> Type used to define the type and index in elsticsearch </typeparam>
		/// <param name="search">  search body for Elastic Search API </param>
		/// <param name="routing"> routing used for the search </param>
		/// <returns> true if one document exists for the search query </returns>
		public async Task<ResultDetails<bool>> SearchExistsAsync<T>(Model.SearchModel.Search search, string routing = null)
		{
			SearchUrlParameters searchUrlParameters = null;
			if (!string.IsNullOrEmpty(routing))
			{
				searchUrlParameters = new SearchUrlParameters
				{
					Routing = routing
				};
			}
			return await _searchRequest.PostSearchExistsAsync<T>(search.ToString(), searchUrlParameters);
		}

		/// <summary>
		///     Creates a new scan and scroll search. Takes the query json content and returns a _scroll_id in the payload for the following searches. If your doing a live reindexing, you should use a timestamp in the json content query. 
		/// </summary>
		/// <typeparam name="T"> index and type formt search scan and scroll </typeparam>
		/// <param name="jsonContent">                query which will be saved. </param>
		/// <param name="scanAndScrollConfiguration"> The scan and scroll configuration, for example scroll in time units </param>
		/// <returns> Returns the _scroll_id in the Payload property and the total number of hits. </returns>
		public ResultDetails<SearchResult<T>> SearchCreateScanAndScroll<T>(string jsonContent, ScanAndScrollConfiguration scanAndScrollConfiguration)
		{
			return _searchRequest.PostSearchCreateScanAndScroll<T>(jsonContent, scanAndScrollConfiguration);
		}

		/// <summary>
		///     Creates a new scan and scroll search. Takes the query json content and returns a _scroll_id in the payload for the following searches. If your doing a live reindexing, you should use a timestamp in the json content query. 
		/// </summary>
		/// <typeparam name="T"> index and type formt search scan and scroll </typeparam>
		/// <param name="search">                     search body for Elastic Search API </param>
		/// <param name="scanAndScrollConfiguration"> The scan and scroll configuration, for example scroll in time units </param>
		/// <returns> Returns the _scroll_id in the Payload property and the total number of hits. </returns>
		public ResultDetails<SearchResult<T>> SearchCreateScanAndScroll<T>(Model.SearchModel.Search search, ScanAndScrollConfiguration scanAndScrollConfiguration)
		{
			return _searchRequest.PostSearchCreateScanAndScroll<T>(search.ToString(), scanAndScrollConfiguration);
		}

		/// <summary>
		///     Async Creates a new scan and scroll search. Takes the query json content and returns a _scroll_id in the payload for the following searches. If your doing a live reindexing, you should use a timestamp in the json content query. 
		/// </summary>
		/// <typeparam name="T"> index and type formt search scan and scroll </typeparam>
		/// <param name="jsonContent">                query which will be saved. </param>
		/// <param name="scanAndScrollConfiguration"> The scan and scroll configuration, for example scroll in time units </param>
		/// <returns> Returns the _scroll_id in the Payload property and the total number of hits. </returns>
		public async Task<ResultDetails<SearchResult<T>>> SearchCreateScanAndScrollAsync<T>(string jsonContent, ScanAndScrollConfiguration scanAndScrollConfiguration)
		{
			return await _searchRequest.PostSearchCreateScanAndScrollAsync<T>(jsonContent, scanAndScrollConfiguration);
		}

		/// <summary>
		///     Async Creates a new scan and scroll search. Takes the query json content and returns a _scroll_id in the payload for the following searches. If your doing a live reindexing, you should use a timestamp in the json content query. 
		/// </summary>
		/// <typeparam name="T"> index and type formt search scan and scroll </typeparam>
		/// <param name="search">                     search body for Elastic Search API </param>
		/// <param name="scanAndScrollConfiguration"> The scan and scroll configuration, for example scroll in time units </param>
		/// <returns> Returns the _scroll_id in the Payload property and the total number of hits. </returns>
		public async Task<ResultDetails<SearchResult<T>>> SearchCreateScanAndScrollAsync<T>(Model.SearchModel.Search search, ScanAndScrollConfiguration scanAndScrollConfiguration)
		{
			return await _searchRequest.PostSearchCreateScanAndScrollAsync<T>(search.ToString(), scanAndScrollConfiguration);
		}

		/// <summary>
		///     ElasticContextCount to amount of hits for a index, type and query. 
		/// </summary>
		/// <typeparam name="T"> Type to find </typeparam>
		/// <param name="jsonContent"> json query data, returns all in emtpy </param>
		/// <returns> Result amount of document found </returns>
		public long Count<T>(string jsonContent = "")
		{
			return _elasticContextCount.PostCount<T>(jsonContent).PayloadResult;
		}

		/// <summary>
		///     ElasticContextCount to amount of hits for a index, type and query. 
		/// </summary>
		/// <typeparam name="T"> Type to find </typeparam>
		/// <param name="search"> search body for Elastic Search API </param>
		/// <returns> Result amount of document found </returns>
		public long Count<T>(Model.SearchModel.Search search)
		{
			return _elasticContextCount.PostCount<T>(search.ToString()).PayloadResult;
		}

		/// <summary>
		///     ElasticContextCount to amount of hits for a index, type and query. 
		/// </summary>
		/// <typeparam name="T"> Type to find </typeparam>
		/// <param name="jsonContent"> json query data, returns all in emtpy </param>
		/// <returns> Result amount of document found in a result details task </returns>
		public async Task<ResultDetails<long>> CountAsync<T>(string jsonContent = "")
		{
			return await _elasticContextCount.PostCountAsync<T>(jsonContent);
		}

		/// <summary>
		///     ElasticContextCount to amount of hits for a index, type and query. 
		/// </summary>
		/// <typeparam name="T"> Type to find </typeparam>
		/// <param name="search"> search body for Elastic Search API </param>
		/// <returns> Result amount of document found in a result details task </returns>
		public async Task<ResultDetails<long>> CountAsync<T>(Model.SearchModel.Search search)
		{
			return await _elasticContextCount.PostCountAsync<T>(search.ToString());
		}

		/// <summary>
		///     Async Deletes all documents found using the query in the body. 
		/// </summary>
		/// <typeparam name="T"> Type used to define the index and the type in Elastic </typeparam>
		/// <param name="jsonContent"> json string using directly in Elastic API. </param>
		/// <returns> Returns true if ok </returns>
		public async Task<ResultDetails<bool>> DeleteByQueryAsync<T>(string jsonContent)
		{
			if (string.IsNullOrEmpty(jsonContent))
			{
				throw new ElasticException("Context: you must supply a json query for DeleteByQueryAsync");
			}

			return await _elasticContextDeleteByQuery.DeleteByQueryAsync<T>(jsonContent);
		}

		/// <summary>
		///     Deletes all documents found using the query in the body. 
		/// </summary>
		/// <typeparam name="T"> Type used to define the index and the type in Elastic </typeparam>
		/// <param name="jsonContent"> json string using directly in Elastic API. </param>
		/// <returns> Returns true if ok </returns>
		public ResultDetails<bool> DeleteByQuery<T>(string jsonContent)
		{
			if (string.IsNullOrEmpty(jsonContent))
			{
				throw new ElasticException("Context: you must supply a json query for DeleteByQueryAsync");
			}

			return _elasticContextDeleteByQuery.SendDeleteByQuery<T>(jsonContent);
		}

		/// <summary>
		///     Deletes all documents found using the query in the body. 
		/// </summary>
		/// <typeparam name="T"> Type used to define the index and the type in Elastic </typeparam>
		/// <param name="search"> search body for Elastic Search API </param>
		/// <returns> Returns true if ok </returns>
		public ResultDetails<bool> DeleteByQuery<T>(Model.SearchModel.Search search)
		{
			if (search == null)
			{
				throw new ElasticException("Context: you must supply a json query for DeleteByQueryAsync");
			}

			return _elasticContextDeleteByQuery.SendDeleteByQuery<T>(search.ToString());
		}

		/// <summary>
		///     Gets a document by id. Elastic GET API 
		/// </summary>
		/// <typeparam name="T"> type used for the document index and type definition </typeparam>
		/// <param name="documentId">        document id </param>
		/// <param name="routingDefinition">
		///     Parent Id of the document if document is a child document Id the Id is incorrect, you may still recieve the child document (parentId might be saved to the same shard.) If the child is a child document and no parent id is set, no docuemnt will be found.
		/// </param>
		/// <returns> Document type T in a Task with result details </returns>
		public async Task<ResultDetails<T>> GetDocumentAsync<T>(object documentId, RoutingDefinition routingDefinition = null)
		{
			return await _elasticContextGet.GetDocumentAsync<T>(documentId, routingDefinition);
		}

		/// <summary>
		///     Checks if a document exists with a head request 
		/// </summary>
		/// <typeparam name="T"> Type of document to find </typeparam>
		/// <param name="documentId">        Id of the document </param>
		/// <param name="routingDefinition">
		///     parent Id, required if hte docuemtnis a child document and routing is required.
		///     NOTE: if the parent Id is incorrect but save on the same shard as the correct parentId, the document will be found!
		/// </param>
		/// <returns> true or false </returns>
		public bool DocumentExists<T>(object documentId, RoutingDefinition routingDefinition = null)
		{
			return _elasticContextExists.Exists(_elasticContextExists.DocumentExistsAsync<T>(documentId, routingDefinition));
		}

		/// <summary>
		///     Async Checks if a document exists with a head request 
		/// </summary>
		/// <typeparam name="T"> Type of document to find </typeparam>
		/// <param name="documentId">        Id of the document </param>
		/// <param name="routingDefinition">
		///     parent Id, required if hte docuemtnis a child document and routing is required.
		///     NOTE: if the parent Id is incorrect but save on the same shard as the correct parentId, the document will be found!
		/// </param>
		/// <returns> true or false </returns>
		public async Task<ResultDetails<bool>> DocumentExistsAsync<T>(object documentId, RoutingDefinition routingDefinition = null)
		{
			return await _elasticContextExists.DocumentExistsAsync<T>(documentId, routingDefinition);
		}

		/// <summary>
		///     Send a HEAD request to Eleasticseach to find out if the item defined in the URL exists 
		/// </summary>
		/// <param name="uri"> Full URI of Elasticseach plus item </param>
		/// <returns> true if it exists false for 404 </returns>
		public bool Exists(Uri uri)
		{
			return _elasticContextExists.Exists(_elasticContextExists.ExistsAsync(uri));
		}

		/// <summary>
		///     Async Send a HEAD request to Eleasticseach to find out if the item defined in the URL exists 
		/// </summary>
		/// <param name="uri"> Full URI of Elasticseach plus item </param>
		/// <returns> true if it exists false for 404 </returns>
		public async Task<ResultDetails<bool>> ExistsAsync(Uri uri)
		{
			return await _elasticContextExists.ExistsAsync(uri);
		}

		/// <summary>
		///     async Checks if a index exists for the type T 
		/// </summary>
		/// <typeparam name="T"> Type used for the index exists HEAD request. Gets the index using the mapping </typeparam>
		/// <returns> true if it exists false for 404 </returns>
		public bool IndexExists<T>()
		{
			var syncExecutor = new SyncExecute(_traceProvider);
			return syncExecutor.Execute(() => _elasticContextExists.IndexExistsAsync<T>());
		}

		/// <summary>
		///     async Checks if a index exists for the type T 
		/// </summary>
		/// <typeparam name="T"> Type used for the index exists HEAD request. Gets the index using the mapping </typeparam>
		/// <returns> true if it exists false for 404 </returns>
		public async Task<ResultDetails<bool>> IndexExistsAsync<T>()
		{
			return await _elasticContextExists.IndexExistsAsync<T>();
		}

		/// <summary>
		///     Checks if a type exists for an index for the type T 
		/// </summary>
		/// <typeparam name="T"> Type used for the index + plus exists HEAD request. Gets the index using the mapping </typeparam>
		/// <returns> true if it exists false for 404 </returns>
		public bool IndexTypeExists<T>()
		{
			return _elasticContextExists.Exists(_elasticContextExists.IndexTypeExistsAsync<T>());
		}

		/// <summary>
		///     Checks if a type exists for an index for the type T 
		/// </summary>
		/// <typeparam name="T"> Type used for the index + plus exists HEAD request. Gets the index using the mapping </typeparam>
		/// <returns> true if it exists false for 404 </returns>
		public async Task<ResultDetails<bool>> IndexTypeExistsAsync<T>()
		{
			return await _elasticContextExists.IndexTypeExistsAsync<T>();
		}

		/// <summary>
		///     Checks if an alias exists for an index for the type T 
		/// </summary>
		/// <typeparam name="T"> Type used for the index + plus exists HEAD request. Gets the index using the mapping </typeparam>
		/// <returns> true if the alias exists false for 404 </returns>
		public bool AliasExistsForIndex<T>(string alias)
		{
			return _elasticContextExists.Exists(_elasticContextExists.AliasExistsForIndexAsync<T>(alias));
		}

		/// <summary>
		///     async Checks if an alias exists for an index for the type T 
		/// </summary>
		/// <typeparam name="T"> Type used for the index + plus exists HEAD request. Gets the index using the mapping </typeparam>
		/// <returns> true if the alias exists false for 404 </returns>
		public async Task<ResultDetails<bool>> AliasExistsForIndexAsync<T>(string alias)
		{
			return await _elasticContextExists.AliasExistsForIndexAsync<T>(alias);
		}

		/// <summary>
		///     Checks if an alias exists 
		/// </summary>
		/// <returns> true if the alias exists false for 404 </returns>
		public bool AliasExists(string alias)
		{
			return _elasticContextExists.Exists(_elasticContextExists.AliasExistsAsync(alias));
		}

		/// <summary>
		///     async Checks if an alias exists 
		/// </summary>
		/// <returns> true if the alias exists false for 404 </returns>
		public async Task<ResultDetails<bool>> AliasExistsAsync(string alias)
		{
			return await _elasticContextExists.AliasExistsAsync(alias);
		}

		/// <summary>
		///     Clears the cache for the index. The index is defined using the Type 
		/// </summary>
		/// <typeparam name="T"> Type used to get the index name </typeparam>
		/// <returns> returns true if cache has been cleared </returns>
		public bool IndexClearCache<T>()
		{
			return _elasticContextClearCache.ClearCacheForIndex<T>();
		}

		/// <summary>
		///     Clears the cache for the index. The index is defined using the Type 
		/// </summary>
		/// <returns> returns true if cache has been cleared </returns>
		public bool IndexClearCache(string index)
		{
			return _elasticContextClearCache.ClearCacheForIndex(index);
		}

		/// <summary>
		///     Async Clears the cache for the index. The index is defined using the Type 
		/// </summary>
		/// <typeparam name="T"> Type used to get the index name </typeparam>
		/// <returns> returns true if cache has been cleared </returns>
		public async Task<ResultDetails<bool>> IndexClearCacheAsync<T>()
		{
			return await _elasticContextClearCache.ClearCacheForIndexAsync<T>();
		}

		/// <summary>
		///     Creates a new alias for the index parameter. 
		/// </summary>
		/// <param name="alias"> name of the alias </param>
		/// <param name="index"> index for the alias </param>
		/// <returns> true if the alias was created </returns>
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

		/// <summary>
		///     Async Creates a new alias for the index parameter. 
		/// </summary>
		/// <param name="alias"> name of the alias </param>
		/// <param name="index"> index for the alias </param>
		/// <returns> true if the alias was created </returns>
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

		/// <summary>
		///     Creates any alias command depending on the json content 
		/// </summary>
		/// <param name="jsonContent"> content for the _aliases, see Elastic documentation </param>
		/// <returns> returns true if the alias command was completed successfully </returns>
		public bool Alias(string jsonContent)
		{
			return _elasticContextAlias.SendAliasCommand(jsonContent);
		}

		/// <summary>
		///     Creates any alias command depending on the json content var aliasParameters = new AliasParameters { Actions = new List AliasBaseParameters { new AliasAddParameters("test2", "indexaliasdtotests"), new AliasAddParameters("test3", "indexaliasdtotests") } }; 
		/// </summary>
		/// <param name="aliasParameters"> content for the _aliases, see Elastic documentation </param>
		/// <returns> returns true if the alias command was completed successfully </returns>
		public bool Alias(AliasParameters aliasParameters)
		{
			return _elasticContextAlias.SendAliasCommand(aliasParameters.ToString());
		}

		/// <summary>
		///     Async Creates any alias command depending on the json content 
		/// </summary>
		/// <param name="jsonContent"> content for the _aliases, see Elastic documentation </param>
		/// <returns> returns true if the alias command was completed successfully </returns>
		public async Task<ResultDetails<bool>> AliasAsync(string jsonContent)
		{
			return await _elasticContextAlias.SendAliasCommandAsync(jsonContent);
		}

		/// <summary>
		///     Create a new warmer 
		/// </summary>
		/// <param name="warmer"> Wamrer with Query or Agg </param>
		/// <param name="index">  index if required </param>
		/// <param name="type">   type if required </param>
		/// <returns> true if created </returns>
		public bool WarmerCreate(Warmer warmer, string index = "", string type = "")
		{
			return _elasticContextWarmer.SendWarmerCommand(warmer, index, type);
		}

		/// <summary>
		///     Create a new warmer async 
		/// </summary>
		/// <param name="warmer"> Wamrer with Query or Agg </param>
		/// <param name="index">  index if required </param>
		/// <param name="type">   type if required </param>
		/// <returns> true if created </returns>
		public async Task<ResultDetails<bool>> WarmerCreateAsync(Warmer warmer, string index = "", string type = "")
		{
			return await _elasticContextWarmer.SendWarmerCommandAsync(warmer, index, type);
		}

		/// <summary>
		///     deletes a warmer 
		/// </summary>
		/// <param name="warmerName"> name of the warmer </param>
		/// <param name="index">      index </param>
		/// <returns> true if ok </returns>
		public bool WarmerDelete(string warmerName, string index)
		{
			return _elasticContextWarmer.SendWarmerDeleteCommand(warmerName, index);
		}

		/// <summary>
		///     deletes a warmer async 
		/// </summary>
		/// <param name="warmerName"> name of the warmer </param>
		/// <param name="index">      index </param>
		/// <returns> true if ok </returns>
		public async Task<ResultDetails<bool>> WarmerDeleteAsync(string warmerName, string index = "")
		{
			return await _elasticContextWarmer.SendWarmerDeleteCommandAsync(warmerName, index);
		}

		/// <summary>
		///     Async Creates any alias command depending on the json content 
		/// </summary>
		/// <param name="aliasParameters"> content for the _aliases, see Elastic documentation </param>
		/// <returns> returns true if the alias command was completed successfully </returns>
		public async Task<ResultDetails<bool>> AliasAsync(AliasParameters aliasParameters)
		{
			return await _elasticContextAlias.SendAliasCommandAsync(aliasParameters.ToString());
		}

		/// <summary>
		///     Removes a new alias for the index parameter. 
		/// </summary>
		/// <param name="alias"> name of the alias </param>
		/// <param name="index"> index for the alias </param>
		/// <returns> true if the alias was removed </returns>
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

		/// <summary>
		///     asnyc Removes a new alias for the index parameter. 
		/// </summary>
		/// <param name="alias"> name of the alias </param>
		/// <param name="index"> index for the alias </param>
		/// <returns> true if the alias was removed </returns>
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

		/// <summary>
		///     Replaces the index for the alias. This can be used when reindexing live 
		/// </summary>
		/// <param name="alias">    Name of the alias </param>
		/// <param name="indexOld"> Old index which will be removed </param>
		/// <param name="indexNew"> New index which will be mapped to the alias </param>
		/// <returns> Returns true if the index was replaced </returns>
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

		/// <summary>
		///     Async Replaces the index for the alias. This can be used when reindexing live 
		/// </summary>
		/// <param name="alias">    Name of the alias </param>
		/// <param name="indexOld"> Old index which will be removed </param>
		/// <param name="indexNew"> New index which will be mapped to the alias </param>
		/// <returns> Returns true if the index was replaced </returns>
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

		/// <summary>
		///     Delete the whole index if it exists and Elastic allows delete index. Property AllowDeleteForIndex must also be set to true. 
		/// </summary>
		/// <typeparam name="T"> Type used to get the index to delete. </typeparam>
		/// <returns> Result details in a task </returns>
		public async Task<ResultDetails<bool>> DeleteIndexAsync<T>()
		{
			return await _elasticContextAddDeleteUpdate.DeleteIndexAsync<T>(AllowDeleteForIndex);
		}

		/// <summary>
		///     Delete the whole index if it exists and Elastic allows delete index. Property AllowDeleteForIndex must also be set to true. 
		/// </summary>
		/// <typeparam name="T"> Type used to get the index to delete. </typeparam>
		/// <returns> Result details in a , true if ok </returns>
		public bool DeleteIndex<T>()
		{
			return _elasticContextAddDeleteUpdate.DeleteIndexAsync<T>(AllowDeleteForIndex).Result.PayloadResult;
		}

		/// <summary>
		///     Delete the whole index if it exists and Elastic allows delete index. Property AllowDeleteForIndex must also be set to true. 
		/// </summary>
		/// <returns> Result details in a , true if ok </returns>
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

		/// <summary>
		///     Dispose used to clean the HttpClient 
		/// </summary>
		public void Dispose()
		{
			if (_client != null)
			{
				_client.Dispose();
			}
		}
	}
}