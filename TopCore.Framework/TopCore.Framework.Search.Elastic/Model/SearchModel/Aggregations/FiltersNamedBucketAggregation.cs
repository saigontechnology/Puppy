using System.Collections.Generic;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Aggregations
{
	public class FiltersNamedBucketAggregation : BaseBucketAggregation
    {
        private readonly List<NamedFilter> _namedFilters;

        public FiltersNamedBucketAggregation(string name, List<NamedFilter> namedFilters)
            : base("filters", name)
        {
            _namedFilters = namedFilters;
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("filters");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            foreach (var filter in _namedFilters)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName(filter.Name);
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                filter.Filter.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }

    public class NamedFilter
    {
        public NamedFilter(string name, IFilter filter)
        {
            Name = name;
            Filter = filter;
        }

        public IFilter Filter { get; }
        public string Name { get; }
    }
}