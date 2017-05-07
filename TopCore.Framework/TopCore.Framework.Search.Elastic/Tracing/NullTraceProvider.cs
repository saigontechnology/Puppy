using System;
using System.Diagnostics;

namespace TopCore.Framework.Search.Elastic.Tracing
{
	public class NullTraceProvider : ITraceProvider
    {
        public void Trace(TraceEventType level, string message, params object[] args)
        {
        }

        public void Trace(TraceEventType level, Exception ex, string message, params object[] args)
        {
        }
    }
}