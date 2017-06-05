using Puppy.Search.Elastic.Utils;
using System.Collections.Generic;

namespace Puppy.Search.Elastic.Model.GeoModel
{
    public class GeoShapeMultiPolygon : IGeoType
    {
        // TODO validate that first and the last items in each polygon are the same
        public List<List<List<GeoPoint>>> Coordinates { get; set; }

        public string Type { get; set; }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            JsonHelper.WriteValue("type", DefaultGeoShapes.MultiPolygon, elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("coordinates");
            elasticCrudJsonWriter.JsonWriter.WriteStartArray();
            foreach (var itemlists in Coordinates)
            {
                elasticCrudJsonWriter.JsonWriter.WriteStartArray();
                foreach (var items in itemlists)
                {
                    elasticCrudJsonWriter.JsonWriter.WriteStartArray();
                    foreach (var item in items)
                        item.WriteJson(elasticCrudJsonWriter);
                    elasticCrudJsonWriter.JsonWriter.WriteEndArray();
                }
                elasticCrudJsonWriter.JsonWriter.WriteEndArray();
            }
            elasticCrudJsonWriter.JsonWriter.WriteEndArray();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}