using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Filters
{
    /// <summary>
    ///     The Filter also supports using a shape which has already been indexed in another index and/or index type. This is
    ///     particularly useful for when you have a pre-defined list of shapes which are useful to your application and you
    ///     want to reference this using a logical name (for example New
    ///     Zealand) rather than having to provide their coordinates each time. In this situation it is only necessary to
    ///     provide: id - The ID of the document that containing the pre-indexed shape. index - Name of the index where the
    ///     pre-indexed shape is. Defaults to shapes. type - Index type where
    ///     the pre-indexed shape is. path - The field specified as path containing the pre-indexed shape. Defaults to shape.
    /// </summary>
    public class GeoShapePreIndexedFilter : IFilter
    {
        private readonly string _field;

        public GeoShapePreIndexedFilter(string field)
        {
            _field = field;
        }

        /// <summary>
        ///     id - The ID of the document that containing the pre-indexed shape.
        /// </summary>
        public object Id { get; set; }

        /// <summary>
        ///     index - Name of the index where the pre-indexed shape is. Defaults to shapes.
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        ///     type - Index type where the pre-indexed shape is.
        /// </summary>
        public string PreIndexedType { get; set; }

        /// <summary>
        ///     path - The field specified as path containing the pre-indexed shape. Defaults to shape.
        /// </summary>
        public string Path { get; set; }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("geo_shape");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_field);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName("indexed_shape");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("id", Id, elasticCrudJsonWriter);
            JsonHelper.WriteValue("index", Index, elasticCrudJsonWriter);
            JsonHelper.WriteValue("type", PreIndexedType, elasticCrudJsonWriter);
            JsonHelper.WriteValue("path", Path, elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}