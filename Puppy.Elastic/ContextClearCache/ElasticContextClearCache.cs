using Puppy.Elastic.Model;
using Puppy.Elastic.Tracing;
using Puppy.Elastic.Utils;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Puppy.Elastic.ContextClearCache
{
    public class ElasticContextClearCache
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly HttpClient _client;
        private readonly string _connectionString;
        private readonly ElasticSerializerConfiguration _elasticSerializerConfiguration;
        private readonly ITraceProvider _traceProvider;

        public ElasticContextClearCache(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource,
            ElasticSerializerConfiguration elasticSerializerConfiguration, HttpClient client, string connectionString)
        {
            _traceProvider = traceProvider;
            _cancellationTokenSource = cancellationTokenSource;
            _elasticSerializerConfiguration = elasticSerializerConfiguration;
            _client = client;
            _connectionString = connectionString;
        }

        public bool ClearCacheForIndex(string index)
        {
            var syncExecutor = new SyncExecute(_traceProvider);
            return syncExecutor.Execute(() => ClearCacheForIndexAsync(index));
        }

        public bool ClearCacheForIndex<T>()
        {
            var syncExecutor = new SyncExecute(_traceProvider);
            return syncExecutor.Execute(() => ClearCacheForIndexAsync<T>());
        }

        public async Task<ResultDetails<bool>> ClearCacheForIndexAsync<T>()
        {
            var elasticSearchMapping =
                _elasticSerializerConfiguration.ElasticMappingResolver.GetElasticSearchMapping(typeof(T));
            var index = elasticSearchMapping.GetIndexForType(typeof(T));
            _traceProvider.Trace(TraceEventType.Verbose,
                string.Format("ElasticContextClearCache: Clearing Cache for index {0}", index));

            var resultDetails = new ResultDetails<bool> { Status = HttpStatusCode.InternalServerError };
            var elasticUrlForClearCache = string.Format("{0}/{1}/_cache/clear", _connectionString, index);
            var uri = new Uri(elasticUrlForClearCache);
            _traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP POST uri: {0}", uri.AbsoluteUri,
                "ElasticContextClearCache");

            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            var response = await _client.SendAsync(request, _cancellationTokenSource.Token).ConfigureAwait(true);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                resultDetails.PayloadResult = true;
                return resultDetails;
            }

            _traceProvider.Trace(TraceEventType.Error,
                string.Format("ElasticContextClearCache: Could nor clear cache for index {0}",
                    elasticSearchMapping.GetIndexForType(typeof(T))));
            throw new ElasticException(string.Format("ElasticContextClearCache: Could nor clear cache for index {0}",
                elasticSearchMapping.GetIndexForType(typeof(T))));
        }

        public async Task<ResultDetails<bool>> ClearCacheForIndexAsync(string index)
        {
            _traceProvider.Trace(TraceEventType.Verbose,
                string.Format("ElasticContextClearCache: Clearing Cache for index {0}", index));

            var resultDetails = new ResultDetails<bool> { Status = HttpStatusCode.InternalServerError };
            var elasticUrlForClearCache = string.Format("{0}/{1}/_cache/clear", _connectionString, index);
            var uri = new Uri(elasticUrlForClearCache);
            _traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP POST uri: {0}", uri.AbsoluteUri,
                "ElasticContextClearCache");

            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            var response = await _client.SendAsync(request, _cancellationTokenSource.Token).ConfigureAwait(true);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                resultDetails.PayloadResult = true;
                return resultDetails;
            }

            _traceProvider.Trace(TraceEventType.Error,
                string.Format("ElasticContextClearCache: Could nor clear cache for index {0}", index));
            throw new ElasticException(string.Format("ElasticContextClearCache: Could nor clear cache for index {0}",
                index));
        }
    }
}