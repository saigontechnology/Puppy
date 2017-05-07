using TopCore.Framework.Search.Elastic.Model.Units;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Queries.FunctionQuery
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