using Newtonsoft.Json;

namespace Puppy.DataTable.Models
{
    public class PaginateModel
    {
        [JsonProperty(PropertyName = "sFirst")]
        public string sFirst { get; set; }

        [JsonProperty(PropertyName = "sPrevious")]
        public string sPrevious { get; set; }

        [JsonProperty(PropertyName = "sNext")]
        public string sNext { get; set; }

        [JsonProperty(PropertyName = "sLast")]
        public string sLast { get; set; }
    }
}