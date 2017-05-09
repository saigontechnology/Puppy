using TopCore.Framework.Search.Elastic.Model.SearchModel.Sorting;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.Model.SearchModel
{
    public class InnerHits
    {
        private int _from;
        private bool _fromSet;
        private string _name;
        private bool _nameSet;
        private int _size;
        private bool _sizeSet;
        private ISortHolder _sortHolder;
        private bool _sortHolderSet;

        /// <summary>
        ///     name The name to be used for the particular inner hit definition in the response. Useful when multiple inner hits have been defined in a single search request. The default depends in which query the inner hit is defined. For has_child query and filter this is the child type,
        ///     has_parent query and filter this is the parent type and the nested query and filter this is the nested path.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                _nameSet = true;
            }
        }

        /// <summary>
        ///     How the inner hits should be sorted per inner_hits. By default the hits are sorted by the score. 
        /// </summary>
        public ISortHolder Sort
        {
            get => _sortHolder;
            set
            {
                _sortHolder = value;
                _sortHolderSet = true;
            }
        }

        /// <summary>
        ///     from The offset from where the first hit to fetch for each inner_hits in the returned regular search hits. 
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
        ///     size The maximum number of hits to return per inner_hits. By default the top three matching hits are returned. 
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

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("inner_hits");
            elasticCrudJsonWriter.JsonWriter.WriteStartObject();

            JsonHelper.WriteValue("from", _from, elasticCrudJsonWriter, _fromSet);
            JsonHelper.WriteValue("size", _size, elasticCrudJsonWriter, _sizeSet);

            if (_sortHolderSet)
                _sortHolder.WriteJson(elasticCrudJsonWriter);
            JsonHelper.WriteValue("name", _name, elasticCrudJsonWriter, _nameSet);

            elasticCrudJsonWriter.JsonWriter.WriteEndObject();
        }
    }
}