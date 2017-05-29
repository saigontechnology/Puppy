using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.Model.SearchModel.Aggregations.RangeParam
{
    public class ToFromRangeAggregationParameter<T> : RangeAggregationParameter<T>
    {
        private readonly T _from;
        private readonly T _to;

        public ToFromRangeAggregationParameter(T to, T from)
        {
            _to = to;
            _from = from;
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            JsonHelper.WriteValue("key", KeyValue, elasticCrudJsonWriter, KeySet);
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("to");
            elasticCrudJsonWriter.JsonWriter.WriteValue(_to);
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("from");
            elasticCrudJsonWriter.JsonWriter.WriteValue(_from);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}