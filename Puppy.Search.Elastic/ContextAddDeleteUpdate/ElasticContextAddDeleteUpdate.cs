using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Puppy.Search.Elastic.Model;
using Puppy.Search.Elastic.Tracing;
using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.ContextAddDeleteUpdate
{
    public class ElasticContextAddDeleteUpdate
    {
        private const string BatchOperationPath = "/_bulk";
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly HttpClient _client;
        private readonly string _connectionString;
        private readonly ElasticSerializerConfiguration _elasticSerializerConfiguration;
        private readonly Uri _elasticUrlBatch;
        private readonly ITraceProvider _traceProvider;
        private bool _saveChangesAndInitMappings;

        public ElasticContextAddDeleteUpdate(ITraceProvider traceProvider,
            CancellationTokenSource cancellationTokenSource,
            ElasticSerializerConfiguration elasticSerializerConfiguration, HttpClient client, string connectionString)
        {
            _traceProvider = traceProvider;
            _cancellationTokenSource = cancellationTokenSource;
            _elasticSerializerConfiguration = elasticSerializerConfiguration;
            _client = client;
            _connectionString = connectionString;
            _elasticUrlBatch = new Uri(new Uri(connectionString), BatchOperationPath);
        }

        public ResultDetails<string> SaveChanges(List<EntityContextInfo> entityPendingChanges,
            bool saveChangesAndInitMappingsForChildDocuments)
        {
            _saveChangesAndInitMappings = saveChangesAndInitMappingsForChildDocuments;
            var syncExecutor = new SyncExecute(_traceProvider);
            return syncExecutor.ExecuteResultDetails(() => SaveChangesAsync(entityPendingChanges));
        }

        public async Task<ResultDetails<string>> SaveChangesAsync(List<EntityContextInfo> entityPendingChanges)
        {
            _traceProvider.Trace(TraceEventType.Verbose, "{0}: Save changes to Elastic started",
                "ElasticContextAddDeleteUpdate");
            var resultDetails = new ResultDetails<string> { Status = HttpStatusCode.InternalServerError };

            if (entityPendingChanges.Count == 0)
            {
                resultDetails = new ResultDetails<string> { Status = HttpStatusCode.OK, Description = "Nothing to save" };
                return resultDetails;
            }

            try
            {
                string serializedEntities;
                using (var serializer = new ElasticSerializer(_traceProvider, _elasticSerializerConfiguration,
                    _saveChangesAndInitMappings))
                {
                    var result = serializer.Serialize(entityPendingChanges);
                    if (_saveChangesAndInitMappings)
                        await result.IndexMappings.Execute(_client, _connectionString, _traceProvider,
                            _cancellationTokenSource);
                    _saveChangesAndInitMappings = false;
                    serializedEntities = result.Content;
                }
                var content = new StringContent(serializedEntities);
                _traceProvider.Trace(TraceEventType.Verbose, "{1}: sending bulk request: {0}", serializedEntities,
                    "ElasticContextAddDeleteUpdate");
                _traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP POST uri: {0}",
                    _elasticUrlBatch.AbsoluteUri, "ElasticContextAddDeleteUpdate");
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                resultDetails.RequestUrl = _elasticUrlBatch.OriginalString;
                var response = await _client.PostAsync(_elasticUrlBatch, content, _cancellationTokenSource.Token)
                    .ConfigureAwait(true);

                resultDetails.Status = response.StatusCode;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _traceProvider.Trace(TraceEventType.Warning, "{2}: SaveChangesAsync response status code: {0}, {1}",
                        response.StatusCode, response.ReasonPhrase, "ElasticContextAddDeleteUpdate");
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        resultDetails.Description = errorInfo;
                        return resultDetails;
                    }

                    return resultDetails;
                }

                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                var responseObject = JObject.Parse(responseString);
                _traceProvider.Trace(TraceEventType.Verbose, "{1}: response: {0}", responseString,
                    "ElasticContextAddDeleteUpdate");
                var errors = string.Empty;
                var items = responseObject["items"];
                if (items != null)
                    foreach (var item in items)
                        if (item["delete"] != null && item["delete"]["status"] != null)
                            if (item["delete"]["status"].ToString() == "404")
                            {
                                resultDetails.Status = HttpStatusCode.NotFound;
                                errors = errors + string.Format("Delete failed for item: {0}, {1}, {2}  :",
                                             item["delete"]["_index"],
                                             item["delete"]["_type"], item["delete"]["_id"]);
                            }

                resultDetails.Description = responseString;
                resultDetails.PayloadResult = serializedEntities;

                if (!string.IsNullOrEmpty(errors))
                {
                    _traceProvider.Trace(TraceEventType.Warning, errors);
                    throw new ElasticException(errors);
                }

                return resultDetails;
            }
            catch (OperationCanceledException oex)
            {
                _traceProvider.Trace(TraceEventType.Warning, oex, "{1}: Get Request OperationCanceledException: {0}",
                    oex.Message, "ElasticContextAddDeleteUpdate");
                resultDetails.Description = "OperationCanceledException";
                return resultDetails;
            }
            finally
            {
                entityPendingChanges.Clear();
            }
        }

        public async Task<ResultDetails<bool>> DeleteIndexAsync<T>(bool allowDeleteForIndex)
        {
            var elasticSearchMapping =
                _elasticSerializerConfiguration.ElasticMappingResolver.GetElasticSearchMapping(typeof(T));
            var elasticUrlForIndexDelete = string.Format("{0}/{1}", _connectionString,
                elasticSearchMapping.GetIndexForType(typeof(T)));
            var uri = new Uri(elasticUrlForIndexDelete);
            return await DeleteInternalAsync(allowDeleteForIndex, uri);
        }

        public async Task<ResultDetails<bool>> DeleteIndexAsync(bool allowDeleteForIndex, string index)
        {
            var elasticUrlForIndexDelete = string.Format("{0}/{1}", _connectionString, index);
            var uri = new Uri(elasticUrlForIndexDelete);
            return await DeleteInternalAsync(allowDeleteForIndex, uri);
        }

        public async Task<ResultDetails<bool>> DeleteInternalAsync(bool allowDeleteForIndex, Uri uri)
        {
            if (!allowDeleteForIndex)
            {
                _traceProvider.Trace(TraceEventType.Error,
                    "{0}: Delete Index, index type is not activated for this context. If you want to activate it, set the AllowDeleteForIndex property of the context",
                    "ElasticContextAddDeleteUpdate");
                throw new ElasticException(
                    "ElasticContext: Index, index type is not activated for this context. If you want to activate it, set the AllowDeleteForIndex property of the context");
            }
            _traceProvider.Trace(TraceEventType.Verbose, "{1}: Request to delete complete index for Type: {0}", uri,
                "ElasticContextAddDeleteUpdate");

            var resultDetails = new ResultDetails<bool> { Status = HttpStatusCode.InternalServerError };
            try
            {
                _traceProvider.Trace(TraceEventType.Warning, "{1}: Request HTTP Delete uri: {0}", uri.AbsoluteUri,
                    "ElasticContextAddDeleteUpdate");
                var response = await _client.DeleteAsync(uri, _cancellationTokenSource.Token).ConfigureAwait(false);

                resultDetails.Status = response.StatusCode;
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _traceProvider.Trace(TraceEventType.Warning,
                        "{2}: Delete Index, index type response status code: {0}, {1}", response.StatusCode,
                        response.ReasonPhrase, "ElasticContextAddDeleteUpdate");
                    if (response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        resultDetails.Description = errorInfo;
                        return resultDetails;
                    }
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    resultDetails.PayloadResult = false;
                }
                else
                {
                    var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    _traceProvider.Trace(TraceEventType.Verbose, "{1}: Delete Index Request response: {0}",
                        responseString, "ElasticContextAddDeleteUpdate");
                    resultDetails.Description = responseString;
                    resultDetails.PayloadResult = true;
                }

                return resultDetails;
            }
            catch (OperationCanceledException oex)
            {
                _traceProvider.Trace(TraceEventType.Warning, oex, "{1}: Get Request OperationCanceledException: {0}",
                    oex.Message, "ElasticContextAddDeleteUpdate");
                return resultDetails;
            }
        }
    }
}