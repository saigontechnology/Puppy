using System;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.CharFilters
{
    //"char_filter" : {
    //	   "my_mapping" : {
    //		   "type" : "mapping",
    //		   "mappings" : ["ph=>f", "qu=>k"]
    //	   }
    //},
    public abstract class AnalysisCharFilterBase
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