using System;

namespace Puppy.Elastic.Model
{
    public class IndexTypeMapping : ElasticMapping
    {
        private readonly string _index;
        private readonly string _indexType;
        private readonly Type _type;

        public IndexTypeMapping(string index, string indexType, Type type)
        {
            _index = index;
            _indexType = indexType;
            _type = type;
        }

        public override string GetIndexForType(Type type)
        {
            return _index;
        }

        public override string GetDocumentType(Type type)
        {
            if (_type == type)
                return _indexType;

            return base.GetDocumentType(type);
        }
    }
}