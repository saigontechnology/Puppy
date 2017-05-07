using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Queries
{
	/// <summary>
	///     The boosting query can be used to effectively demote results that match a given query. Unlike the "NOT" clause in bool query, this still selects documents that contain undesirable terms, but reduces their overall score. { "boosting" : { "positive" : { "term" : { "field1" : "value1" } },
	///     "negative" : { "term" : { "field2" : "value2" } }, "negative_boost" : 0.2 } }
	/// </summary>
	public class BoostingQuery : IQuery
    {
        private IQuery _negative;
        private double _negativeBoost;
        private bool _negativeBoostSet;
        private bool _negativeSet;
        private IQuery _positive;
        private bool _positiveSet;

        public BoostingQuery(IQuery positive, IQuery negative, double negativeBoost)
        {
            Positive = positive;
            Negative = negative;
            NegativeBoost = negativeBoost;
        }

	    /// <summary>
	    ///     positive 
	    /// </summary>
	    public IQuery Positive
        {
            get => _positive;
            set
            {
                _positive = value;
                _positiveSet = true;
            }
        }

        public IQuery Negative
        {
            get => _negative;
            set
            {
                _negative = value;
                _negativeSet = true;
            }
        }

	    /// <summary>
	    ///     negative_boost 
	    /// </summary>
	    public double NegativeBoost
        {
            get => _negativeBoost;
            set
            {
                _negativeBoost = value;
                _negativeBoostSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("boosting");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            if (_positiveSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("positive");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                _positive.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }

            if (_negativeSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("negative");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                _negative.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }

            JsonHelper.WriteValue("negative_boost", _negativeBoost, elasticCrudJsonWriter, _negativeBoostSet);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}