using Newtonsoft.Json;
using Puppy.DataTable.Helpers;
using Puppy.DataTable.Helpers.Reflection;
using Puppy.DataTable.Models;
using System.Collections.Generic;
using System.Linq;

namespace Puppy.DataTable
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

        public DataTablesResponseDataModel GetDataTablesResponse<TSource>(IQueryable<TSource> data)
        {
            var totalRecords = data.Count(); // annoying this, as it causes an extra evaluation..

            var filters = new DataTablesFiltering();

            var outputProperties = DataTablesTypeInfo<TSource>.Properties;

            var filteredData = filters.ApplyFiltersAndSort(this, data, outputProperties);
            var totalDisplayRecords = filteredData.Count();

            var skipped = filteredData.Skip(this.iDisplayStart);
            var page = (this.iDisplayLength <= 0 ? skipped : skipped.Take(this.iDisplayLength)).ToArray();

            var result = new DataTablesResponseDataModel()
            {
                iTotalRecords = totalRecords,
                iTotalDisplayRecords = totalDisplayRecords,
                sEcho = this.sEcho,
                aaData = page.Cast<object>().ToArray(),
            };

            return result;
        }
    }
}