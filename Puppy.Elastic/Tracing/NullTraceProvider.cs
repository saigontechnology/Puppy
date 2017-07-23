using System;
using System.Diagnostics;

namespace Puppy.Elastic.Tracing
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