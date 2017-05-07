namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Aggregations
{
	public class AvgMetricAggregation : BaseMetricAggregation
    {
        public AvgMetricAggregation(string name, string field) : base("avg", name, field)
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