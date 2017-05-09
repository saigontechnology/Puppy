using System;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.SimilarityCustom
{
    public abstract class SimilarityBase
    {
        protected bool AnalyzerSet;
        protected string Name;
        protected string Type;

        public abstract void WriteJson(ElasticJsonWriter elasticCrudJsonWriter);

        protected virtual void WriteJsonBase(ElasticJsonWriter elasticCrudJsonWriter,
            Action<ElasticJsonWriter> writeFilterSpecific)
        {
            if (AnalyzerSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName(Name);
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();

                writeFilterSpecific.Invoke(elasticCrudJsonWriter);

                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }
        }
    }
}