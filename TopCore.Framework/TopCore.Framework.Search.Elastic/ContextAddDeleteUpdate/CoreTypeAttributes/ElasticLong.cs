using System;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.CoreTypeAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ElasticLong : ElasticNumber
    {
        public override string JsonString()
        {
            return JsonStringInternal("long");
        }
    }
}