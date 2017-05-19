using System;
using System.Collections.Generic;

namespace TopCore.Framework.Search.Elastic
{
    /// <summary>
    ///   This class is used to add all register all the type definitions and then resolver them when requesting or handling response data from
    ///   Elastic. If no mapping is defined, the default mapping is used.
    /// </summary>
    public class ElasticMappingResolver : IElasticMappingResolver
    {
        private readonly Dictionary<Type, ElasticMapping> _mappingDefinitions = new Dictionary<Type, ElasticMapping>();

        public ElasticMapping GetElasticSearchMapping(Type type)
        {
            if (_mappingDefinitions.ContainsKey(type))
                return _mappingDefinitions[type];

            _mappingDefinitions.Add(type, new ElasticMapping());
            return _mappingDefinitions[type];
        }

        /// <summary>
        ///   You can add custom Type handlers here for specific mapping. Only one mapping can be defined pro type. 
        /// </summary>
        /// <param name="type">    Type of class </param>
        /// <param name="mapping"> mapping definition. </param>
        public void AddElasticSearchMappingForEntityType(Type type, ElasticMapping mapping)
        {
            if (_mappingDefinitions.ContainsKey(type))
                throw new ElasticException("The mapping for this type is already defined.");
            _mappingDefinitions.Add(type, mapping);
        }
    }
}