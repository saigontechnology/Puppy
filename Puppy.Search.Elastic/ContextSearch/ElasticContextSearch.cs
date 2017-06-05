using Puppy.Search.Elastic.ContextSearch.SearchModel;
using Puppy.Search.Elastic.Model;
using Puppy.Search.Elastic.Tracing;
using Puppy.Search.Elastic.Utils;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Puppy.Search.Elastic.ContextSearch
{
    public class ElasticContextSearch
    {
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly HttpClient _client;
        private readonly string _connectionString;
        private readonly ElasticSerializerConfiguration _elasticSerializerConfiguration;
        private readonly ITraceProvider _traceProvider;

        public ElasticContextSearch(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource,
            ElasticSerializerConfiguration elasticSerializerConfiguration, HttpClient client, string connectionString)
        {
            _traceProvider = traceProvider;
            _cancellationTokenSource = cancellationTokenSource;
            _elasticSerializerConfiguration = elasticSerializerConfiguration;
            _client = client;
            _connectionString = connectionString;
        }

        public T SearchById<T>(object entityId, SearchUrlParameters searchUrlParameters)
        {
            var syncExecutor = new SyncExecute(_traceProvider);
            var result = syncExecutor.ExecuteResultDetails(() => SearchByIdAsync<T>(entityId, searchUrlParameters));

            if (result.Status == HttpStatusCode.NotFound)
            {
                _traceProvider.Trace(TraceEventType.Warning, "ElasticContextSearch: HttpStatusCode.NotFound");
                throw new ElasticException("ElasticContextSearch: HttpStatusCode.NotFound");
            }
            if (result.Status == HttpStatusCode.BadRequest)
            {
                _traceProvider.Trace(TraceEventType.Warning, "ElasticContextSearch: HttpStatusCode.BadRequest");
                throw new ElasticException("ElasticContextSearch: HttpStatusCode.BadRequest" + result.Description);
            }

            return result.PayloadResult;
        }

        public async Task<ResultDetails<T>> SearchByIdAsync<T>(object entityId, SearchUrlParameters searchUrlParameters)
        {
            var elasticSearchMapping =
                _elasticSerializerConfiguration.ElasticMappingResolver.GetElasticSearchMapping(typeof(T));
            var index = elasticSearchMapping.GetIndexForType(typeof(T));
            var type = elasticSearchMapping.GetDocumentType(typeof(T));
            _traceProvider.Trace(TraceEventType.Verbose,
                string.Format("ElasticContextSearch: Searching for document id: {0}, index {1}, type {2}", entityId,
                    index, type));

            var resultDetails = new ResultDetails<T> { Status = HttpStatusCode.InternalServerError };

            var search = new SearchRequest(_traceProvider, _cancellationTokenSource, _elasticSerializerConfiguration,
                _client, _connectionString);

            var result = await search.PostSearchAsync<T>(BuildSearchById(entityId), null, null, searchUrlParameters);
            resultDetails.RequestBody = result.RequestBody;
            resultDetails.RequestUrl = result.RequestUrl;

            if (result.Status == HttpStatusCode.OK && result.PayloadResult.Hits.Total > 0)
            {
                resultDetails.PayloadResult = result.PayloadResult.Hits.HitsResult.First().Source;
                return resultDetails;
            }

            if (result.Status == HttpStatusCode.OK && result.PayloadResult.Hits.Total == 0)
            {
                resultDetails.Status = HttpStatusCode.NotFound;
                resultDetails.Description = string.Format("No document found id: {0}, index {1}, type {2}", entityId,
                    index, type);
                _traceProvider.Trace(TraceEventType.Information,
                    string.Format("ElasticContextSearch: No document found id: {0},, index {1}, type {2}", entityId,
                        index, type));
                return resultDetails;
            }

            resultDetails.Status = result.Status;
            resultDetails.Description = result.Description;
            _traceProvider.Trace(TraceEventType.Error,
                string.Format("ElasticContextSearch: No document found id: {0},  index {1}, type {2}", entityId, index,
                    type));
            return resultDetails;
        }

        //{
        // "query" : {
        //	 "filtered": {
        //	   "query": {
        //		"term": {"id": "47"}
        //	  }
        //	  }
        //  }
        //}
        private string BuildSearchById(object childId)
        {
            var buildJson = new StringBuilder();
            buildJson.AppendLine("{");
            buildJson.AppendLine("\"query\": {");
            buildJson.AppendLine("\"filtered\": {");
            buildJson.AppendLine("\"query\": {");
            buildJson.AppendLine("\"term\": {\"_id\": " + childId + "}");
            buildJson.AppendLine("}");
            buildJson.AppendLine("}");
            buildJson.AppendLine("}");
            buildJson.AppendLine("}");

            return buildJson.ToString();
        }
    }
}