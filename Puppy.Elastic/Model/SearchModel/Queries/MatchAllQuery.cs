using Puppy.Elastic.Utils;

namespace Puppy.Elastic.Model.SearchModel.Queries
{
    public class MatchAllQuery : IQuery
    {
        private double _boost;
        private bool _boostSet;

        public double Boost
        {
            get => _boost;
            set
            {
                _boost = value;
                _boostSet = true;
            }
        }

        //{
        // "query" : {
        //	  "match_all" : { }
        //  }
        //}
        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("match_all");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("boost", _boost, elasticCrudJsonWriter, _boostSet);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}