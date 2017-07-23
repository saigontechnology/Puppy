using System;

namespace Puppy.Elastic.ContextAddDeleteUpdate.CoreTypeAttributes
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