#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey </Project>
//     <File>
//         <Name> PagedCollectionResultModel.cs </Name>
//         <Created> 20/09/17 10:40:19 AM </Created>
//         <Key> 29d46047-1508-4814-8c3f-9121707ea7f7 </Key>
//     </File>
//     <Summary>
//         PagedCollectionResultModel.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Puppy.Web.Models.Api
{
    [Serializable]
    [KnownType(typeof(PagedCollectionResultModel<>))]
    public class PagedCollectionResultModel<T>
    {
        public int Skip { get; set; }

        public int Take { get; set; }

        public string Terms { get; set; }

        public long Total { get; set; }

        public List<T> Items { get; set; } = new List<T>();

        [JsonExtensionData]
        public Dictionary<string, object> AdditionalData { get; set; } = new Dictionary<string, object>();
    }
}