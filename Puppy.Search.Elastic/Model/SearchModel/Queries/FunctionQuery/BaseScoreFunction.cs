using System;
using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.Model.SearchModel.Queries.FunctionQuery
{
    public abstract class BaseScoreFunction
    {
        private IFilter _filter;
        private bool _filterSet;
        private double _weight;
        private bool _weightSet;

        public IFilter Filter
        {
            get => _filter;
            set
            {
                _filter = value;
                _filterSet = true;
            }
        }

        public double Weight
        {
            get => _weight;
            set
            {
                _weight = value;
                _weightSet = true;
            }
        }

        public abstract void WriteJson(ElasticJsonWriter elasticCrudJsonWriter);

        protected virtual void WriteJsonBase(ElasticJsonWriter elasticCrudJsonWriter,
            Action<ElasticJsonWriter> writeFunctionSpecific)
        {
            if (_filterSet)
            {
                elasticCrudJsonWriter.JsonWriter.WritePropertyName("filter");
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                _filter.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }
            JsonHelper.WriteValue("weight", _weight, elasticCrudJsonWriter, _weightSet);

            writeFunctionSpecific.Invoke(elasticCrudJsonWriter);
        }
    }
}