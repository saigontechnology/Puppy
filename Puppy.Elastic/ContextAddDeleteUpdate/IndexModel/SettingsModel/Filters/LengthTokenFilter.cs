using Puppy.Elastic.Model;
using Puppy.Elastic.Utils;

namespace Puppy.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel.Filters
{
    public class LengthTokenFilter : AnalysisFilterBase
    {
        private int _max;
        private bool _maxSet;
        private int _min;
        private bool _minSet;

        /// <summary>
        ///     A token filter of type length that removes words that are too long or too short for
        ///     the stream.
        /// </summary>
        /// <param name="name"> name for the custom filter </param>
        public LengthTokenFilter(string name)
        {
            AnalyzerSet = true;
            Name = name.ToLower();
            Type = DefaultTokenFilters.Length;
        }

        public int Min
        {
            get => _min;
            set
            {
                _min = value;
                _minSet = true;
            }
        }

        public int Max
        {
            get => _max;
            set
            {
                _max = value;
                _maxSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteValue("type", Type, elasticCrudJsonWriter);
            JsonHelper.WriteValue("min", _min, elasticCrudJsonWriter, _minSet);
            JsonHelper.WriteValue("max", _min, elasticCrudJsonWriter, _maxSet);
        }
    }
}