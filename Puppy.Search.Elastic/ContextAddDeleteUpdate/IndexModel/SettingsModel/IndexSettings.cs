using Puppy.Search.Elastic.Utils;

namespace Puppy.Search.Elastic.ContextAddDeleteUpdate.IndexModel.SettingsModel
{
    public class IndexSettings : IndexUpdateSettings
    {
        private int _numberOfShards;
        private bool _numberOfShardsSet;

        public int NumberOfShards
        {
            get => _numberOfShards;
            set
            {
                _numberOfShards = value;
                _numberOfShardsSet = true;
            }
        }

        public override void WriteJson(ElasticJsonWriter elasticCrudJsonWriter)
        {
            Similarities.WriteJson(elasticCrudJsonWriter);
            Analysis.WriteJson(elasticCrudJsonWriter);
            JsonHelper.WriteValue("number_of_shards", _numberOfShards, elasticCrudJsonWriter, _numberOfShardsSet);
            base.WriteJson(elasticCrudJsonWriter);
        }
    }
}