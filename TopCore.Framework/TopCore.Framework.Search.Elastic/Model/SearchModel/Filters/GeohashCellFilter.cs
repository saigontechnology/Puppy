using TopCore.Framework.Search.Elastic.Model.GeoModel;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Filters
{
    public class GeohashCellFilter : IFilter
    {
        private readonly string _field;
        private readonly GeoPoint _location;
        private readonly bool _neighbors;
        private readonly int _precision;

        public GeohashCellFilter(string field, GeoPoint location, int precision, bool neighbors)
        {
            _field = field;
            _location = location;
            _precision = precision;
            _neighbors = neighbors;
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("geohash_cell");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_field);
            _location.WriteJson(elasticCrudJsonWriter);
            JsonHelper.WriteValue("precision", _precision, elasticCrudJsonWriter);
            JsonHelper.WriteValue("neighbors", _neighbors, elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}