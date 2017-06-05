using Puppy.Search.Elastic.Utils;
using System.Collections.Generic;

namespace Puppy.Search.Elastic.Model.SearchModel.Sorting
{
    /// <summary>
    ///     Allows to add one or more sort on specific fields. Each sort can be reversed as well. The
    ///     sort is defined on a per field level, with special field name for _score to sort by
    ///     score. When sorting, the relevant sorted field values are loaded into memory. This means
    ///     that per shard, there should be enough memory to contain them. For string based types,
    ///     the field sorted on should not be analyzed / tokenized. For numeric types, if possible,
    ///     it is recommended to explicitly set the type to six_hun types (like short, integer and float).
    /// </summary>
    public class SortHolder : ISortHolder
    {
        private readonly List<ISort> _sortItems;
        private bool _trackScores;
        private bool _trackScoresSet;

        public SortHolder(List<ISort> sortItems)
        {
            if (sortItems == null)
                throw new ElasticException("SortHolder requires some sort values");
            _sortItems = sortItems;
        }

        /// <summary>
        ///     track_scores When sorting on a field, scores are not computed. By setting
        ///     track_scores to true, scores will still be computed and tracked.
        /// </summary>
        public bool TrackScores
        {
            get => _trackScores;
            set
            {
                _trackScores = value;
                _trackScoresSet = true;
            }
        }

        public void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("track_scores", _trackScores, elasticCrudJsonWriter, _trackScoresSet);
            elasticCrudJsonWriter.JsonWriter.WritePropertyName("sort");
            elasticCrudJsonWriter.JsonWriter.WriteStartArray();

            foreach (var item in _sortItems)
            {
                elasticCrudJsonWriter.JsonWriter.WriteStartObject();
                item.WriteJson(elasticCrudJsonWriter);
                elasticCrudJsonWriter.JsonWriter.WriteEndObject();
            }

            elasticCrudJsonWriter.JsonWriter.WriteEndArray();
        }
    }

    public interface ISortHolder
    {
        void WriteJson(ElasticJsonWriter elasticCrudJsonWriter);
    }
}