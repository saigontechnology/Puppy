using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Filters
{
    public class NestedFilter : IFilter
    {
        private readonly IFilter _filter;
        private readonly string _path;
        private InnerHits _innerHits;
        private bool _innerHitsSet;
        private bool _join;
        private bool _joinSet;

        public NestedFilter(IFilter filter, string path)
        {
            _filter = filter;
            _path = path;
        }

        public bool Join
        {
            get => _join;
            set
            {
                _join = value;
                _joinSet = true;
            }
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
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("nested");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("path", _path, elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("filter");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            _filter.WriteJson(elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();

            JsonHelper.WriteValue("join", _join, elasticCrudJsonWriter, _joinSet);

            if (_innerHitsSet)
                _innerHits.WriteJson(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}