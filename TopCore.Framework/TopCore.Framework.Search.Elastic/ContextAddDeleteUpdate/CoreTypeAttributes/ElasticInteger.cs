using System;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.CoreTypeAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ElasticInteger : ElasticNumber
    {
        public override string JsonString()
        {
            return JsonStringInternal("integer");
        }
    }
}