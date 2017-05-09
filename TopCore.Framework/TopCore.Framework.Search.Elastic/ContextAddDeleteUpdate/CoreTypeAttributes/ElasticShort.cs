using System;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.CoreTypeAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ElasticShort : ElasticNumber
    {
        public override string JsonString()
        {
            return JsonStringInternal("short");
        }
    }
}