using System;

namespace TopCore.Framework.Search.Elastic.Utils
{
    /// <summary>
    ///   This mapping can be used if you require a search accross all indices and all types. 
    /// </summary>
    public class GlobalElasticMapping : ElasticMapping
    {
        public override string GetIndexForType(Type type)
        {
            return "";
        }

        public override string GetDocumentType(Type type)
        {
            return "";
        }
    }
}