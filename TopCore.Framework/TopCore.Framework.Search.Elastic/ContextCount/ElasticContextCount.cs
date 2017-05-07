using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Tracing;

namespace TopCore.Framework.Search.Elastic.ContextCount
{
	public class ElasticContextCount
	{
		private readonly ITraceProvider _traceProvider;
		private readonly CancellationTokenSource _cancellationTokenSource;
		private readonly ElasticSerializerConfiguration _elasticSerializerConfiguration;
		private readonly HttpClient _client;
		private readonly string _connectionString;

		public ElasticContextCount(ITraceProvider traceProvider, CancellationTokenSource cancellationTokenSource, ElasticSerializerConfiguration elasticSerializerConfiguration, HttpClient client, string connectionString)
		{
			_traceProvider = traceProvider;
			_cancellationTokenSource = cancellationTokenSource;
			_elasticSerializerConfiguration = elasticSerializerConfiguration;
			_client = client;
			_connectionString = connectionString;
		}

		public async Task<ResultDetailsCount<long>> PostCountAsync<T>(string jsonContent)
		{
			_traceProvider.Trace(TraceEventType.Verbose, "{2}: Request for ElasticContextCount: {0}, content: {1}", typeof(T), jsonContent, "ElasticContextCount");
			var resultDetails = new ResultDetailsCount<long>
			{
				Status = HttpStatusCode.InternalServerError,
				RequestBody = jsonContent
			};

			try
			{
				var elasticSearchMapping = _elasticSerializerConfiguration.ElasticMappingResolver.GetElasticSearchMapping(typeof(T));
				var elasticUrlForEntityGet = string.Format("{0}/{1}/{2}/_count", _connectionString, elasticSearchMapping.GetIndexForType(typeof(T)), elasticSearchMapping.GetDocumentType(typeof(T)));

				var content = new StringContent(jsonContent);
				var uri = new Uri(elasticUrlForEntityGet);
				_traceProvider.Trace(TraceEventType.Verbose, "{1}: Request HTTP GET uri: {0}", uri.AbsoluteUri, "ElasticContextCount");

				content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
				resultDetails.RequestUrl = elasticUrlForEntityGet;
				var response = await _client.PostAsync(uri, content, _cancellationTokenSource.Token).ConfigureAwait(true);

				resultDetails.Status = response.StatusCode;
				if (response.StatusCode != HttpStatusCode.OK)
				{
					_traceProvider.Trace(TraceEventType.Warning, "{2}: GetCountAsync response status code: {0}, {1}", response.StatusCode, response.ReasonPhrase, "ElasticContextCount");
					if (response.StatusCode == HttpStatusCode.BadRequest)
					{
						var errorInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
						resultDetails.Description = errorInfo;
						if (errorInfo.Contains("RoutingMissingException"))
						{
							throw new ElasticException("ElasticContextCount: HttpStatusCode.BadRequest: RoutingMissingException, adding the parent Id if this is a child item...");
						}

						return resultDetails;
					}

					throw new ElasticException("ElasticContextCount: Index not found");
				}

				var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(true);
				_traceProvider.Trace(TraceEventType.Verbose, "{1}: Get Request response: {0}", responseString, "ElasticContextCount");
				var responseObject = JObject.Parse(responseString);

				resultDetails.Count = (long)responseObject["count"];
				resultDetails.PayloadResult = resultDetails.Count;

				return resultDetails;
			}
			catch (OperationCanceledException oex)
			{
				_traceProvider.Trace(TraceEventType.Verbose, oex, "{1}: Get Request OperationCanceledException: {0}", oex.Message, "ElasticContextCount");
				return resultDetails;
			}
		}

		public ResultDetailsCount<long> PostCount<T>(string jsonContent)
		{
			return ExecuteCountResultDetails(() => PostCountAsync<T>(jsonContent));
		}

		private ResultDetailsCount<T> ExecuteCountResultDetails<T>(Func<Task<ResultDetailsCount<T>>> method)
		{
			try
			{
				Task<ResultDetailsCount<T>> task = Task.Run(() => method.Invoke());
				task.Wait();
				if (task.Result.Status == HttpStatusCode.NotFound)
				{
					_traceProvider.Trace(TraceEventType.Information, "SyncExecute: ExecuteResultDetails HttpStatusCode.NotFound");
				}

				return task.Result;
			}
			catch (AggregateException ae)
			{
				ae.Handle(x =>
				{
					_traceProvider.Trace(TraceEventType.Warning, x, "SyncExecute: ExecuteResultDetails {0}", typeof(T));
					if (x is ElasticException || x is HttpRequestException)
					{
						throw x;
					}

					throw new ElasticException(x.Message);
				});
			}

			_traceProvider.Trace(TraceEventType.Error, "SyncExecute: Unknown error for Exists  Type {0}", typeof(T));
			throw new ElasticException(string.Format("SyncExecute: Unknown error for Exists Type {0}", typeof(T)));
		}
	}
}