namespace Puppy.Elastic.Model.SearchModel.Aggregations
{
    public class GlobalBucketAggregation : BaseBucketAggregation
    {
        private readonly string _name;

        public GlobalBucketAggregation(string name) : base("global", name)
        {
            _name = name;
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