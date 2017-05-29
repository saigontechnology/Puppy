namespace Puppy.Search.Elastic
{
    /// <summary>
    ///     Configuration class used for the context settings. 
    /// </summary>
    public class ElasticSerializerConfiguration
    {
        public ElasticSerializerConfiguration(IElasticMappingResolver elasticMappingResolver,
            bool saveChildObjectsAsWellAsParent = true, bool processChildDocumentsAsSeparateChildIndex = false,
            bool userDefinedRouting = false)
        {
            ElasticMappingResolver = elasticMappingResolver;
            SaveChildObjectsAsWellAsParent = saveChildObjectsAsWellAsParent;
            ProcessChildDocumentsAsSeparateChildIndex = processChildDocumentsAsSeparateChildIndex;
            UserDefinedRouting = userDefinedRouting;
        }

        /// <summary>
        ///     Mapping resolver used to get set each mapping configuration for a type. A type can
        ///     only have one mapping pro context.
        /// </summary>
        public IElasticMappingResolver ElasticMappingResolver { get; }

        /// <summary>
        ///     Saves all child objects as well as the parent if set. The child objects will be saved
        ///     as nested or as separate documents depending on ProcessChildDocumentsAsSeparateChildIndex
        /// </summary>
        public bool SaveChildObjectsAsWellAsParent { get; }

        /// <summary>
        ///     Context will save child objects as separate types in the same index if set. Otherwise
        ///     child itemas are saved as nested objects.
        /// </summary>
        public bool ProcessChildDocumentsAsSeparateChildIndex { get; }

        /// <summary>
        ///     This defines idf you want to define your own routing. You should only do this if you
        ///     know what you are doing or you have grandchild documents in a parent child grandchild
        ///     relationship. This makes certain that the grandchildren are defined on the same
        ///     shard. It also cuses that gets, etc required the routing value of the parent route document...
        /// </summary>
        public bool UserDefinedRouting { get; }
    }
}