using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puppy.DataTable.Constants;
using Puppy.DataTable.Helpers;
using Puppy.DataTable.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Puppy.DataTable.Models
{
    public class DataTableConfigModel
    {
        public bool IsDevelopMode { get; set; } = false;

        public bool IsAutoWidthColumn { get; set; } = false;

        public bool IsResponsive { get; set; } = true;

        public bool IsEnableColVis { get; set; } = true;

        public bool IsShowPageSize { get; set; } = true;

        public bool IsShowGlobalSearchInput { get; set; } = true;

        public bool IsUseTableTools { get; set; } = true;

        public bool IsHideHeader { get; set; } = false;

        public bool IsUseColumnFilter { get; set; } = true;

        public bool IsStateSave { get; set; } = true;

        public string TableClass { get; set; } = "table table-hover dataTable table-striped";

        public string DrawCallback { get; set; }

        public string Id { get; set; }

        public string AjaxUrl { get; set; }

        public IEnumerable<ColDefModel> Columns { get; set; }

        public IDictionary<string, object> AdditionalOptions { get; } = new Dictionary<string, object>();

        public string ColumnDefsString => ConvertColumnDefsToJson(Columns.ToArray());

        public ColumnFilterSettingModel ColumnFilterVm { get; set; }

        public JToken SearchCols
        {
            get
            {
                var initialSearches = Columns
                    .Select(c => c.IsSearchable & c.SearchCols != null ? c.SearchCols : null as object).ToArray();
                return new JArray(initialSearches);
            }
        }

        private string _dom;

        public string Dom
        {
            get
            {
                if (!string.IsNullOrEmpty(_dom))
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

        public string ColumnSortingString => ConvertColumnSortingToJson(Columns);

        public string LanguageCode { get; set; }

        public LengthMenuVm LengthMenu { get; set; } = new LengthMenuVm
        {
            Tuple.Create("5", 5),
            Tuple.Create("10", 10),
            Tuple.Create("25", 25),
            Tuple.Create("50", 50),
            Tuple.Create("All", -1)
        };

        public int? PageSize { get; set; }

        public string GlobalJsVariableName { get; set; }

        //private bool _columnFilter;

        public string AjaxErrorHandler { get; set; }

        public DataTableConfigModel(string id, string ajaxUrl, IEnumerable<ColDefModel> columns)
        {
            AjaxUrl = ajaxUrl;
            Id = id;
            Columns = columns;
            ColumnFilterVm = new ColumnFilterSettingModel(this);
            AjaxErrorHandler =
                "function(jqXHR, textStatus, errorThrown)" +
                "{ " +
                    "console.log('error loading data: ' + textStatus + ' - ' + errorThrown); " +
                    "console.log(arguments);" +
                "}";
        }

        public class _FilterOn<TTarget>
        {
            private readonly TTarget _target;

            private readonly ColDefModel _colDefModel;

            public _FilterOn(TTarget target, ColDefModel colDefModel)
            {
                _target = target;
                _colDefModel = colDefModel;
            }

            public TTarget Select(params string[] options)
            {
                _colDefModel.Filter.type = "select";
                _colDefModel.Filter.values = options.Cast<object>().ToArray();
                if (_colDefModel.Type.GetTypeInfo().IsEnum)
                {
                    _colDefModel.Filter.values = _colDefModel.Type.EnumValLabPairs();
                }
                return _target;
            }

            public TTarget NumberRange()
            {
                _colDefModel.Filter.type = "number-range";
                return _target;
            }

            public TTarget DateRange()
            {
                _colDefModel.Filter.type = "date-range";
                return _target;
            }

            public TTarget Number()
            {
                _colDefModel.Filter.type = "number";
                return _target;
            }

            public TTarget CheckBoxes(params string[] options)
            {
                _colDefModel.Filter.type = "checkbox";
                _colDefModel.Filter.values = options.Cast<object>().ToArray();
                if (_colDefModel.Type.GetTypeInfo().IsEnum)
                {
                    _colDefModel.Filter.values = _colDefModel.Type.EnumValLabPairs();
                }
                return _target;
            }

            public TTarget Text()
            {
                _colDefModel.Filter.type = "text";
                return _target;
            }

            public TTarget None()
            {
                _colDefModel.Filter = null;
                return _target;
            }
        }

        public _FilterOn<DataTableConfigModel> FilterOn<T>()
        {
            return FilterOn<T>(null);
        }

        public _FilterOn<DataTableConfigModel> FilterOn<T>(object jsOptions)
        {
            IDictionary<string, object> optionsDict = ConvertObjectToDictionary(jsOptions);
            return FilterOn<T>(optionsDict);
        }

        ////public _FilterOn<DataTableConfigVm> FilterOn<T>(IDictionary<string, object> filterOptions)
        ////{
        ////    return new _FilterOn<DataTableConfigVm>(this, this.FilterTypeRules, (c, t) => t == typeof(T), filterOptions);
        ////}
        public _FilterOn<DataTableConfigModel> FilterOn(string columnName)
        {
            return FilterOn(columnName, null);
        }

        public _FilterOn<DataTableConfigModel> FilterOn(string columnName, object jsOptions)
        {
            IDictionary<string, object> optionsDict = ConvertObjectToDictionary(jsOptions);
            return FilterOn(columnName, optionsDict);
        }

        public _FilterOn<DataTableConfigModel> FilterOn(string columnName, object jsOptions, object jsInitialSearchCols)
        {
            IDictionary<string, object> optionsDict = ConvertObjectToDictionary(jsOptions);
            IDictionary<string, object> initialSearchColsDict = ConvertObjectToDictionary(jsInitialSearchCols);
            return FilterOn(columnName, optionsDict, initialSearchColsDict);
        }

        public _FilterOn<DataTableConfigModel> FilterOn(string columnName, IDictionary<string, object> filterOptions)
        {
            return FilterOn(columnName, filterOptions, null);
        }

        public _FilterOn<DataTableConfigModel> FilterOn(string columnName, IDictionary<string, object> filterOptions, IDictionary<string, object> jsInitialSearchCols)
        {
            var colDef = this.Columns.Single(c => c.Name == columnName);
            if (filterOptions != null)
            {
                foreach (var jsOption in filterOptions)
                {
                    colDef.Filter[jsOption.Key] = jsOption.Value;
                }
            }
            if (jsInitialSearchCols != null)
            {
                colDef.SearchCols = new JObject();
                foreach (var jsInitialSearchCol in jsInitialSearchCols)
                {
                    colDef.SearchCols[jsInitialSearchCol.Key] = new JValue(jsInitialSearchCol.Value);
                }
            }
            return new _FilterOn<DataTableConfigModel>(this, colDef);
        }

        private static string ConvertDictionaryToJsonBody(IDictionary<string, object> dict)
        {
            // Converting to System.Collections.Generic.Dictionary<> to ensure Dictionary will be
            // converted to Json in correct format
            var dictSystem = new Dictionary<string, object>(dict);
            var json = JsonConvert.SerializeObject((object)dictSystem, Formatting.None, new RawConverter());
            return json.Substring(1, json.Length - 2);
        }

        private static string ConvertColumnDefsToJson(ColDefModel[] columns)
        {
            Func<bool, bool> isFalse = x => x == false;
            Func<string, bool> isNonEmptyString = x => !string.IsNullOrEmpty(x);

            var defs = new List<dynamic>();

            defs.AddRange(ConvertColumnDefsToTargetedProperty(
                jsonPropertyName: "bSortable",
                propertySelector: column => column.IsSortable,
                propertyPredicate: isFalse,
                columns: columns));
            defs.AddRange(ConvertColumnDefsToTargetedProperty(
                jsonPropertyName: "bVisible",
                propertySelector: column => column.IsVisible,
                propertyPredicate: isFalse,
                columns: columns));
            defs.AddRange(ConvertColumnDefsToTargetedProperty(
                jsonPropertyName: "bSearchable",
                propertySelector: column => column.IsSearchable,
                propertyPredicate: isFalse,
                columns: columns));
            defs.AddRange(ConvertColumnDefsToTargetedProperty(
                jsonPropertyName: "mRender",
                propertySelector: column => column.MRenderFunction,
                propertyConverter: x => new JRaw(x),
                propertyPredicate: isNonEmptyString,
                columns: columns));
            defs.AddRange(ConvertColumnDefsToTargetedProperty(
                jsonPropertyName: "className",
                propertySelector: column => column.CssClass,
                propertyPredicate: isNonEmptyString,
                columns: columns));
            defs.AddRange(ConvertColumnDefsToTargetedProperty(
                jsonPropertyName: "width",
                propertySelector: column => column.Width,
                propertyPredicate: isNonEmptyString,
                columns: columns));

            if (defs.Count > 0)
                return JsonConvert.SerializeObject(defs);

            return "[]";
        }

        private static string ConvertColumnSortingToJson(IEnumerable<ColDefModel> columns)
        {
            var sortList = columns.Select((c, idx) => c.SortDirection == SortDirection.None ? new dynamic[] { -1, "" } : (c.SortDirection == SortDirection.Ascending ? new dynamic[] { idx, "asc" } : new dynamic[] { idx, "desc" })).Where(x => x[0] > -1).ToArray();

            if (sortList.Length > 0)
                return JsonConvert.SerializeObject(sortList);

            return "[]";
        }

        private static IDictionary<string, object> ConvertObjectToDictionary(object obj)
        {
            var d = new Dictionary<string, object>();
            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                d[propertyInfo.Name] = propertyInfo.GetValue(obj);
            }
            return d;
        }

        private static IEnumerable<JObject> ConvertColumnDefsToTargetedProperty<TProperty>(
            string jsonPropertyName,
            Func<ColDefModel, TProperty> propertySelector,
            Func<TProperty, bool> propertyPredicate,
            IEnumerable<ColDefModel> columns)
        {
            return ConvertColumnDefsToTargetedProperty(
                jsonPropertyName,
                propertySelector,
                propertyPredicate,
                x => x,
                columns);
        }

        private static IEnumerable<JObject> ConvertColumnDefsToTargetedProperty<TProperty, TResult>(
            string jsonPropertyName,
            Func<ColDefModel, TProperty> propertySelector,
            Func<TProperty, bool> propertyPredicate,
            Func<TProperty, TResult> propertyConverter,
            IEnumerable<ColDefModel> columns)
        {
            return columns
                .Select((x, idx) => new { rawPropertyValue = propertySelector(x), idx })
                .Where(x => propertyPredicate(x.rawPropertyValue))
                .GroupBy(
                    x => x.rawPropertyValue,
                    (rawPropertyValue, groupedItems) => new
                    {
                        rawPropertyValue,
                        indices = groupedItems.Select(x => x.idx)
                    })
                .Select(x => new JObject(
                    new JProperty(jsonPropertyName, propertyConverter(x.rawPropertyValue)),
                    new JProperty("aTargets", new JArray(x.indices))
                    ));
        }
    }
}