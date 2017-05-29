using Puppy.Search.Elastic.Model.Units;

namespace Puppy.Search.Elastic.Model.SearchModel.Queries.FunctionQuery
{
    public class LinearDateTimePointFunction : DateTimeDecayBaseScoreFunction
    {
        public LinearDateTimePointFunction(string field, TimeUnit scale) : base(field, scale, "linear")
        {
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }
    }
}