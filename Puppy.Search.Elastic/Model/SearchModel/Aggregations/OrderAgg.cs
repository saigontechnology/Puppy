using Puppy.Search.Elastic.Model.SearchModel.Sorting;
using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.Model.SearchModel.Aggregations
{
    public class OrderAgg
    {
        private readonly string _field;
        private readonly OrderEnum _order;

        public OrderAgg(string field, OrderEnum order)
        {
            _field = field;
            _order = order;
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("order");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            JsonHelper.WriteValue(_field, _order.ToString(), elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}