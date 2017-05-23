using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.GeoModel
{
    public class GeoShapeCircle : IGeoType
    {
        public GeoPoint Coordinates { get; set; }

        /// <summary>
        ///     The inner radius field is required. If not specified, then the units of the radius
        ///     will default to METERS.
        /// </summary>
        public string Radius { get; set; }

        public string Type { get; set; }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            JsonHelper.WriteValue("type", DefaultGeoShapes.Circle, elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("coordinates");
            Coordinates.WriteJson(elasticCrudJsonWriter);
            JsonHelper.WriteValue("radius", Radius, elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}