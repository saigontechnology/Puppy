using System;

namespace Puppy.Elastic.ContextAddDeleteUpdate.CoreTypeAttributes
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