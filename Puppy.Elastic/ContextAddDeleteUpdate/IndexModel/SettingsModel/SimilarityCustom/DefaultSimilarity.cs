using Puppy.Elastic.Model;
using Puppy.Elastic.Utils;

namespace Puppy.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.SimilarityCustom
{
    public class DefaultSimilarity : SimilarityBase
    {
        private bool _discountOverlaps;
        private bool _discountOverlapsSet;

        /// <summary>
        ///     The default similarity that is based on the TF/IDF model. 
        /// </summary>
        /// <param name="name"></param>
        public DefaultSimilarity(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultSimilarities.Default;
        }

        /// <summary>
        ///     discount_overlaps Determines whether overlap tokens (Tokens with 0 position
        ///     increment) are ignored when computing norm. By default this is true, meaning overlap
        ///                tokens do not count when computing norms.
        /// </summary>
        public bool DiscountOverlaps
        {
            get => _discountOverlaps;
            set
            {
                _discountOverlaps = value;
                _discountOverlapsSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteValue("discount_overlaps", _discountOverlaps, elasticCrudJsonWriter, _discountOverlapsSet);
        }
    }
}