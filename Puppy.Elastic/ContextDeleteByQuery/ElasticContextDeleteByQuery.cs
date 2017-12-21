using Puppy.Elastic.Model;
using Puppy.Elastic.Tracing;
using Puppy.Elastic.Utils;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Puppy.Elastic.ContextDeleteByQuery
{
    public class ElasticContextDeleteByQuery
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly HttpClient _client;
        private readonly string _connectionString;
        private readonly ElasticSerializerConfiguration _elasticSerializerConfiguration;
        private readonly ITraceProvider _traceProvider;

        public ElasticContextDeleteByQuery(ITraceProvider traceProvider,
            CancellationTokenSource cancellationTokenSource,
            ElasticSerializerConfiguration elasticSerializerConfiguration, HttpClient client, string connectionString)
        {
            _traceProvider = traceProvider;
            _cancellationTokenSource = cancellationTokenSource;
            _elasticSerializerConfiguration = elasticSerializerConfiguration;
            _client = client;
            _connectionString = connectionString;
        }

        public async Task<ResultDetails<bool>> DeleteByQueryAsync<T>(string jsonContent)
        {
            _traceProvider.Trace(TraceEventType.Verbose,
                "{2}: Request for ElasticContextDeleteByQuery: {0}, content: {1}", typeof(T), jsonContent,
                "ElasticContextDeleteByQuery");
            var resultDetails = new ResultDetails<bool>
            {
                Status = HttpStatusCode.InternalServerError,
                RequestBody = jsonContent
            };

            try
            {
                var elasticSearchMapping = _elasticSerializerConfiguration.ElasticMappingResolver
                    .GetElasticSearchMapping(typeof(T));
                var elasticUrlForEntityGet = string.Format("{0}/{1}/{2}/_query", _connectionString,
                    elasticSearchMapping.GetIndexForType(typeof(T)), elasticSearchMapping.GetDocumentType(typeof(T)));

                var content = new StringContent(jsonContent);
                var uri = new Uri(elasticUrlForEntityGet);
                _traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP DELETE uri: {0}", uri.AbsoluteUri,
                    "Search");

                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var request = new HttpRequestMessage(HttpMethod.Delete, uri)
                {
                    Content = content
                };
                var response = await _client.SendAsync(request, _cancellationTokenSource.Token).ConfigureAwait(true);

                resultDetails.RequestUrl = elasticUrlForEntityGet;
                resultDetails.Status = response.StatusCode;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _traceProvider.Trace(TraceEventType.Warning, "{2}: GetSearchAsync response status code: {0}, {1}",
                        response.StatusCode, response.ReasonPhrase, "Search");
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                        resultDetails.Description = errorInfo;
                        if (errorInfo.Contains("RoutingMissingException"))
                            throw new ElasticException(
                                "HttpStatusCode.BadRequest: RoutingMissingException, adding the parent Id if this is a child item...");

                        return resultDetails;
                    }
                }

                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
                _traceProvider.Trace(TraceEventType.Verbose, "{1}: Delete Request response: {0}", responseString,
                    "Search");

                return resultDetails;
            }
            catch (OperationCanceledException oex)
            {
                _traceProvider.Trace(TraceEventType.Verbose, oex, "{1}: Delete Request OperationCanceledException: {0}",
                    oex.Message, "Search");
                return resultDetails;
            }
        }

        public ResultDetails<bool> SendDeleteByQuery<T>(string jsonContent)
        {
            var syncExecutor = new SyncExecute(_traceProvider);
            var result = syncExecutor.ExecuteResultDetails(() => DeleteByQueryAsync<T>(jsonContent));

            if (result.Status == HttpStatusCode.NotFound)
            {
                _traceProvider.Trace(TraceEventType.Warning, "ElasticContextDeleteByQuery: HttpStatusCode.NotFound");
                throw new ElasticException("ElasticContextDeleteByQuery: HttpStatusCode.NotFound");
            }
            if (result.Status == HttpStatusCode.BadRequest)
            {
                _traceProvider.Trace(TraceEventType.Warning, "ElasticContextDeleteByQuery: HttpStatusCode.BadRequest");
                throw new ElasticException(
                    "ElasticContextDeleteByQuery: HttpStatusCode.BadRequest" + result.Description);
            }

            return result;
        }
    }
}