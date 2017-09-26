using Microsoft.AspNetCore.Mvc.ModelBinding;
using Puppy.DataTable.Constants;
using Puppy.DataTable.Models.Request;
using System;
using System.Threading.Tasks;

namespace Puppy.DataTable
{
    /// <inheritdoc />
    public class DataTableModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProvider = bindingContext.ValueProvider;

            int columns = GetValue<int>(valueProvider, PropertyConst.Columns);

            if (columns <= 0)
            {
                var bindModel = BindModel(valueProvider);

                bindingContext.Result = ModelBindingResult.Success(bindModel);

                return Task.FromResult(bindModel);
            }

            var bindLegacyModel = BindLegacyModel(valueProvider, columns);

            bindingContext.Result = ModelBindingResult.Success(bindLegacyModel);

            return Task.FromResult(bindLegacyModel);
        }

        private static object BindModel(IValueProvider valueProvider)
        {
            DataTableParamModel obj = new DataTableParamModel
            {
                DisplayStart = GetValue<int>(valueProvider, "start"),
                DisplayLength = GetValue<int>(valueProvider, "length"),
                Search = GetValue<string>(valueProvider, "search[value]"),
                EscapeRegex = GetValue<bool>(valueProvider, "search[regex]"),
                Echo = GetValue<int>(valueProvider, "draw")
            };

            int colIdx = 0;
            while (true)
            {
                string colPrefix = $"columns[{colIdx}]";
                string colName = GetValue<string>(valueProvider, $"{colPrefix}[data]");
                if (string.IsNullOrWhiteSpace(colName))
                {
                    break;
                }
                obj.ColumnNames.Add(colName);
                obj.ListIsSortable.Add(GetValue<bool>(valueProvider, $"{colPrefix}[orderable]"));
                obj.ListIsSearchable.Add(GetValue<bool>(valueProvider, $"{colPrefix}[searchable]"));
                obj.SearchValues.Add(GetValue<string>(valueProvider, $"{colPrefix}[search][value]"));
                obj.ListIsEscapeRegexColumn.Add(GetValue<bool>(valueProvider, $"{colPrefix}[searchable][regex]"));
                colIdx++;
            }
            obj.Columns = colIdx;
            colIdx = 0;

            while (true)
            {
                string colPrefix = $"order[{colIdx}]";

                int? orderColumn = GetValue<int?>(valueProvider, $"{colPrefix}[column]");

                if (orderColumn.HasValue)
                {
                    obj.SortCol.Add(orderColumn.Value);
                    obj.SortDir.Add(GetValue<string>(valueProvider, $"{colPrefix}[dir]"));
                    colIdx++;
                }
                else
                {
                    break;
                }
            }
            obj.SortingCols = colIdx;
            return obj;
        }

        private static DataTableParamModel BindLegacyModel(IValueProvider valueProvider, int columns)
        {
            DataTableParamModel obj = new DataTableParamModel(columns)
            {
                DisplayStart = GetValue<int>(valueProvider, PropertyConst.DisplayStart),
                DisplayLength = GetValue<int>(valueProvider, PropertyConst.DisplayLength),
                Search = GetValue<string>(valueProvider, PropertyConst.Search),
                EscapeRegex = GetValue<bool>(valueProvider, PropertyConst.EscapeRegex),
                SortingCols = GetValue<int>(valueProvider, PropertyConst.SortingCols),
                Echo = GetValue<int>(valueProvider, PropertyConst.Echo)
            };

            for (int i = 0; i < obj.Columns; i++)
            {
                obj.ListIsSortable.Add(GetValue<bool>(valueProvider, $"{PropertyConst.Sortable}_{i}"));
                obj.ListIsSearchable.Add(GetValue<bool>(valueProvider, $"{PropertyConst.Searchable}_{i}"));

                // Important Legacy DataTable bind sSearch for sSearchValues
                obj.SearchValues.Add(GetValue<string>(valueProvider, $"{PropertyConst.Search}_{i}"));

                obj.ListIsEscapeRegexColumn.Add(GetValue<bool>(valueProvider, $"{PropertyConst.EscapeRegex}_{i}"));
                obj.SortCol.Add(GetValue<int>(valueProvider, $"{PropertyConst.SortCol}_{i}"));
                obj.SortDir.Add(GetValue<string>(valueProvider, $"{PropertyConst.SortDir}_{i}"));
            }
            return obj;
        }

        private static T GetValue<T>(IValueProvider valueProvider, string key)
        {
            ValueProviderResult valueResult = valueProvider.GetValue(key);
            return valueResult.ConvertTo<T>();
        }
    }

    public class DataTablesModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.Metadata.ModelType == typeof(DataTableParamModel)
                ? new DataTableModelBinder()
                : null;
        }
    }
}