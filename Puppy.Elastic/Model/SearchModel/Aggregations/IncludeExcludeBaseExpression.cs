using Puppy.Elastic.Utils;
using System.Collections.Generic;

namespace Puppy.Elastic.Model.SearchModel.Aggregations
{
    public abstract class IncludeExcludeBaseExpression
    {
        private readonly string _expressionProperty;
        private readonly string _pattern;
        private List<IncludeExcludeExpressionFlags> _includeExcludeExpressionFlags;
        private bool _includeExcludeExpressionFlagsSet;

        public IncludeExcludeBaseExpression(string pattern, string expressionProperty)
        {
            _pattern = pattern;
            _expressionProperty = expressionProperty;
        }

        public List<IncludeExcludeExpressionFlags> Flags
        {
            get => _includeExcludeExpressionFlags;
            set
            {
                _includeExcludeExpressionFlags = value;
                _includeExcludeExpressionFlagsSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_expressionProperty);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("pattern", _pattern, elasticCrudJsonWriter);
            if (_includeExcludeExpressionFlagsSet)
            {
                var flags = string.Join("|", _includeExcludeExpressionFlags.ToArray());
                ;
                JsonHelper.WriteValue("flags", flags, elasticCrudJsonWriter);
            }

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}