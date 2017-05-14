using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel;
using TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.MappingModel;
using TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel;
using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Tracing;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate
{
    public class ElasticContextIndexMapping
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly HttpClient _client;
        private readonly string _connectionString;
        private readonly ElasticSerializerConfiguration _elasticSerializerConfiguration;
        private readonly ITraceProvider _traceProvider;

        public ElasticContextIndexMapping(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource,
            ElasticSerializerConfiguration elasticSerializerConfiguration, HttpClient client, string connectionString)
        {
            _traceProvider = traceProvider;
            _cancellationTokenSource = cancellationTokenSource;
            _elasticSerializerConfiguration = elasticSerializerConfiguration;
            _client = client;
            _connectionString = connectionString;
        }

        public ResultDetails<string> CreateIndexWithMapping<T>(IndexDefinition indexDefinition)
        {
            var syncExecutor = new SyncExecute(_traceProvider);
            return syncExecutor.ExecuteResultDetails(() => CreateIndexWithMappingAsync<T>(indexDefinition));
        }

        public async Task<ResultDetails<string>> CreateIndexWithMappingAsync<T>(IndexDefinition indexDefinition)
        {
            if (indexDefinition == null)
                indexDefinition = new IndexDefinition();
            _traceProvider.Trace(TraceEventType.Verbose, "{0}: CreateIndexWithMappingAsync Elastic started",
                "ElasticContextIndexMapping");
            var resultDetails = new ResultDetails<string> {Status = HttpStatusCode.InternalServerError};

            try
            {
                var item = Activator.CreateInstance<T>();
                var entityContextInfo = new EntityContextInfo
                {
                    RoutingDefinition = indexDefinition.Mapping.RoutingDefinition,
                    Document = item,
                    EntityType = typeof(T),
                    Id = "0"
                };

                var index =
                    _elasticSerializerConfiguration.ElasticMappingResolver.GetElasticSearchMapping(
                            entityContextInfo.EntityType)
                        .GetIndexForType(entityContextInfo.EntityType);
                MappingUtils.GuardAgainstBadIndexName(index);

                var indexMappings = new IndexMappings(_traceProvider, _elasticSerializerConfiguration);
                indexMappings.CreateIndexSettingsAndMappingsForDocument(
                    index,
                    indexDefinition.IndexSettings,
                    indexDefinition.IndexAliases,
                    indexDefinition.IndexWarmers, entityContextInfo, indexDefinition.Mapping);
                indexDefinition.Mapping.Index = index;
                //indexMappings.CreatePropertyMappingForTopDocument(entityContextInfo, indexDefinition.Mapping);
                await indexMappings.Execute(_client, _connectionString, _traceProvider, _cancellationTokenSource);

                return resultDetails;
            }
            catch (OperationCanceledException oex)
            {
                _traceProvider.Trace(TraceEventType.Warning, oex,
                    "{1}: CreateIndexWithMappingAsync Request OperationCanceledException: {0}", oex.Message,
                    "ElasticContextIndexMapping");
                resultDetails.Description = "OperationCanceledException";
                return resultDetails;
            }
        }

        public ResultDetails<string> CreateIndex(string index, IndexSettings indexSettings, IndexAliases indexAliases,
            IndexWarmers indexWarmers)
        {
            var syncExecutor = new SyncExecute(_traceProvider);
            return syncExecutor.ExecuteResultDetails(
                () => CreateIndexAsync(index, indexSettings, indexAliases, indexWarmers));
        }

        public async Task<ResultDetails<string>> CreateIndexAsync(string index, IndexSettings indexSettings,
            IndexAliases indexAliases, IndexWarmers indexWarmers)
        {
            if (string.IsNullOrEmpty(index))
                throw new ElasticException("CreateIndexAsync: index is required");
            if (indexSettings == null)
                indexSettings = new IndexSettings {NumberOfShards = 5, NumberOfReplicas = 1};
            if (indexAliases == null)
                indexAliases = new IndexAliases();

            if (indexWarmers == null)
                indexWarmers = new IndexWarmers();

            _traceProvider.Trace(TraceEventType.Verbose, "{0}: CreateIndexAsync Elastic started",
                "ElasticContextIndexMapping");
            var resultDetails = new ResultDetails<string> {Status = HttpStatusCode.InternalServerError};

            try
            {
                MappingUtils.GuardAgainstBadIndexName(index);

                var indexMappings = new IndexMappings(_traceProvider, _elasticSerializerConfiguration);
                indexMappings.CreateIndexSettingsForDocument(index, indexSettings, indexAliases, indexWarmers);
                await indexMappings.Execute(_client, _connectionString, _traceProvider, _cancellationTokenSource);

                return resultDetails;
            }
            catch (OperationCanceledException oex)
            {
                _traceProvider.Trace(TraceEventType.Warning, oex,
                    "{1}: CreateIndexAsync Request OperationCanceledException: {0}", oex.Message,
                    "ElasticContextIndexMapping");
                resultDetails.Description = "OperationCanceledException";
                return resultDetails;
            }
        }

        public ResultDetails<string> CreateTypeMappingForIndex<T>(MappingDefinition mappingDefinition)
        {
            var syncExecutor = new SyncExecute(_traceProvider);
            return syncExecutor.ExecuteResultDetails(() => CreateTypeMappingForIndexAsync<T>(mappingDefinition));
        }

        public async Task<ResultDetails<string>> CreateTypeMappingForIndexAsync<T>(MappingDefinition mappingDefinition)
        {
            if (mappingDefinition == null)
                throw new ElasticException(
                    "CreateTypeMappingForIndexAsync: A mapping definition with the parent index is required");
            _traceProvider.Trace(TraceEventType.Verbose, "{0}: CreateTypeMappingForIndex Elastic started",
                "ElasticContextIndexMapping");
            var resultDetails = new ResultDetails<string> {Status = HttpStatusCode.InternalServerError};

            try
            {
                var indexMappings = new IndexMappings(_traceProvider, _elasticSerializerConfiguration);

                var item = Activator.CreateInstance<T>();

                var entityContextInfo = new EntityContextInfo
                {
                    RoutingDefinition = mappingDefinition.RoutingDefinition,
                    Document = item,
                    EntityType = typeof(T),
                    Id = "0"
                };

                if (string.IsNullOrEmpty(mappingDefinition.Index))
                {
                    var index = _elasticSerializerConfiguration.ElasticMappingResolver
                        .GetElasticSearchMapping(entityContextInfo.EntityType)
                        .GetIndexForType(entityContextInfo.EntityType);
                    MappingUtils.GuardAgainstBadIndexName(index);
                    mappingDefinition.Index = index;
                }

                indexMappings.CreatePropertyMappingForTopDocument(entityContextInfo, mappingDefinition);
                await indexMappings.Execute(_client, _connectionString, _traceProvider, _cancellationTokenSource);

                return resultDetails;
            }
            catch (OperationCanceledException oex)
            {
                _traceProvider.Trace(TraceEventType.Warning, oex,
                    "{1}: CreateTypeMappingForIndexAsync Request OperationCanceledException: {0}", oex.Message,
                    "ElasticContextIndexMapping");
                resultDetails.Description = "OperationCanceledException";
                return resultDetails;
            }
        }

        public ResultDetails<string> UpdateIndexSettings(IndexUpdateSettings indexUpdateSettings, string index = null)
        {
            var syncExecutor = new SyncExecute(_traceProvider);
            return syncExecutor.ExecuteResultDetails(() => UpdateIndexSettingsAsync(indexUpdateSettings, index));
        }

        public async Task<ResultDetails<string>> UpdateIndexSettingsAsync(IndexUpdateSettings indexUpdateSettings,
            string index = null)
        {
            _traceProvider.Trace(TraceEventType.Verbose, "{0}: UpdateIndexSettingsAsync Elastic started",
                "ElasticContextIndexMapping");
            var resultDetails = new ResultDetails<string> {Status = HttpStatusCode.InternalServerError};

            try
            {
                var indexMappings = new IndexMappings(_traceProvider, _elasticSerializerConfiguration);
                indexMappings.UpdateSettings(index, indexUpdateSettings);
                await indexMappings.Execute(_client, _connectionString, _traceProvider, _cancellationTokenSource);

                resultDetails.PayloadResult = "completed";
                return resultDetails;
            }
            catch (OperationCanceledException oex)
            {
                _traceProvider.Trace(TraceEventType.Warning, oex,
                    "{1}: UpdateIndexSettingsAsync OperationCanceledException: {0}", oex.Message,
                    "ElasticContextIndexMapping");
                resultDetails.Description = "OperationCanceledException";
                return resultDetails;
            }
        }

        public ResultDetails<bool> CloseIndex(string index)
        {
            var syncExecutor = new SyncExecute(_traceProvider);
            return syncExecutor.ExecuteResultDetails(() => CloseIndexAsync(index));
        }

        public async Task<ResultDetails<bool>> CloseIndexAsync(string index)
        {
            if (string.IsNullOrEmpty(index))
                throw new ElasticException("CloseIndexAsync: index is required");

            var elasticUrlForPostRequest = string.Format("{0}/{1}/_close", _connectionString, index);
            var uri = new Uri(elasticUrlForPostRequest);
            return await CloseOpenIndexAsync(uri);
        }

        public ResultDetails<bool> OpenIndex(string index)
        {
            var syncExecutor = new SyncExecute(_traceProvider);
            return syncExecutor.ExecuteResultDetails(() => OpenIndexAsync(index));
        }

        public async Task<ResultDetails<bool>> OpenIndexAsync(string index)
        {
            if (string.IsNullOrEmpty(index))
                throw new ElasticException("OpenIndexAsync: index is required");

            var elasticUrlForPostRequest = string.Format("{0}/{1}/_open", _connectionString, index);
            var uri = new Uri(elasticUrlForPostRequest);
            return await CloseOpenIndexAsync(uri);
        }

        public ResultDetails<OptimizeResult> IndexOptimize(string index, OptimizeParameters optimizeParameters)
        {
            var syncExecutor = new SyncExecute(_traceProvider);
            return syncExecutor.ExecuteResultDetails(() => IndexOptimizeAsync(index, optimizeParameters));
        }

        public async Task<ResultDetails<OptimizeResult>> IndexOptimizeAsync(string index,
            OptimizeParameters optimizeParameters)
        {
            if (optimizeParameters == null)
                optimizeParameters = new OptimizeParameters();

            var elasticUrlForPostRequest = string.Format("{0}/{1}/_optimize{2}", _connectionString, index,
                optimizeParameters.GetOptimizeParameters());
            var uri = new Uri(elasticUrlForPostRequest);
            _traceProvider.Trace(TraceEventType.Verbose, "IndexOptimizeAsync Request POST with url: {0}",
                uri.ToString());

            var resultDetails = new ResultDetails<OptimizeResult> {Status = HttpStatusCode.InternalServerError};
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, uri);
                var response = await _client.SendAsync(request, _cancellationTokenSource.Token).ConfigureAwait(false);

                resultDetails.RequestUrl = uri.OriginalString;

                resultDetails.Status = response.StatusCode;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        resultDetails.Description = errorInfo;
                        throw new ElasticException(
                            "IndexOptimizeAsync: HttpStatusCode.BadRequest: RoutingMissingException, adding the parent Id if this is a child item...");
                    }
                    if (response.StatusCode == HttpStatusCode.NotFound)
                        throw new ElasticException("IndexOptimizeAsync: HttpStatusCode.NotFound index does not exist");

                    _traceProvider.Trace(TraceEventType.Information,
                        "IndexOptimizeAsync:  response status code: {0}, {1}", response.StatusCode,
                        response.ReasonPhrase);
                }
                else
                {
                    var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                    var responseObject = JObject.Parse(responseString);
                    resultDetails.PayloadResult = responseObject.ToObject<OptimizeResult>();
                }

                return resultDetails;
            }
            catch (OperationCanceledException oex)
            {
                _traceProvider.Trace(TraceEventType.Verbose, oex,
                    "IndexOptimizeAsync:  Get Request OperationCanceledException: {0}", oex.Message);
                return resultDetails;
            }
        }

        private async Task<ResultDetails<bool>> CloseOpenIndexAsync(Uri uri)
        {
            _traceProvider.Trace(TraceEventType.Verbose, "CloseOpenIndexAsync Request POST with url: {0}",
                uri.ToString());
            var resultDetails = new ResultDetails<bool> {Status = HttpStatusCode.InternalServerError};
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, uri);
                var response = await _client.SendAsync(request, _cancellationTokenSource.Token).ConfigureAwait(false);

                resultDetails.RequestUrl = uri.OriginalString;

                resultDetails.Status = response.StatusCode;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    resultDetails.PayloadResult = false;
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        resultDetails.Description = errorInfo;
                        throw new ElasticException(
                            "CloseOpenIndexAsync: HttpStatusCode.BadRequest: RoutingMissingException, adding the parent Id if this is a child item...");
                    }
                    if (response.StatusCode == HttpStatusCode.NotFound)
                        throw new ElasticException("CloseOpenIndexAsync: HttpStatusCode.NotFound index does not exist");

                    _traceProvider.Trace(TraceEventType.Information,
                        "CloseOpenIndexAsync:  response status code: {0}, {1}", response.StatusCode,
                        response.ReasonPhrase);
                }
                else
                {
                    resultDetails.PayloadResult = true;
                }

                return resultDetails;
            }
            catch (OperationCanceledException oex)
            {
                _traceProvider.Trace(TraceEventType.Verbose, oex,
                    "CloseOpenIndexAsync:  POST Request OperationCanceledException: {0}", oex.Message);
                return resultDetails;
            }
        }
    }
}