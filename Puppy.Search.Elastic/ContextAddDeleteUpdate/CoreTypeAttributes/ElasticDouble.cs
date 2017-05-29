using System;

namespace Puppy.Search.Elastic.ContextAddDeleteUpdate.CoreTypeAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ElasticDouble : ElasticNumber
    {
        public override string JsonString()
        {
            return JsonStringInternal("double");
        }
    }
}