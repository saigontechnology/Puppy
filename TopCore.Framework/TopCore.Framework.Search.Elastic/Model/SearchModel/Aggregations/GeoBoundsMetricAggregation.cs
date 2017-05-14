using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Aggregations
{
    /// <summary>
    ///     A metric aggregation that computes the bounding box containing all geo_point values for a field.
    /// </summary>
    public class GeoBoundsMetricAggregation : BaseMetricAggregation
    {
        private bool _wrapLongitude;
        private bool _wrapLongitudeSet;

        public GeoBoundsMetricAggregation(string name, string field) : base("geo_bounds", name, field)
        {
        }

        public bool WrapLongitude
        {
            get => _wrapLongitude;
            set
            {
                _wrapLongitude = value;
                _wrapLongitudeSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("wrap_longitude", _wrapLongitude, elasticCrudJsonWriter, _wrapLongitudeSet);
        }
    }
}