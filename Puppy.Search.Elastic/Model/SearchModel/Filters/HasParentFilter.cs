using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.Model.SearchModel.Filters
{
    /// <summary>
    ///     The has_parent filter accepts a query and a parent type. The query is executed in the
    ///     parent document space, which is specified by the parent type. This filter returns child
    ///     documents which associated parents have matched. For the rest has_parent filter has the
    ///     same options and works in the same manner as the has_child filter.
    /// </summary>
    public class HasParentFilter : IFilter
    {
        private readonly IFilter _filter;
        private readonly string _type;
        private InnerHits _innerHits;
        private bool _innerHitsSet;

        public HasParentFilter(string type, IFilter filter)
        {
            _type = type;
            _filter = filter;
        }

        public InnerHits InnerHits
        {
            get => _innerHits;
            set
            {
                _innerHits = value;
                _innerHitsSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("has_parent");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("type", _type, elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("filter");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            _filter.WriteJson(elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();

            if (_innerHitsSet)
                _innerHits.WriteJson(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}