using System;

namespace Puppy.Elastic
{
    public interface IElasticMappingResolver
    {
        ElasticMapping GetElasticSearchMapping(Type type);

        void AddElasticSearchMappingForEntityType(Type type, ElasticMapping mapping);
    }
}