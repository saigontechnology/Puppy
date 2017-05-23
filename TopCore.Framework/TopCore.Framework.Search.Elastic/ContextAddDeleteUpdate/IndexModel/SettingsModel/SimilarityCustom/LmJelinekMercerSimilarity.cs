using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.SimilarityCustom
{
    public class LmJelinekMercerSimilarity : SimilarityBase
    {
        private double _lambda;
        private bool _lambdaSet;

        /// <summary>
        ///     LM Jelinek Mercer similarity http://lucene.apache.org/core/4_7_1/core/org/apache/lucene/search/similarities/LMJelinekMercerSimilarity.html 
        /// </summary>
        /// <param name="name"></param>
        public LmJelinekMercerSimilarity(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultSimilarities.LmDirichlet;
        }

        /// <summary>
        ///     The optimal value depends on both the collection and the query. The optimal value is
        ///     around 0.1 for title queries and 0.7 for long queries. Default to 0.1.
        /// </summary>
        public double Lambda
        {
            get => _lambda;
            set
            {
                _lambda = value;
                _lambdaSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteValue("lambda", _lambda, elasticCrudJsonWriter, _lambdaSet);
        }
    }
}