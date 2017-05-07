using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.SimilarityCustom
{
	public class IbSimilarity : SimilarityBase
    {
        private IbDistribution _distribution;
        private bool _distributionSet;
        private IbLambda _lambda;
        private bool _lambdaSet;

        private DfrIbNormalization _normalization;
        private bool _normalizationSet;

	    /// <summary>
	    ///     nformation based model http://lucene.apache.org/core/4_1_0/core/org/apache/lucene/search/similarities/IBSimilarity.html 
	    /// </summary>
	    /// <param name="name"></param>
	    public IbSimilarity(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultSimilarities.Ib;
        }

	    /// <summary>
	    ///     distribution Possible values: ll and spl. 
	    /// </summary>
	    public IbDistribution Distribution
        {
            get => _distribution;
            set
            {
                _distribution = value;
                _distributionSet = true;
            }
        }

	    /// <summary>
	    ///     lambda Possible values: df and ttf. 
	    /// </summary>
	    public IbLambda Lambda
        {
            get => _lambda;
            set
            {
                _lambda = value;
                _lambdaSet = true;
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
            JsonHelper.WriteValue("distribution", _distribution.ToString(), elasticCrudJsonWriter, _distributionSet);
            JsonHelper.WriteValue("lambda", _lambda.ToString(), elasticCrudJsonWriter, _lambdaSet);
            JsonHelper.WriteValue("normalization", _normalization.ToString(), elasticCrudJsonWriter, _normalizationSet);
        }
    }

    public enum IbLambda
    {
        df,
        ttf
    }

    public enum IbDistribution
    {
        ll,
        spl
    }
}