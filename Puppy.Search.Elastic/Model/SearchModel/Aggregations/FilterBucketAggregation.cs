namespace Puppy.Search.Elastic.Model.SearchModel.Aggregations
{
    public class FilterBucketAggregation : BaseBucketAggregation
    {
        private readonly IFilter _filter;

        public FilterBucketAggregation(string name, IFilter filter) : base("filter", name)
        {
            _filter = filter;
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            _filter.WriteJson(elasticCrudJsonWriter);
        }
    }
}