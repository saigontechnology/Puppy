using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.GeoModel
{
    /// <summary>
    ///   A polygon is defined by a list of a list of points. The first and last points in each (outer) list must be the same (the polygon must be
    ///   closed). The first array represents the outer boundary of the polygon, the other arrays represent the interior shapes ("holes"): IMPORTANT
    ///   NOTE: GeoJSON does not mandate a specific order for vertices thus ambiguous polygons around the dateline and poles are possible. To alleviate
    ///   ambiguity the Open Geospatial Consortium (OGC) Simple Feature Access specification defines the following vertex ordering: Outer Ring -
    ///   Counterclockwise Inner Ring(s) / Holes - Clockwise For polygons that do not cross the dateline, vertex order will not matter in Elastic. For
    ///   polygons that do cross the dateline, Elastic requires vertex ordering to comply with the OGC specification. Otherwise, an unintended polygon
    ///   may be created and unexpected query/filter results will be returned. Elastic will apply OGC standards to eliminate ambiguity resulting in a
    ///   polygon that crosses the dateline. An orientation parameter can be defined when setting the geo_shape mapping (see the section called
    ///   “Mapping Optionsedit”). This will define vertex order for the coordinate list on the mapped geo_shape field. It can also be overridden on
    ///   each document.
    /// </summary>
    public class GeoShapePolygon : IGeoType
    {
        // TODO validate that first and the last items in each polygon are the same
        public List<List<GeoPoint>> Coordinates { get; set; }

        public string Type { get; set; }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            JsonHelper.WriteValue("type", DefaultGeoShapes.Polygon, elasticCrudJsonWriter);
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