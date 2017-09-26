using Newtonsoft.Json;
using Puppy.DataTable.Constants;

namespace Puppy.DataTable.Models.Config.Language
{
    public class PaginateModel
    {
        [JsonProperty(PropertyName = PropertyConst.First)]
        public string First { get; set; }

        [JsonProperty(PropertyName = PropertyConst.Previous)]
        public string Previous { get; set; }

        [JsonProperty(PropertyName = PropertyConst.Next)]
        public string Next { get; set; }

        [JsonProperty(PropertyName = PropertyConst.Last)]
        public string Last { get; set; }
    }
}