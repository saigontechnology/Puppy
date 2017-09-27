using Puppy.DataTable.Models.Config.Column;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puppy.DataTable.Models.Config
{
    public class DataTableModel
    {
        public bool IsDevelopMode { get; set; } = false;

        public bool IsAutoWidthColumn { get; set; } = false;

        public bool IsResponsive { get; set; } = true;

        public bool IsEnableColVis { get; set; } = true;

        public bool IsShowPageSize { get; set; } = true;

        public bool IsShowGlobalSearchInput { get; set; } = true;

        public bool IsUseTableTools { get; set; } = true;

        public bool IsHideHeader { get; set; } = false;

        public bool IsUseColumnFilter { get; set; } = false;

        public bool IsStateSave { get; set; } = true;

        public string TableClass { get; set; } = "table table-hover dataTable table-striped";

        public string Id { get; set; }

        public string AjaxUrl { get; set; }

        public List<ColumnModel> Columns { get; set; }

        public IDictionary<string, object> AdditionalOptions { get; } = new Dictionary<string, object>();

        public ColumnFilterModel ColumnFilter { get; set; }

        private string _dom;

        public string Dom
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(_dom))
                    return _dom;

                string str = "<\"dt-panelmenu clearfix\"";
                if (IsShowPageSize)
                    str += "l";
                if (IsShowGlobalSearchInput)
                    str += "f";
                if (IsUseTableTools)
                    str += "B";
                return str + ">t<\"dt-panelfooter clearfix\"rip>";
            }

            set => _dom = value;
        }

        public string LanguageCode { get; set; }

        public LengthMenuModel LengthMenu { get; set; } = new LengthMenuModel
        {
            Tuple.Create("5", 5),
            Tuple.Create("10", 10),
            Tuple.Create("25", 25),
            Tuple.Create("50", 50),
            Tuple.Create("All", -1)
        };

        public int? PageSize { get; set; }

        public string GlobalJsVariableName { get; set; }

        /// <summary>
        ///     Your javascript function as string with params: jqXHR, textStatus, errorThrown. Ex:
        ///     "function(jqXHR, textStatus, errorThrown){console.log(textStatus)}"
        /// </summary>
        public string AjaxErrorHandler { get; set; }

        /// <summary>
        ///     Function name of Draw Call Back. DataTable will pass "setting" to the function. Ex: drawCallBackHandle(oSettings). 
        /// </summary>
        public string DrawCallbackFunctionName { get; set; }

        /// <summary>
        ///     Function name of before send handler, you can modified data before submit by this
        ///     way. DataTable will pass "list key-value" submit to server as params to the function.
        ///     Ex: beforeSendHandle(aoData).
        /// </summary>
        public string BeforeSendFunctionName { get; set; }

        public DataTableModel(string id, string ajaxUrl, params ColumnModel[] columns)
        {
            AjaxUrl = ajaxUrl;
            Id = id;
            Columns = columns.ToList();
            ColumnFilter = new ColumnFilterModel(this);
            AjaxErrorHandler =
                "function(jqXHR, textStatus, errorThrown)" +
                "{ " +
                    "console.log('error loading data: ' + textStatus + ' - ' + errorThrown); " +
                    "console.log(arguments);" +
                "}";
        }
    }
}