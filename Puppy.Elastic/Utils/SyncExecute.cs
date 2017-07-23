using Puppy.Elastic.Model;
using Puppy.Elastic.Tracing;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Puppy.Elastic.Utils
{
    public class SyncExecute
    {
        private readonly ITraceProvider _traceProvider;

        public SyncExecute(ITraceProvider traceProvider)
        {
            _traceProvider = traceProvider;
        }

        public T Execute<T>(Func<Task<ResultDetails<T>>> method)
        {
            try
            {
                var task = Task.Run(() => method.Invoke());
                task.Wait();
                if (task.Result.Status == HttpStatusCode.NotFound)
                    _traceProvider.Trace(TraceEventType.Information, "SyncExecute: Execute HttpStatusCode.NotFound");

                return task.Result.PayloadResult;
            }
            catch (AggregateException ae)
            {
                ae.Handle(x =>
                {
                    _traceProvider.Trace(TraceEventType.Warning, x, "SyncExecute: Execute {0}", typeof(T));
                    if (x is ElasticException || x is HttpRequestException)
                        throw x;

                    throw new ElasticException(x.Message);
                });
            }

            _traceProvider.Trace(TraceEventType.Error, "SyncExecute: Unknown error for Exists  Type {0}", typeof(T));
            throw new ElasticException(string.Format("SyncExecute: Unknown error for Exists Type {0}", typeof(T)));
        }

        public ResultDetails<T> ExecuteResultDetails<T>(Func<Task<ResultDetails<T>>> method)
        {
            try
            {
                var task = Task.Run(() => method.Invoke());
                task.Wait();
                if (task.Result.Status == HttpStatusCode.NotFound)
                    _traceProvider.Trace(TraceEventType.Information,
                        "SyncExecute: ExecuteResultDetails HttpStatusCode.NotFound");

                return task.Result;
            }
            catch (AggregateException ae)
            {
                ae.Handle(x =>
                {
                    _traceProvider.Trace(TraceEventType.Warning, x, "SyncExecute: ExecuteResultDetails {0}", typeof(T));
                    if (x is ElasticException || x is HttpRequestException)
                        throw x;

                    throw new ElasticException(x.Message);
                });
            }

            _traceProvider.Trace(TraceEventType.Error, "SyncExecute: Unknown error for Exists  Type {0}", typeof(T));
            throw new ElasticException(string.Format("SyncExecute: Unknown error for Exists Type {0}", typeof(T)));
        }
    }
}