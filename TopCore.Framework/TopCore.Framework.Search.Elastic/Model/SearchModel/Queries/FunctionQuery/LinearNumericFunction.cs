namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Queries.FunctionQuery
{
	public class LinearNumericFunction<T> : DecayBaseScoreFunction<T>
    {
        public LinearNumericFunction(string field, T origin, T scale) : base(field, origin, scale, "linear")
        {
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }
    }
}