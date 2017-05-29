using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.Model.SearchModel.Aggregations.RangeParam
{
    public class ToRangeAggregationParameter<T> : RangeAggregationParameter<T>
    {
        private readonly T _value;

        public ToRangeAggregationParameter(T value)
        {
            _value = value;
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            JsonHelper.WriteValue("key", KeyValue, elasticCrudJsonWriter, KeySet);
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("to");
            elasticCrudJsonWriter.JsonWriter.WriteValue(_value);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}