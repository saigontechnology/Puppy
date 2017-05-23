using TopCore.Framework.Search.Elastic.Model.SearchModel.Sorting;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel.Aggregations
{
    public class TopHitsMetricAggregation : IAggs
    {
        private readonly string _name;
        private int _from;
        private bool _fromSet;
        private int _size;
        private bool _sizeSet;
        private ISortHolder _sortHolder;
        private bool _sortSet;

        public TopHitsMetricAggregation(string name)
        {
            _name = name;
        }

        /// <summary>
        ///     from The starting from index of the hits to return. Defaults to 0. 
        /// </summary>
        public int From
        {
            get => _from;
            set
            {
                _from = value;
                _fromSet = true;
            }
        }

        /// <summary>
        ///     size The number of hits to return. Defaults to 10. 
        /// </summary>
        public int Size
        {
            get => _size;
            set
            {
                _size = value;
                _sizeSet = true;
            }
        }

        public ISortHolder Sort
        {
            get => _sortHolder;
            set
            {
                _sortHolder = value;
                _sortSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName(_name);
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("top_hits");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("from", _from, elasticCrudJsonWriter, _fromSet);
            JsonHelper.WriteValue("size", _size, elasticCrudJsonWriter, _sizeSet);
            if (_sortSet)
                _sortHolder.WriteJson(elasticCrudJsonWriter);
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}