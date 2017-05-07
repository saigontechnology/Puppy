using System.Collections.Generic;
using TopCore.Framework.Search.Elastic.Model.GeoModel;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Sorting
{
	public class SortGeoDistance : ISort
    {
        private readonly string _field;
        private GeoPoint _geoPoint;
        private List<GeoPoint> _geoPoints;
        private bool _geoPointSet;
        private bool _geoPointsSet;
        private SortModeGeo _mode;
        private bool _modeSet;

        public SortGeoDistance(string field, DistanceUnitEnum distanceUnit, GeoPoint geoPoint)
        {
            _field = field;
            Order = OrderEnum.asc;
            Unit = distanceUnit;
            GeoPoint = geoPoint;
        }

        public SortGeoDistance(string field, DistanceUnitEnum distanceUnit, List<GeoPoint> geoPoints)
        {
            _field = field;
            Order = OrderEnum.asc;
            Unit = distanceUnit;
            GeoPoints = geoPoints;
        }

        public OrderEnum Order { get; set; }

	    /// <summary>
	    ///     mode Elastic supports sorting by array or multi-valued fields. The mode option controls what array value is picked for sorting the document it belongs to. The mode option can have the following values: SortMode enum: min, max, avg 
	    /// </summary>
	    public SortModeGeo Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                _modeSet = true;
            }
        }

        public GeoPoint GeoPoint
        {
            get => _geoPoint;
            set
            {
                _geoPoint = value;
                _geoPointSet = true;
            }
        }

        public List<GeoPoint> GeoPoints
        {
            get => _geoPoints;
            set
            {
                _geoPoints = value;
                _geoPointsSet = true;
            }
        }

        public DistanceUnitEnum Unit { get; set; }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("_geo_distance");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            if (_geoPointSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName(_field);
                _geoPoint.WriteJson(elasticCrudJsonWriter);
            }
            else if (_geoPointsSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName(_field);
                elasticCrudJsonWriter.JsonWriter.WriteStartArray();

                foreach (var item in _geoPoints)
                    item.WriteJson(elasticCrudJsonWriter);

                elasticCrudJsonWriter.JsonWriter.WriteEndArray();
            }
            JsonHelper.WriteValue("order", Order.ToString(), elasticCrudJsonWriter);
            JsonHelper.WriteValue("mode", _mode.ToString(), elasticCrudJsonWriter, _modeSet);
            JsonHelper.WriteValue("unit", Unit.ToString(), elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }

    public enum SortModeGeo
    {
	    /// <summary>
	    ///     Pick the lowest value. 
	    /// </summary>
	    min,

	    /// <summary>
	    ///     Pick the highest value. 
	    /// </summary>
	    max,

	    /// <summary>
	    ///     Use the average of all values as sort value. Only applicable for number based array fields. 
	    /// </summary>
	    avg
    }
}