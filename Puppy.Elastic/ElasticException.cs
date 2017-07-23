using System;

namespace Puppy.Elastic
{
    public class ElasticException : Exception
    {
        public ElasticException(string message) : base(message)
        {
        }
    }
}