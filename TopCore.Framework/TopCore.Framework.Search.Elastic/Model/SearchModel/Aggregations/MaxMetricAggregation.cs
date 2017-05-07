namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Aggregations
{
	public class MaxMetricAggregation : BaseMetricAggregation
    {
        public MaxMetricAggregation(string name, string field) : base("max", name, field)
        {
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
        }
    }
}