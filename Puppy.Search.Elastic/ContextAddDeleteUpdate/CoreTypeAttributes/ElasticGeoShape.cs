using System;
using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.ContextAddDeleteUpdate.CoreTypeAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ElasticGeoShape : ElasticGeoTypeAttribute
    {
        private string _distanceErrorPct;
        private bool _distanceErrorPctSet;
        private string _orientation;
        private bool _orientationSet;
        private string _precision;
        private bool _precisionSet;
        private GeoShapeTree _tree;
        private string _treeLevels;
        private bool _treeLevelsSet;
        private bool _treeSet;

        /// <summary>
        ///     tree Name of the PrefixTree implementation to be used: geohash for GeohashPrefixTree
        ///     and quadtree for QuadPrefixTree. Defaults to geohash.
        /// </summary>
        public virtual GeoShapeTree Tree
        {
            get => _tree;
            set
            {
                _tree = value;
                _treeSet = true;
            }
        }

        /// <summary>
        ///     precision This parameter may be used instead of tree_levels to set an appropriate
        ///     value for the tree_levels parameter. The value specifies the desired precision and
        ///     Elastic will calculate the best tree_levels value to honor this precision. The value
        ///     should be a number followed by an optional distance unit. Valid distance units
        ///     include: in, inch, yd, yard, mi, miles, km, kilometers, m,meters (default),
        ///              cm,centimeters, mm, millimeters.
        /// </summary>
        public virtual string Precision
        {
            get => _precision;
            set
            {
                _precision = value;
                _precisionSet = true;
            }
        }

        /// <summary>
        ///     tree_levels Maximum number of layers to be used by the PrefixTree. This can be used
        ///     to control the precision of shape representations and therefore how many terms are
        ///     indexed. Defaults to the default value of the chosen PrefixTree implementation. Since
        ///     this parameter requires a certain level of understanding of the underlying
        ///     implementation, users may use the precision parameter instead. However, Elastic only
        ///     uses the tree_levels parameter internally and this is what is returned via the
        ///     mapping API even if you use the precision parameter.
        /// </summary>
        public virtual string TreeLevels
        {
            get => _treeLevels;
            set
            {
                _treeLevels = value;
                _treeLevelsSet = true;
            }
        }

        /// <summary>
        ///     distance_error_pct Used as a hint to the PrefixTree about how precise it should be.
        ///     Defaults to 0.025 (2.5%) with 0.5 as the maximum supported value.
        /// </summary>
        public virtual string DistanceErrorPct
        {
            get => _distanceErrorPct;
            set
            {
                _distanceErrorPct = value;
                _distanceErrorPctSet = true;
            }
        }

        /// <summary>
        ///     orientation Optionally define how to interpret vertex order for polygons /
        ///     multipolygons. This parameter defines one of two coordinate system rules (Right-hand
        ///     or Left-hand) each of which can be specified in three different ways.
        ///     1. Right-hand rule (default): right, ccw, counterclockwise,
        ///     2. Left-hand rule: left, cw, clockwise. The default orientation (counterclockwise)
        ///        complies with the OGC standard which defines outer ring vertices in
        ///        counterclockwise order with inner ring(s) vertices (holes) in clockwise order.
        ///        Setting this parameter in the geo_shape mapping explicitly sets vertex order for
        ///        the coordinate list of a geo_shape field but can be overridden in each individual
        ///        GeoJSON document.
        /// </summary>
        public virtual string Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                _orientationSet = true;
            }
        }

        public override string JsonString()
        {
            var elasticCrudJsonWriter = new ElasticJsonWriter();
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("type", "geo_shape", elasticCrudJsonWriter);
            JsonHelper.WriteValue("tree", _tree.ToString(), elasticCrudJsonWriter, _treeSet);
            JsonHelper.WriteValue("precision", _precision, elasticCrudJsonWriter, _precisionSet);
            JsonHelper.WriteValue("tree_levels", _treeLevels, elasticCrudJsonWriter, _treeLevelsSet);
            JsonHelper.WriteValue("distance_error_pct", _distanceErrorPct, elasticCrudJsonWriter, _distanceErrorPctSet);
            JsonHelper.WriteValue("orientation", _orientation, elasticCrudJsonWriter, _orientationSet);

            WriteBaseValues(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            return elasticCrudJsonWriter.Stringbuilder.ToString();
        }
    }

    public enum GeoShapeTree
    {
        quadtree,
        geohash
    }
}