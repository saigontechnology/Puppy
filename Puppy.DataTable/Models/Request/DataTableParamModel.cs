using Newtonsoft.Json;
using Puppy.DataTable.Constants;
using System.Collections.Generic;

namespace Puppy.DataTable.Models.Request
{
    public class DataTableParamModel
    {
        [JsonProperty(PropertyName = PropertyConst.DisplayStart)]
        public int DisplayStart { get; set; }

        [JsonProperty(PropertyName = PropertyConst.DisplayLength)]
        public int DisplayLength { get; set; }

        [JsonProperty(PropertyName = PropertyConst.Columns)]
        public int Columns { get; set; }

        [JsonProperty(PropertyName = PropertyConst.Search)]
        public string Search { get; set; }

        [JsonProperty(PropertyName = PropertyConst.EscapeRegex)]
        public bool EscapeRegex { get; set; }

        [JsonProperty(PropertyName = PropertyConst.SortingCols)]
        public int SortingCols { get; set; }

        [JsonProperty(PropertyName = PropertyConst.Echo)]
        public int Echo { get; set; }

        [JsonProperty(PropertyName = PropertyConst.ColumnNames)]
        public List<string> ColumnNames { get; set; }

        [JsonProperty(PropertyName = PropertyConst.Sortable)]
        public List<bool> ListIsSortable { get; set; }

        [JsonProperty(PropertyName = PropertyConst.Searchable)]
        public List<bool> ListIsSearchable { get; set; }

        [JsonProperty(PropertyName = PropertyConst.SearchValues)]
        public List<string> SearchValues { get; set; }

        [JsonProperty(PropertyName = PropertyConst.SortCol)]
        public List<int> SortCol { get; set; }

        [JsonProperty(PropertyName = PropertyConst.SortDir)]
        public List<string> SortDir { get; set; }

        [JsonProperty(PropertyName = PropertyConst.EscapeRegexColumns)]
        public List<bool> ListIsEscapeRegexColumn { get; set; }

        /// <summary>
        ///     Store all information by key/name-value from client side
        /// </summary>
        [JsonIgnore]
        public Dictionary<string, object> Data { get; set; }

        public DataTableParamModel()
        {
            ColumnNames = new List<string>();
            ListIsSortable = new List<bool>();
            ListIsSearchable = new List<bool>();
            SearchValues = new List<string>();
            SortCol = new List<int>();
            SortDir = new List<string>();
            ListIsEscapeRegexColumn = new List<bool>();
        }

        public DataTableParamModel(int columns)
        {
            Columns = columns;
            ColumnNames = new List<string>(columns);
            ListIsSortable = new List<bool>(columns);
            ListIsSearchable = new List<bool>(columns);
            SearchValues = new List<string>(columns);
            SortCol = new List<int>(columns);
            SortDir = new List<string>(columns);
            ListIsEscapeRegexColumn = new List<bool>(columns);
        }
    }
}