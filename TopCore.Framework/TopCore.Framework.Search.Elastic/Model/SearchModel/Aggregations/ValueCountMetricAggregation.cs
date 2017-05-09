namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Aggregations
{
    public class ValueCountMetricAggregation : BaseMetricAggregation
    {
        public ValueCountMetricAggregation(string name, string field) : base("value_count", name, field)
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