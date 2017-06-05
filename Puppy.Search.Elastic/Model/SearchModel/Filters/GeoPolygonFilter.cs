using Puppy.Search.Elastic.Model.GeoModel;
using System.Collections.Generic;

namespace Puppy.Search.Elastic.Model.SearchModel.Filters
{
    public class GeoPolygonFilter : IFilter
    {
        private readonly string _field;
        private readonly List<GeoPoint> _locations;

        public GeoPolygonFilter(string field, List<GeoPoint> locations)
        {
            if (locations.Count < 3)
                throw new ElasticException("A Polygon has at least 3 points!");
            _field = field;
            _locations = locations;
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("geo_polygon");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_field);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("points");
            elasticCrudJsonWriter.JsonWriter.WriteStartArray();
            foreach (var item in _locations)
                item.WriteJson(elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndArray();

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}