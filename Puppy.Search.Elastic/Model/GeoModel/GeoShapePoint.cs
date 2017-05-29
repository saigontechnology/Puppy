using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.Model.GeoModel
{
    public class GeoShapePoint : IGeoType
    {
        public GeoPoint Coordinates { get; set; }

        public string Type { get; set; }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            JsonHelper.WriteValue("type", DefaultGeoShapes.Point, elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("coordinates");
            Coordinates.WriteJson(elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}