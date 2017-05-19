using TopCore.Framework.Search.Elastic.Model;
using TopCore.Framework.Search.Elastic.Utils;

namespace TopCore.Framework.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
    public class EdgeNGramTokenFilter : AnalysisFilterBase
    {
        private int _maxGram;
        private bool _maxGramSet;
        private int _minGram;
        private bool _minGramSet;
        private Side _side;
        private bool _sideSet;

        public EdgeNGramTokenFilter(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenFilters.EdgeNGram;
        }

        /// <summary>
        ///   min_gram Minimum size in codepoints of a single n-gram 
        /// </summary>
        public int MinGram
        {
            get => _minGram;
            set
            {
                _minGram = value;
                _minGramSet = true;
            }
        }

        /// <summary>
        ///   max_gram Maximum size in codepoints of a single n-gram 
        /// </summary>
        public int MaxGram
        {
            get => _maxGram;
            set
            {
                _maxGram = value;
                _maxGramSet = true;
            }
        }

        /// <summary>
        ///   Either front or back. Defaults to front. 
        /// </summary>
        public Side Side
        {
            get => _side;
            set
            {
                _side = value;
                _sideSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteValue("min_gram", _minGram, elasticCrudJsonWriter, _minGramSet);
            JsonHelper.WriteValue("max_gram", _maxGram, elasticCrudJsonWriter, _maxGramSet);
            JsonHelper.WriteValue("side", _side.ToString(), elasticCrudJsonWriter, _sideSet);
        }
    }

    public enum Side
    {
        front,
        back
    }
}