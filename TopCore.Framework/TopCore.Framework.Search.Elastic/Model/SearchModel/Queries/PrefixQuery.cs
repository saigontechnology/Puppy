using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Queries
{
	public class PrefixQuery : IQuery
    {
        private readonly string _field;
        private readonly object _prefix;
        private double _boost;
        private bool _boostSet;

        public PrefixQuery(string field, object prefix)
        {
            _field = field;
            _prefix = prefix;
        }

        public double Boost
        {
            get => _boost;
            set
            {
                _boost = value;
                _boostSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("prefix");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_field);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("prefix", _prefix, elasticCrudJsonWriter);
            JsonHelper.WriteValue("boost", _boost, elasticCrudJsonWriter, _boostSet);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }

    public class TemplateQuery : IQuery
    {
        private readonly string _field;
        private readonly object _prefix;
        private double _boost;
        private bool _boostSet;

        public TemplateQuery(string field, object prefix)
        {
            _field = field;
            _prefix = prefix;
        }

        public double Boost
        {
            get => _boost;
            set
            {
                _boost = value;
                _boostSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("prefix");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_field);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("prefix", _prefix, elasticCrudJsonWriter);
            JsonHelper.WriteValue("boost", _boost, elasticCrudJsonWriter, _boostSet);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}