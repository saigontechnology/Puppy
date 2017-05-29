using Puppy.Search.Elastic.Model.Units;

namespace Puppy.Search.Elastic.Model.SearchModel.Queries.FunctionQuery
{
    public class ExpDateTimePointFunction : DateTimeDecayBaseScoreFunction
    {
        public ExpDateTimePointFunction(string field, TimeUnit scale) : base(field, scale, "exp")
        {
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }
    }
}