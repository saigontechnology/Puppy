using TopCore.Framework.Search.Elastic.Model.GeoModel;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Queries
{
    public class GeoShapeQuery : IQuery
    {
        private readonly string _field;
        private readonly IGeoType _geoType;

        public GeoShapeQuery(string field, IGeoType geoType)
        {
            _field = field;
            _geoType = geoType;
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("geo_shape");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_field);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("shape");

            _geoType.WriteJson(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}