using TopCore.Framework.Search.Elastic.Model.GeoModel;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Filters
{
    /// <summary>
    ///     A filter allowing to filter hits based on a point location using a bounding box.
    /// </summary>
    public class GeoBoundingBoxFilter : IFilter
    {
        private readonly GeoPoint _bottomRight;
        private readonly string _field;
        private readonly GeoPoint _topLeft;
        private GeoBoundingBoxFilterType _type;
        private bool _typeSet;

        public GeoBoundingBoxFilter(string field, GeoPoint topLeft, GeoPoint bottomRight)
        {
            _field = field;
            _topLeft = topLeft;
            _bottomRight = bottomRight;
        }

        /// <summary>
        ///     type The type of the bounding box execution by default is set to memory, which means in memory checks if the doc
        ///     falls within the bounding box range. In some cases, an indexed option will perform faster (but note that the
        ///     geo_point type must have lat and lon indexed in this case).
        ///     Note, when using the indexed option, multi locations per document field are not supported.
        /// </summary>
        public GeoBoundingBoxFilterType Type
        {
            get => _type;
            set
            {
                _type = value;
                _typeSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("geo_bounding_box");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("type", _type.ToString(), elasticCrudJsonWriter, _typeSet);
            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_field);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName("top_left");
            _topLeft.WriteJson(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WritePropertyName("bottom_right");
            _bottomRight.WriteJson(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }

    public enum GeoBoundingBoxFilterType
    {
        memory,
        indexed
    }
}