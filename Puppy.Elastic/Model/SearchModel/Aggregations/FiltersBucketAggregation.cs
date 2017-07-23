using System.Collections.Generic;

namespace Puppy.Elastic.Model.SearchModel.Aggregations
{
    /// <summary>
    ///     A multi-bucket value source based aggregation where buckets are dynamically built - one
    ///     per unique value.
    /// </summary>
    public class FiltersBucketAggregation : BaseBucketAggregation
    {
        private readonly List<IFilter> _filters;

        public FiltersBucketAggregation(string name, List<IFilter> filters)
            : base("filters", name)
        {
            _filters = filters;
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("filters");
            elasticCrudJsonWriter.JsonWriter.WriteStartArray();

            foreach (var filter in _filters)
            {
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                filter.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }

            elasticCrudJsonWriter.JsonWriter.WriteEndArray();
        }
    }
}