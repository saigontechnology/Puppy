namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Aggregations
{
    public class SumMetricAggregation : BaseMetricAggregation
    {
        public SumMetricAggregation(string name, string field) : base("sum", name, field)
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