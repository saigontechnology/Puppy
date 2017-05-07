using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TopCore.Framework.Search.Elastic.Model
{
	public class GetResult
    {
        [JsonExtensionData]
        public Dictionary<string, JToken> Fields { get; set; }
    }
}