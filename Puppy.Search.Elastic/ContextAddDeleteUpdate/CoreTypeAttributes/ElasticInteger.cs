using System;

namespace Puppy.Search.Elastic.ContextAddDeleteUpdate.CoreTypeAttributes
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