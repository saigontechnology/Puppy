using Puppy.Search.Elastic.Model.GeoModel;
using Puppy.Search.Elastic.Model.Units;

namespace Puppy.Search.Elastic.Model.SearchModel.Queries.FunctionQuery
{
    public class LinearGeoPointFunction : GeoDecayBaseScoreFunction
    {
        public LinearGeoPointFunction(string field, GeoPoint origin, DistanceUnit scale) : base(field, origin, scale,
            "linear")
        {
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }
    }
}