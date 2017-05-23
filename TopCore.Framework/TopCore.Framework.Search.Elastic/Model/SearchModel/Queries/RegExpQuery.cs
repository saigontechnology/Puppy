using TopCore.Framework.Search.Elastic.Model.SearchModel.Filters;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Queries
{
    /// <summary>
    ///     The regexp query allows you to use regular expression term queries. See Regular
    ///     expression syntax for details of the supported regular expression language. The "term
    ///     queries" in that first sentence means that Elastic will apply the regexp to the terms
    ///     produced by the tokenizer for that field, and not to the original text of the field.
    ///     Note: The performance of a regexp query heavily depends on the regular expression chosen.
    ///           Matching everything like .* is very slow as well as using lookaround regular
    ///           expressions. If possible, you should try to use a long prefix before your regular
    ///           expression starts. Wildcard matchers like .*?+ will mostly lower performance. http://www.elastic.org/guide/en/elastic/reference/current/query-dsl-regexp-query.html
    /// </summary>
    public class RegExpQuery : IQuery
    {
        private readonly string _field;
        private readonly string _regularExpression;
        private double _boost;
        private bool _boostSet;
        private RegExpFlags _flags;
        private bool _flagsSet;
        private uint _maxDeterminizedStates;
        private bool _maxDeterminizedStatesSet;

        public RegExpQuery(string field, string regularExpression)
        {
            _field = field;
            _regularExpression = regularExpression;
        }

        public RegExpFlags Flags
        {
            get => _flags;
            set
            {
                _flags = value;
                _flagsSet = true;
            }
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

        /// <summary>
        ///     max_determinized_states 
        /// </summary>
        public uint MaxDeterminizedStates
        {
            get => _maxDeterminizedStates;
            set
            {
                _maxDeterminizedStates = value;
                _maxDeterminizedStatesSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("regexp");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_field);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            JsonHelper.WriteValue("value", _regularExpression, elasticCrudJsonWriter);
            JsonHelper.WriteValue("flags", _flags.ToString(), elasticCrudJsonWriter, _flagsSet);
            JsonHelper.WriteValue("boost", _boost, elasticCrudJsonWriter, _boostSet);
            JsonHelper.WriteValue("max_determinized_states", _maxDeterminizedStates, elasticCrudJsonWriter,
                _maxDeterminizedStatesSet);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}