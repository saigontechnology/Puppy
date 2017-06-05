using Puppy.Search.Elastic.Utils;
using System.Collections.Generic;

namespace Puppy.Search.Elastic.Model.GeoModel
{
    public class GeoShapeMultiLineString : IGeoType
    {
        public List<List<GeoPoint>> Coordinates { get; set; }

        public string Type { get; set; }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            JsonHelper.WriteValue("type", DefaultGeoShapes.MultiLineString, elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("coordinates");
            elasticCrudJsonWriter.JsonWriter.WriteStartArray();
            foreach (var items in Coordinates)
            {
                elasticCrudJsonWriter.JsonWriter.WriteStartArray();
                foreach (var item in items)
                    item.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndArray();
            }
            elasticCrudJsonWriter.JsonWriter.WriteEndArray();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}