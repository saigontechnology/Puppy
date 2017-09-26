using Newtonsoft.Json;
using System;
using System.Linq;

namespace Puppy.DataTable.Models
{
    public class DataTableResponseDataModel
    {
        [JsonProperty(PropertyName = "iTotalRecords")]
        public int iTotalRecords { get; set; }

        [JsonProperty(PropertyName = "iTotalDisplayRecords")]
        public int iTotalDisplayRecords { get; set; }

        [JsonProperty(PropertyName = "sEcho")]
        public int sEcho { get; set; }

        [JsonProperty(PropertyName = "aaData")]
        public object[] aaData { get; set; }

        public DataTableResponseDataModel Transform<TData, TTransform>(Func<TData, TTransform> transformRow, ResponseOptionModel responseOptions = null)
        {
            var data = new DataTableResponseDataModel
            {
                aaData = aaData.Cast<TData>().Select(transformRow).Cast<object>().ToArray(),
                iTotalDisplayRecords = iTotalDisplayRecords,
                iTotalRecords = iTotalRecords,
                sEcho = sEcho
            };
            return data;
        }
    }
}