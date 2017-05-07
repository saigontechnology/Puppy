namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Aggregations.RangeParam
{
	public abstract class RangeAggregationParameter<T>
    {
        protected bool KeySet;
        protected string KeyValue;

        public string Key
        {
            get => KeyValue;
            set
            {
                KeyValue = value;
                KeySet = true;
            }
        }

        public abstract void WriteJson(ElasticJsonWriter elasticCrudJsonWriter);
    }
}