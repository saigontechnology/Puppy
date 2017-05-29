using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.Model.SearchModel.Queries
{
    /// <summary>
    ///     Matches spans containing a term. The span term query maps to Lucene SpanTermQuery. 
    /// </summary>
    public class SpanTermQuery : ISpanQuery
    {
        private readonly string _field;
        private readonly string _fieldValue;
        private double _boost;
        private bool _boostSet;

        public SpanTermQuery(string field, string fieldValue)
        {
            _field = field;
            _fieldValue = fieldValue;
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
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("span_term");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_field);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("term", _fieldValue, elasticCrudJsonWriter);
            JsonHelper.WriteValue("boost", _boost, elasticCrudJsonWriter, _boostSet);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}