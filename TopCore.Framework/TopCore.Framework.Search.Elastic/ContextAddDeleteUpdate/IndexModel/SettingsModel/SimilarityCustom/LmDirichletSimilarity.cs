using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.SimilarityCustom
{
	public class LmDirichletSimilarity : SimilarityBase
    {
        private int _mu;
        private bool _muSet;

	    /// <summary>
	    ///     LM Jelinek Mercer similarity http://lucene.apache.org/core/4_7_1/core/org/apache/lucene/search/similarities/LMJelinekMercerSimilarity.html 
	    /// </summary>
	    /// <param name="name"></param>
	    public LmDirichletSimilarity(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultSimilarities.LmDirichlet;
        }

        public int Mu
        {
            get => _mu;
            set
            {
                _mu = value;
                _muSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteValue("mu", _mu, elasticCrudJsonWriter, _muSet);
        }
    }
}