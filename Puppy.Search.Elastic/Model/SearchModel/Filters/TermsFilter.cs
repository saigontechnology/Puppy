using System.Collections.Generic;
using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.Model.SearchModel.Filters
{
    public class TermsFilter : IFilter
    {
        private readonly string _term;
        private readonly List<object> _termValues;

        public TermsFilter(string term, List<object> termValues)
        {
            _term = term;
            _termValues = termValues;
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("terms");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteListValue(_term, _termValues, elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}