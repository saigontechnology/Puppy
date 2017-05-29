using Puppy.Search.Elastic.Model.Units;

namespace Puppy.Search.Elastic.Model.SearchModel.Queries.FunctionQuery
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