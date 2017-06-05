using Puppy.Search.Elastic.Model;
using Puppy.Search.Elastic.Tracing;
using Puppy.Search.Elastic.Utils;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Puppy.Search.Elastic.ContextAlias
{
    internal class ElasticContextAlias
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly HttpClient _client;
        private readonly string _connectionString;
        private readonly ElasticSerializerConfiguration _elasticSerializerConfiguration;
        private readonly ITraceProvider _traceProvider;

        public ElasticContextAlias(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource,
            ElasticSerializerConfiguration elasticSerializerConfiguration, HttpClient client, string connectionString)
        {
            _traceProvider = traceProvider;
            _cancellationTokenSource = cancellationTokenSource;
            _elasticSerializerConfiguration = elasticSerializerConfiguration;
            _client = client;
            _connectionString = connectionString;
        }

        public bool SendAliasCommand(string contentJson)
        {
            var syncExecutor = new SyncExecute(_traceProvider);
            return syncExecutor.Execute(() => SendAliasCommandAsync(contentJson));
        }

        public async Task<ResultDetails<bool>> SendAliasCommandAsync(string contentJson)
        {
            _traceProvider.Trace(TraceEventType.Verbose,
                string.Format("ElasticContextAlias: Creating Alias {0}", contentJson));

            var resultDetails = new ResultDetails<bool> { Status = HttpStatusCode.InternalServerError };
            var elasticUrlForClearCache = string.Format("{0}/_aliases", _connectionString);
            var uri = new Uri(elasticUrlForClearCache);
            _traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP POST uri: {0}", uri.AbsoluteUri,
                "ElasticContextAlias");

            var content = new StringContent(contentJson);
            var response = await _client.PostAsync(uri, content, _cancellationTokenSource.Token).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                resultDetails.PayloadResult = true;
                return resultDetails;
            }

            _traceProvider.Trace(TraceEventType.Error,
                string.Format("ElasticContextAlias: Cound Not Execute Alias {0}", contentJson));
            throw new ElasticException(string.Format("ElasticContextAlias: Cound Not Execute Alias  {0}", contentJson));
        }
    }
}