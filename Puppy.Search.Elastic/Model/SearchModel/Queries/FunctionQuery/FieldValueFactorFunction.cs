using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.Model.SearchModel.Queries.FunctionQuery
{
    public class FieldValueFactorFunction : BaseScoreFunction
    {
        private readonly string _field;
        private double _factor;
        private bool _factorSet;
        private FieldValueFactorModifier _modifier;
        private bool _modifierSet;

        /// <summary>
        ///     The field_value_factor function allows you to use a field from a document to
        ///     influence the score. It’s similar to using the script_score function, however, it
        ///     avoids the overhead of scripting. If used on a multi-valued field, only the first
        ///     value of the field is used in calculations. Keep in mind that taking the log() of 0,
        ///     or the square root of a negative number is an illegal operation, and an exception
        ///     will be thrown. Be sure to limit the values of the field with a range filter to avoid
        ///     this, or use log1p and ln1p.
        /// </summary>
        public FieldValueFactorFunction(string field)
        {
            _field = field;
        }

        /// <summary>
        ///     Optional factor to multiply the field value with, defaults to 1. 
        /// </summary>
        public double Factor
        {
            get => _factor;
            set
            {
                _factor = value;
                _factorSet = true;
            }
        }

        /// <summary>
        ///     modifier Modifier to apply to the field value, can be one of: none, log, log1p,
        ///     log2p, ln, ln1p, ln2p, square, sqrt, or reciprocal. Defaults to none.
        /// </summary>
        public FieldValueFactorModifier Modifier
        {
            get => _modifier;
            set
            {
                _modifier = value;
                _modifierSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("field_value_factor");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("field", _field, elasticCrudJsonWriter);
            JsonHelper.WriteValue("factor", _factor, elasticCrudJsonWriter, _factorSet);
            JsonHelper.WriteValue("modifier", _modifier.ToString(), elasticCrudJsonWriter, _modifierSet);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }

    public enum FieldValueFactorModifier
    {
        none,
        log,
        log1p,
        log2p,
        ln,
        ln1p,
        ln2p,
        square,
        sqrt,
        reciprocal
    }
}