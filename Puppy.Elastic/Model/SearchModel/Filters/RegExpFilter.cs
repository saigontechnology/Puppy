using Puppy.Elastic.Utils;

namespace Puppy.Elastic.Model.SearchModel.Filters
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
    public class RegExpFilter : IFilter
    {
        private readonly string _field;
        private readonly string _regularExpression;
        private RegExpFlags _flags;
        private bool _flagsSet;
        private uint _maxDeterminizedStates;
        private bool _maxDeterminizedStatesSet;
        private string _name;
        private bool _nameSet;

        public RegExpFilter(string field, string regularExpression)
        {
            _field = field;
            _regularExpression = regularExpression;
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

        public RegExpFlags Flags
        {
            get => _flags;
            set
            {
                _flags = value;
                _flagsSet = true;
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                _nameSet = true;
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

            JsonHelper.WriteValue("max_determinized_states", _maxDeterminizedStates, elasticCrudJsonWriter,
                _maxDeterminizedStatesSet);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();

            JsonHelper.WriteValue("_name", _name, elasticCrudJsonWriter, _nameSet);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }

    public enum RegExpFlags
    {
        INTERSECTION,
        COMPLEMENT,
        EMPTY
    }
}