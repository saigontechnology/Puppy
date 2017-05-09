namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Filters
{
    public class NotFilter : IFilter
    {
        private IFilter _not;
        private bool _notSet;

        public NotFilter(IFilter not)
        {
            Not = not;
        }

        public IFilter Not
        {
            get => _not;
            set
            {
                _not = value;
                _notSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("not");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            WriteNotFilter(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }

        private void WriteNotFilter(ElasticJsonWriter elasticCrudJsonWriter)
        {
            if (_notSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("filter");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                _not.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }
        }
    }
}