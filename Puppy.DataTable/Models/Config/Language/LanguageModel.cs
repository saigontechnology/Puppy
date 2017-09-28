using Newtonsoft.Json;
using Puppy.DataTable.Constants;

namespace Puppy.DataTable.Models.Config.Language
{
    public class LanguageModel
    {
        [JsonProperty(PropertyName = PropertyConst.Processing)]
        public string Processing { get; set; }

        [JsonProperty(PropertyName = PropertyConst.LengthMenu)]
        public string LengthMenu { get; set; }

        [JsonProperty(PropertyName = PropertyConst.ZeroRecords)]
        public string ZeroRecord { get; set; }

        [JsonProperty(PropertyName = PropertyConst.Info)]
        public string Info { get; set; }

        [JsonProperty(PropertyName = PropertyConst.InfoEmpty)]
        public string InfoEmpty { get; set; }

        [JsonProperty(PropertyName = PropertyConst.InfoFiltered)]
        public string InfoFiltered { get; set; }

        [JsonProperty(PropertyName = PropertyConst.InfoPostFix)]
        public string InfoPostFix { get; set; }

        [JsonProperty(PropertyName = PropertyConst.Search)]
        public string Search { get; set; }

        [JsonProperty(PropertyName = PropertyConst.Url)]
        public string Url { get; set; }

        [JsonProperty(PropertyName = PropertyConst.Paginate)]
        public PaginateModel Paginate { get; set; }
    }
}