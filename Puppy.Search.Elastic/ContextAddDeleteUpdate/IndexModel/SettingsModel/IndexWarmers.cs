using System.Collections.Generic;

namespace Puppy.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
    public class IndexWarmers
    {
        public IndexWarmers()
        {
            Warmers = new List<IndexWarmer>();
        }

        public List<IndexWarmer> Warmers { get; set; }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("warmers");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            foreach (var warmer in Warmers)
                warmer.WriteJson(elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}