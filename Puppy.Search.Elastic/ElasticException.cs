using System;

namespace Puppy.Search.Elastic
{
    public class ElasticException : Exception
    {
        public ElasticException(string message) : base(message)
        {
        }
    }
}