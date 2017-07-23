using Puppy.Elastic.Model.GeoModel;
using Puppy.Elastic.Model.Units;

namespace Puppy.Elastic.Model.SearchModel.Queries.FunctionQuery
{
    public class ExpGeoPointFunction : GeoDecayBaseScoreFunction
    {
        public ExpGeoPointFunction(string field, GeoPoint origin, DistanceUnit scale) : base(field, origin, scale,
            "exp")
        {
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }
    }
}