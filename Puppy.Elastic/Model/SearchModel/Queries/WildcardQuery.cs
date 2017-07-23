using Puppy.Elastic.Utils;

namespace Puppy.Elastic.Model.SearchModel.Queries
{
    public class WildcardQuery : IQuery
    {
        private readonly string _field;
        private readonly object _wildcard;
        private double _boost;
        private bool _boostSet;

        public WildcardQuery(string field, object wildcard)
        {
            _field = field;
            _wildcard = wildcard;
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
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("wildcard");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_field);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("wildcard", _wildcard, elasticCrudJsonWriter);
            JsonHelper.WriteValue("boost", _boost, elasticCrudJsonWriter, _boostSet);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}