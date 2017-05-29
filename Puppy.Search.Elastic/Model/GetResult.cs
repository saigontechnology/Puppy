using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Puppy.Search.Elastic.Model
{
    public class GetResult
    {
        [JsonExtensionData]
        public Dictionary<string, JToken> Fields { get; set; }
    }
}