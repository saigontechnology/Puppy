using System;

namespace TopCore.Framework.Search.Elastic
{
    public class ElasticException : Exception
    {
        public ElasticException(string message) : base(message)
        {
        }
    }
}