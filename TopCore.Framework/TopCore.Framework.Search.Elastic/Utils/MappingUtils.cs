using System.Text.RegularExpressions;
using TopCore.Framework.Search.Elastic.Model;

namespace TopCore.Framework.Search.Elastic.Utils
{
    public static class MappingUtils
    {
        public static ElasticMapping GetElasticMapping<T>(IndexTypeDescription indexTypeDescription)
        {
            return new IndexTypeMapping(indexTypeDescription.Index, indexTypeDescription.IndexType, typeof(T));
        }

        public static ElasticMapping GetElasticMapping<T>(string index, string indexType)
        {
            return new IndexTypeMapping(index, indexType, typeof(T));
        }

        public static ElasticMapping GetElasticMapping(string index)
        {
            return new IndexMapping(index);
        }

        public static void GuardAgainstBadIndexName(string index)
        {
            if (Regex.IsMatch(index, "[\\\\/*?\",<>|\\sA-Z]"))
                throw new ElasticException(string.Format("ElasticJsonWriter: index is not allowed in Elastic: {0}",
                    index));
        }
    }
}