using Puppy.Elastic.Utils;

namespace Puppy.Elastic.Model.SearchModel.Filters
{
    public class TermFilter : IFilter
    {
        private readonly string _term;
        private readonly string _termValue;

        public TermFilter(string term, string termValue)
        {
            _term = term;
            _termValue = termValue;
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("term");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue(_term, _termValue, elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}