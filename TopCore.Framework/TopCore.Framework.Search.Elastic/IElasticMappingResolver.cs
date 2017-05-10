using System;

namespace TopCore.Framework.Search.Elastic
{
    public interface IElasticMappingResolver
    {
        ElasticMapping GetElasticSearchMapping(Type type);

        void AddElasticSearchMappingForEntityType(Type type, ElasticMapping mapping);
    }
}