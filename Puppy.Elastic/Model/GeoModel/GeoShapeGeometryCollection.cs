using Newtonsoft.Json;
using Puppy.Elastic.Utils;
using System.Collections.Generic;

namespace Puppy.Elastic.Model.GeoModel
{
    public class GeoShapeGeometryCollection : IGeoType
    {
        [JsonConverter(typeof(GeoShapeGeometryCollectionGeometriesConverter))]
        public List<object> Geometries { get; set; }

        public string Type { get; set; }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            JsonHelper.WriteValue("type", DefaultGeoShapes.GeometryCollection, elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("geometries");
            elasticCrudJsonWriter.JsonWriter.WriteStartArray();
            foreach (var item in Geometries)
                (item as IGeoType).WriteJson(elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndArray();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}