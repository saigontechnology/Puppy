using Newtonsoft.Json;
using Puppy.DataTable.Constants;
using System;
using System.Linq;

namespace Puppy.DataTable.Models.Response
{
    public class DataTableResponseDataModel<T>
    {
        [JsonProperty(PropertyName = PropertyConst.TotalRecords)]
        public int TotalRecord { get; set; }

        [JsonProperty(PropertyName = PropertyConst.TotalDisplayRecords)]
        public int TotalDisplayRecord { get; set; }

        [JsonProperty(PropertyName = PropertyConst.Echo)]
        public int Echo { get; set; }

        [JsonProperty(PropertyName = PropertyConst.Data)]
        public object[] Data { get; set; }

        public Type DataType { get; } = typeof(T);

        public DataTableResponseDataModel<T> Transform<TData, TTransform>(Func<TData, TTransform> transformRow, ResponseOptionModel responseOptions = null)
        {
            var data = new DataTableResponseDataModel<T>
            {
                Data = Data.Cast<TData>().Select(transformRow).Cast<object>().ToArray(),
                TotalDisplayRecord = TotalDisplayRecord,
                TotalRecord = TotalRecord,
                Echo = Echo
            };
            return data;
        }
    }
}