using System;

namespace Puppy.Search.Elastic
{
    public interface IElasticMappingResolver
    {
        ElasticMapping GetElasticSearchMapping(Type type);

        void AddElasticSearchMappingForEntityType(Type type, ElasticMapping mapping);
    }
}