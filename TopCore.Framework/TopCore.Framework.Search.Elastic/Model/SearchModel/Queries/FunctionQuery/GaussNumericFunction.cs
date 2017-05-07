namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Queries.FunctionQuery
{
	public class GaussNumericFunction<T> : DecayBaseScoreFunction<T>
    {
        public GaussNumericFunction(string field, T origin, T scale) : base(field, origin, scale, "gauss")
        {
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }
    }
}