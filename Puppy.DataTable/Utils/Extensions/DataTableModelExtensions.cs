#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> DataTableModelExtensions.cs </Name>
//         <Created> 27/09/17 12:21:08 AM </Created>
//         <Key> aea223c9-a50e-4a61-aafd-7c91065a8df8 </Key>
//     </File>
//     <Summary>
//         DataTableModelExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puppy.Core.ObjectUtils;
using Puppy.DataTable.Constants;
using Puppy.DataTable.Models.Config;
using Puppy.DataTable.Models.Config.Column;
using System;
using System.Collections.Generic;
using System.Linq;
using Puppy.DataTable.Processing.Response;

namespace Puppy.DataTable.Utils.Extensions
{
    public static class DataTableModelExtensions
    {
        public static string GetColumnsJsonString(this DataTableModel model)
        {
            return ConvertColumnsToJson(model.Columns.ToArray());
        }

        public static string GetColumnSortingString(this DataTableModel model)
        {
            return ConvertColumnSortingToJson(model.Columns.ToArray());
        }

        public static JToken GetSearchColumns(this DataTableModel model)
        {
            var initialSearches = model.Columns.Select(c => c.IsSearchable & c.SearchColumns != null ? c.SearchColumns : null as object).ToArray();
            return new JArray(initialSearches);
        }

        public static ColumnFilter<DataTableModel> FilterOn<T>(this DataTableModel model)
        {
            return model.FilterOn<T>(null);
        }

        public static ColumnFilter<DataTableModel> FilterOn<T>(this DataTableModel model, object jsOptions)
        {
            IDictionary<string, object> optionsDict = jsOptions.ToDictionary();
            return model.FilterOn<T>(optionsDict);
        }

        public static ColumnFilter<DataTableModel> FilterOn(this DataTableModel model, string columnName)
        {
            return model.FilterOn(columnName, null);
        }

        public static ColumnFilter<DataTableModel> FilterOn(this DataTableModel model, string columnName, object jsOptions)
        {
            IDictionary<string, object> optionsDict = jsOptions.ToDictionary();
            return model.FilterOn(columnName, optionsDict);
        }

        public static ColumnFilter<DataTableModel> FilterOn(this DataTableModel model, string columnName, object jsOptions, object jsInitialSearchColumns)
        {
            IDictionary<string, object> optionsDict = jsOptions.ToDictionary();
            IDictionary<string, object> initialSearchColsDict = jsInitialSearchColumns.ToDictionary();
            return model.FilterOn(columnName, optionsDict, initialSearchColsDict);
        }

        public static ColumnFilter<DataTableModel> FilterOn(this DataTableModel model, string columnName, IDictionary<string, object> filterOptions)
        {
            return model.FilterOn(columnName, filterOptions, null);
        }

        public static ColumnFilter<DataTableModel> FilterOn(this DataTableModel model, string columnName, IDictionary<string, object> filterOptions, IDictionary<string, object> jsInitialSearchColumns)
        {
            var columns = model.Columns.Single(c => c.Name == columnName);

            if (filterOptions != null)
            {
                foreach (var jsOption in filterOptions)
                {
                    columns.ColumnFilter[jsOption.Key] = jsOption.Value;
                }
            }

            if (jsInitialSearchColumns != null)
            {
                columns.SearchColumns = new JObject();
                foreach (var jsInitialSearchCol in jsInitialSearchColumns)
                {
                    columns.SearchColumns[jsInitialSearchCol.Key] = new JValue(jsInitialSearchCol.Value);
                }
            }
            return new ColumnFilter<DataTableModel>(model, columns);
        }

        private static string ConvertColumnsToJson(params ColumnModel[] columns)
        {
            bool IsFalse(bool x) => x == false;

            bool IsNonEmptyString(string x) => !string.IsNullOrEmpty(x);

            var columnsJsonObject = new List<dynamic>();

            columnsJsonObject.AddRange(ConvertColumnToTargetedProperty(
                jsonPropertyName: PropertyConst.Sortable,
                propertySelector: column => column.IsSortable,
                propertyPredicate: IsFalse,
                columns: columns));

            columnsJsonObject.AddRange(ConvertColumnToTargetedProperty(
                jsonPropertyName: PropertyConst.Visible,
                propertySelector: column => column.IsVisible,
                propertyPredicate: IsFalse,
                columns: columns));

            columnsJsonObject.AddRange(ConvertColumnToTargetedProperty(
                jsonPropertyName: PropertyConst.Searchable,
                propertySelector: column => column.IsSearchable,
                propertyPredicate: IsFalse,
                columns: columns));

            columnsJsonObject.AddRange(ConvertColumnToTargetedProperty(
                jsonPropertyName: PropertyConst.Render,
                propertySelector: column => column.MRenderFunction,
                propertyConverter: x => new JRaw(x),
                propertyPredicate: IsNonEmptyString,
                columns: columns));

            columnsJsonObject.AddRange(ConvertColumnToTargetedProperty(
                jsonPropertyName: PropertyConst.ClassName,
                propertySelector: column => column.CssClass,
                propertyPredicate: IsNonEmptyString,
                columns: columns));

            columnsJsonObject.AddRange(ConvertColumnToTargetedProperty(
                jsonPropertyName: PropertyConst.Width,
                propertySelector: column => column.Width,
                propertyPredicate: IsNonEmptyString,
                columns: columns));

            return columnsJsonObject.Any()
                ? JsonConvert.SerializeObject(columnsJsonObject)
                : DataConst.EmptyArray;
        }

        private static string ConvertColumnSortingToJson(params ColumnModel[] columns)
        {
            var sortList = columns
                .Select((c, idx) => c.SortDirection == SortDirection.None
                    ? new dynamic[] { -1, string.Empty }
                    : (c.SortDirection == SortDirection.Ascending
                        ? new dynamic[] { idx, SortDirectionConst.Ascending }
                        : new dynamic[] { idx, SortDirectionConst.Descending })).Where(x => x[0] > -1).ToArray();

            return sortList.Any()
                ? JsonConvert.SerializeObject(sortList)
                : DataConst.EmptyArray;
        }

        private static IEnumerable<JObject> ConvertColumnToTargetedProperty<TProperty>(string jsonPropertyName,
                                                                                       Func<ColumnModel, TProperty> propertySelector,
                                                                                       Func<TProperty, bool> propertyPredicate,
                                                                                       params ColumnModel[] columns)
        {
            return ConvertColumnToTargetedProperty(jsonPropertyName, propertySelector, propertyPredicate, x => x, columns);
        }

        private static IEnumerable<JObject> ConvertColumnToTargetedProperty<TProperty, TResult>(string jsonPropertyName,
                                                                                                Func<ColumnModel, TProperty> propertySelector,
                                                                                                Func<TProperty, bool> propertyPredicate,
                                                                                                Func<TProperty, TResult> propertyConverter,
                                                                                                params ColumnModel[] columns)
        {
            return
                columns
                    .Select((x, idx) => new { rawPropertyValue = propertySelector(x), idx })
                    .Where(x => propertyPredicate(x.rawPropertyValue))
                    .GroupBy(
                        x => x.rawPropertyValue,
                        (rawPropertyValue, groupedItems) =>
                            new
                            {
                                rawPropertyValue,
                                indices = groupedItems.Select(x => x.idx)
                            })
                    .Select(x => new JObject(
                        new JProperty(jsonPropertyName, propertyConverter(x.rawPropertyValue)),
                        new JProperty(PropertyConst.Targets, new JArray(x.indices))
                    ));
        }
    }
}