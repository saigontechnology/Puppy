using Newtonsoft.Json;
using System.Collections.Generic;

namespace Puppy.DataTable.Models
{
    public class DataTableParamModel
    {
        [JsonProperty(PropertyName = "iDisplayStart")]
        public int iDisplayStart { get; set; }

        [JsonProperty(PropertyName = "iDisplayLength")]
        public int iDisplayLength { get; set; }

        [JsonProperty(PropertyName = "iColumns")]
        public int iColumns { get; set; }

        [JsonProperty(PropertyName = "sSearch")]
        public string sSearch { get; set; }

        [JsonProperty(PropertyName = "bEscapeRegex")]
        public bool bEscapeRegex { get; set; }

        [JsonProperty(PropertyName = "iSortingCols")]
        public int iSortingCols { get; set; }

        [JsonProperty(PropertyName = "sEcho")]
        public int sEcho { get; set; }

        [JsonProperty(PropertyName = "sColumnNames")]
        public List<string> sColumnNames { get; set; }

        [JsonProperty(PropertyName = "bSortable")]
        public List<bool> bSortable { get; set; }

        [JsonProperty(PropertyName = "bSearchable")]
        public List<bool> bSearchable { get; set; }

        [JsonProperty(PropertyName = "sSearchValues")]
        public List<string> sSearchValues { get; set; }

        [JsonProperty(PropertyName = "iSortCol")]
        public List<int> iSortCol { get; set; }

        [JsonProperty(PropertyName = "sSortDir")]
        public List<string> sSortDir { get; set; }

        [JsonProperty(PropertyName = "bEscapeRegexColumns")]
        public List<bool> bEscapeRegexColumns { get; set; }

        public DataTableParamModel()
        {
            sColumnNames = new List<string>();
            bSortable = new List<bool>();
            bSearchable = new List<bool>();
            sSearchValues = new List<string>();
            iSortCol = new List<int>();
            sSortDir = new List<string>();
            bEscapeRegexColumns = new List<bool>();
        }

        public DataTableParamModel(int iColumns)
        {
            this.iColumns = iColumns;
            sColumnNames = new List<string>(iColumns);
            bSortable = new List<bool>(iColumns);
            bSearchable = new List<bool>(iColumns);
            sSearchValues = new List<string>(iColumns);
            iSortCol = new List<int>(iColumns);
            sSortDir = new List<string>(iColumns);
            bEscapeRegexColumns = new List<bool>(iColumns);
        }
    }
}