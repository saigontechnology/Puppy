using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.SimilarityCustom
{
    public class DfrSimilarity : SimilarityBase
    {
        private DfrAfterEffect _afterEffect;
        private bool _afterEffectSet;
        private DfrBasicModel _basicModel;
        private bool _basicModelSet;
        private DfrIbNormalization _normalization;
        private bool _normalizationSet;

        /// <summary>
        ///     Similarity that implements the divergence from randomness framework. http://lucene.apache.org/core/4_1_0/core/org/apache/lucene/search/similarities/DFRSimilarity.html 
        /// </summary>
        /// <param name="name"></param>
        public DfrSimilarity(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultSimilarities.Dfr;
        }

        /// <summary>
        ///     basic_model Possible values: be, d, g, if, in, ine and p 
        /// </summary>
        public DfrBasicModel BasicModel
        {
            get => _basicModel;
            set
            {
                _basicModel = value;
                _basicModelSet = true;
            }
        }

        /// <summary>
        ///     after_effect Possible values: no, b and l. 
        /// </summary>
        public DfrAfterEffect AfterEffect
        {
            get => _afterEffect;
            set
            {
                _afterEffect = value;
                _afterEffectSet = true;
            }
        }

        /// <summary>
        ///     normalization Possible values: no, h1, h2, h3 and z. 
        /// </summary>
        public DfrIbNormalization Normalization
        {
            get => _normalization;
            set
            {
                _normalization = value;
                _normalizationSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteValue("basic_model", _basicModel.ToString(), elasticCrudJsonWriter, _basicModelSet);
            JsonHelper.WriteValue("after_effect", _afterEffect.ToString(), elasticCrudJsonWriter, _afterEffectSet);
            JsonHelper.WriteValue("normalization", _normalization.ToString(), elasticCrudJsonWriter, _normalizationSet);
        }
    }

    public enum DfrBasicModel
    {
        be,
        d,
        g,
        @if,
        @in,
        ine,
        p
    }

    public enum DfrAfterEffect
    {
        no,
        b,
        l
    }

    public enum DfrIbNormalization
    {
        no,
        h1,
        h2,
        h3,
        z
    }
}