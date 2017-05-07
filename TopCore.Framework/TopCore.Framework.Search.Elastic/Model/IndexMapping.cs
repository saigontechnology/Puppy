using System;

namespace TopCore.Framework.Search.Elastic.Model
{
	public class IndexMapping : ElasticMapping
    {
        private readonly string _index;

        public IndexMapping(string index)
        {
            _index = index;
        }

        public override string GetIndexForType(Type type)
        {
            return _index;
        }
    }
}