using Puppy.Elastic.Model.Units;

namespace Puppy.Elastic.Model.SearchModel.Queries.FunctionQuery
{
    public class GaussDateTimePointFunction : DateTimeDecayBaseScoreFunction
    {
        public GaussDateTimePointFunction(string field, TimeUnit scale) : base(field, scale, "gauss")
        {
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }
    }
}