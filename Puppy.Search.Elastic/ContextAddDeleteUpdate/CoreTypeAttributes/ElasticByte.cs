using System;

namespace Puppy.Search.Elastic.ContextAddDeleteUpdate.CoreTypeAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ElasticByte : ElasticNumber
    {
        public override string JsonString()
        {
            return JsonStringInternal("byte");
        }
    }
}