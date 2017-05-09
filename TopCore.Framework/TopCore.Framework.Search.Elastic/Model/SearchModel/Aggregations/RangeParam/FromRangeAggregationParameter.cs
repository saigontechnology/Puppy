using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Aggregations.RangeParam
{
    public class FromRangeAggregationParameter<T> : RangeAggregationParameter<T>
    {
        private readonly T _value;

        public FromRangeAggregationParameter(T value)
        {
            _value = value;
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            JsonHelper.WriteValue("key", KeyValue, elasticCrudJsonWriter, KeySet);
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("from");
            elasticCrudJsonWriter.JsonWriter.WriteValue(_value);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}