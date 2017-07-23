using System;
using System.Collections.Generic;

namespace Puppy.Elastic.Model.SearchModel.Aggregations
{
    public abstract class BaseBucketAggregation : IAggs
    {
        private readonly string _name;
        private readonly string _type;

        private List<IAggs> _aggs;
        private bool _aggsSet;

        public BaseBucketAggregation(string type, string name)
        {
            _type = type;
            _name = name;
        }

        public List<IAggs> Aggs
        {
            get => _aggs;
            set
            {
                if (value.Exists(l => l.GetType() == typeof(GlobalBucketAggregation)))
                    throw new ElasticException("GlobalBucketAggregation cannot be sub aggregations");
                if (value.Exists(l => l.GetType() == typeof(ReverseNestedBucketAggregation)) &&
                    GetType() != typeof(NestedBucketAggregation))
                    throw new ElasticException(
                        "ReverseNestedBucketAggregation can only be defined in a NestedBucketAggregation");
                _aggs = value;
                _aggsSet = true;
            }
        }

        public abstract void WriteJson(ElasticJsonWriter elasticCrudJsonWriter);

        protected virtual void WriteJsonBase(ElasticJsonWriter elasticCrudJsonWriter,
            Action<ElasticJsonWriter> writeFilterSpecific)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_name);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_type);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            writeFilterSpecific.Invoke(elasticCrudJsonWriter);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();

            if (_aggsSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("aggs");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                foreach (var item in _aggs)
                    item.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}