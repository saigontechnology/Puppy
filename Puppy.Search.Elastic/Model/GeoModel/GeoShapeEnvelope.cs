using Puppy.Search.Elastic.Utils;
using System.Collections.Generic;

namespace Puppy.Search.Elastic.Model.GeoModel
{
    public class GeoShapeEnvelope : IGeoType
    {
        // TODO validate that at least two items exist
        public List<GeoPoint> Coordinates { get; set; }

        public string Type { get; set; }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            JsonHelper.WriteValue("type", DefaultGeoShapes.Envelope, elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("coordinates");
            elasticCrudJsonWriter.JsonWriter.WriteStartArray();
            foreach (var item in Coordinates)
                item.WriteJson(elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndArray();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}