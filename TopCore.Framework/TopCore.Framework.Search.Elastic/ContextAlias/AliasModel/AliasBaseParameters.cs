using System;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAlias.AliasModel
{
    public abstract class AliasBaseParameters
    {
        private readonly string _alias;
        private readonly string _index;

        protected AliasBaseParameters(string alias, string index)
        {
            MappingUtils.GuardAgainstBadIndexName(alias);
            MappingUtils.GuardAgainstBadIndexName(index);

            _alias = alias;
            _index = index;
        }

        public abstract void WriteJson(ElasticJsonWriter elasticCrudJsonWriter);

        protected void WriteInternalJson(ElasticJsonWriter elasticCrudJsonWriter, AliasAction aliasAction,
            Action<ElasticJsonWriter> writeFilterSpecific = null)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName(aliasAction.ToString());
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("index", _index, elasticCrudJsonWriter);
            JsonHelper.WriteValue("alias", _alias, elasticCrudJsonWriter);
            if (writeFilterSpecific != null)
                writeFilterSpecific.Invoke(elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}