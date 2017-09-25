using Newtonsoft.Json;

namespace Puppy.DataTable.Models
{
    public class LanguageModel
    {
        [JsonProperty(PropertyName = "sProcessing")]
        public string sProcessing { get; set; }

        [JsonProperty(PropertyName = "sLengthMenu")]
        public string sLengthMenu { get; set; }

        [JsonProperty(PropertyName = "sZeroRecords")]
        public string sZeroRecords { get; set; }

        [JsonProperty(PropertyName = "sInfo")]
        public string sInfo { get; set; }

        [JsonProperty(PropertyName = "sInfoEmpty")]
        public string sInfoEmpty { get; set; }

        [JsonProperty(PropertyName = "sInfoFiltered")]
        public string sInfoFiltered { get; set; }

        [JsonProperty(PropertyName = "sInfoPostFix")]
        public string sInfoPostFix { get; set; }

        [JsonProperty(PropertyName = "sSearch")]
        public string sSearch { get; set; }

        [JsonProperty(PropertyName = "sUrl")]
        public string sUrl { get; set; }

        [JsonProperty(PropertyName = "oPaginate")]
        public PaginateModel oPaginate { get; set; }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}