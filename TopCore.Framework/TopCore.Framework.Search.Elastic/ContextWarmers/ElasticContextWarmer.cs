using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Tracing;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextWarmers
{
    internal class ElasticContextWarmer
    {
        private readonly ITraceProvider _traceProvider;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ElasticSerializerConfiguration _elasticSerializerConfiguration;
        private readonly HttpClient _client;
        private readonly string _connectionString;

        public ElasticContextWarmer(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource, ElasticSerializerConfiguration elasticSerializerConfiguration, HttpClient client, string connectionString)
        {
            _traceProvider = traceProvider;
            _cancellationTokenSource = cancellationTokenSource;
            _elasticSerializerConfiguration = elasticSerializerConfiguration;
            _client = client;
            _connectionString = connectionString;
        }

        public bool SendWarmerCommand(Warmer warmer, string index, string type)
        {
            var syncExecutor = new SyncExecute(_traceProvider);
            return syncExecutor.Execute(() => SendWarmerCommandAsync(warmer, index, type));
        }

        public async Task<ResultDetails<bool>> SendWarmerCommandAsync(Warmer warmer, string index, string type)
        {
            _traceProvider.Trace(TraceEventType.Verbose, string.Format("ElasticContextWarmer: Creating Warmer {0}", warmer.Name));

            var resultDetails = new ResultDetails<bool> { Status = HttpStatusCode.InternalServerError };
            var elasticUrl = CreateWarmerUriParameter(index, type, warmer.Name);
            var uri = new Uri(elasticUrl);
            _traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP PUT uri: {0}", uri.AbsoluteUri, "ElasticContextWarmer");

            var content = new StringContent(warmer.ToString());
            var response = await _client.PutAsync(uri, content, _cancellationTokenSource.Token).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                resultDetails.PayloadResult = true;
                return resultDetails;
            }

            _traceProvider.Trace(TraceEventType.Error, string.Format("ElasticContextWarmer: Cound Not Execute Warmer Create  {0}", warmer.Name));
            throw new ElasticException(string.Format("ElasticContextWarmer: Cound Not Execute Warmer Create  {0}", warmer.Name));
        }

        private string CreateWarmerUriParameter(string index, string type, string warmerName)
        {
            if (string.IsNullOrEmpty(index))
            {
                return string.Format("{0}/_warmer/{1}", _connectionString, warmerName);
            }

            if (string.IsNullOrEmpty(type))
            {
                return string.Format("{0}/{1}/_warmer/{2}", _connectionString, index, warmerName);
            }

            return string.Format("{0}/{1}/{2}/_warmer/{3}", _connectionString, index, type, warmerName);
        }

        public bool SendWarmerDeleteCommand(string warmerName, string index)
        {
            var syncExecutor = new SyncExecute(_traceProvider);
            return syncExecutor.Execute(() => SendWarmerDeleteCommandAsync(warmerName, index));
        }

        public async Task<ResultDetails<bool>> SendWarmerDeleteCommandAsync(string warmerName, string index)
        {
            _traceProvider.Trace(TraceEventType.Verbose, string.Format("ElasticContextWarmer: Deleting Warmer {0}", warmerName));

            var resultDetails = new ResultDetails<bool> { Status = HttpStatusCode.InternalServerError };
            var elasticUrl = string.Format("{0}/{1}/_warmer/{2}", _connectionString, index, warmerName);
            var uri = new Uri(elasticUrl);
            _traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP DELETE uri: {0}", uri.AbsoluteUri, "ElasticContextWarmer");

            var response = await _client.DeleteAsync(uri, _cancellationTokenSource.Token).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                resultDetails.PayloadResult = true;
                return resultDetails;
            }

            _traceProvider.Trace(TraceEventType.Error, string.Format("ElasticContextWarmer: Cound Not Execute Warmer Delete {0}", warmerName));
            throw new ElasticException(string.Format("ElasticContextWarmer: Cound Not Execute Warmer Delete  {0}", warmerName));
        }
    }
}