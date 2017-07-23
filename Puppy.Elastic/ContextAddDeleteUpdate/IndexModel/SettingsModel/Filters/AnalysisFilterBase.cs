using System;

namespace Puppy.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
    // "blocks_filter" : { "type" : "word_delimiter", "preserve_original": "true" }, "shingle":{
    // "type":"shingle", "max_shingle_size":5, "min_shingle_size":2, "output_unigrams":"true" },
    // "filter_stop":{ "type":"stop", "enable_position_increments":"false" }
    public abstract class AnalysisFilterBase
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