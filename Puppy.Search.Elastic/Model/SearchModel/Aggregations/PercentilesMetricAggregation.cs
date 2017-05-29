using System.Collections.Generic;
using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.Model.SearchModel.Aggregations
{
    public class PercentilesMetricAggregation : BaseMetricAggregation
    {
        private uint _compression;
        private bool _compressionSet;
        private List<double> _percents;
        private bool _percentsSet;

        public PercentilesMetricAggregation(string name, string field) : base("percentiles", name, field)
        {
        }

        public List<double> Percents
        {
            get => _percents;
            set
            {
                _percents = value;
                _percentsSet = true;
            }
        }

        public uint Compression
        {
            get => _compression;
            set
            {
                _compression = value;
                _compressionSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            WriteJsonBase(elasticCrudJsonWriter, WriteValues);
        }

        private void WriteValues(ElasticJsonWriter elasticCrudJsonWriter)
        {
            JsonHelper.WriteListValue("percents", _percents, elasticCrudJsonWriter, _percentsSet);
            JsonHelper.WriteValue("compression", _compression, elasticCrudJsonWriter, _compressionSet);
        }
    }
}