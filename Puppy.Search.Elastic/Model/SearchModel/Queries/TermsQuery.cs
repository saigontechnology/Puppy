using Puppy.Search.Elastic.Utils;
using System.Collections.Generic;

namespace Puppy.Search.Elastic.Model.SearchModel.Queries
{
    public class TermsQuery : IQuery
    {
        private readonly string _term;
        private readonly List<object> _termValues;
        private double _boost;
        private bool _boostSet;

        public TermsQuery(string term, List<object> termValues)
        {
            _term = term;
            _termValues = termValues;
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
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("terms");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteListValue(_term, _termValues, elasticCrudJsonWriter);
            JsonHelper.WriteValue("boost", _boost, elasticCrudJsonWriter, _boostSet);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}