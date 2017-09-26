using Microsoft.AspNetCore.Mvc.ModelBinding;
using Puppy.DataTable.Models;
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
            int columns = GetValue<int>(valueProvider, "iColumns");

            //TODO: Consider whether this should be pushed to a worker thread...
            if (columns == 0)
            {
                var bindV10Model = BindV10Model(valueProvider);
                bindingContext.Result = ModelBindingResult.Success(bindV10Model);
                return Task.FromResult(bindV10Model);
            }

            var bindLegacyModel = BindLegacyModel(valueProvider, columns);
            bindingContext.Result = ModelBindingResult.Success(bindLegacyModel);
            return Task.FromResult(bindLegacyModel);
        }

        private object BindV10Model(IValueProvider valueProvider)
        {
            DataTableParamModel obj = new DataTableParamModel
            {
                iDisplayStart = GetValue<int>(valueProvider, "start"),
                iDisplayLength = GetValue<int>(valueProvider, "length"),
                sSearch = GetValue<string>(valueProvider, "search[value]"),
                bEscapeRegex = GetValue<bool>(valueProvider, "search[regex]"),
                sEcho = GetValue<int>(valueProvider, "draw")
            };

            int colIdx = 0;
            while (true)
            {
                string colPrefix = String.Format("columns[{0}]", colIdx);
                string colName = GetValue<string>(valueProvider, colPrefix + "[data]");
                if (String.IsNullOrWhiteSpace(colName))
                {
                    break;
                }
                obj.sColumnNames.Add(colName);
                obj.bSortable.Add(GetValue<bool>(valueProvider, colPrefix + "[orderable]"));
                obj.bSearchable.Add(GetValue<bool>(valueProvider, colPrefix + "[searchable]"));
                obj.sSearchValues.Add(GetValue<string>(valueProvider, colPrefix + "[search][value]"));
                obj.bEscapeRegexColumns.Add(GetValue<bool>(valueProvider, colPrefix + "[searchable][regex]"));
                colIdx++;
            }
            obj.iColumns = colIdx;
            colIdx = 0;

            while (true)
            {
                string colPrefix = String.Format("order[{0}]", colIdx);
                int? orderColumn = GetValue<int?>(valueProvider, colPrefix + "[column]");
                if (orderColumn.HasValue)
                {
                    obj.iSortCol.Add(orderColumn.Value);
                    obj.sSortDir.Add(GetValue<string>(valueProvider, colPrefix + "[dir]"));
                    colIdx++;
                }
                else
                {
                    break;
                }
            }
            obj.iSortingCols = colIdx;
            return obj;
        }

        private DataTableParamModel BindLegacyModel(IValueProvider valueProvider, int columns)
        {
            DataTableParamModel obj = new DataTableParamModel(columns)
            {
                iDisplayStart = GetValue<int>(valueProvider, "iDisplayStart"),
                iDisplayLength = GetValue<int>(valueProvider, "iDisplayLength"),
                sSearch = GetValue<string>(valueProvider, "sSearch"),
                bEscapeRegex = GetValue<bool>(valueProvider, "bEscapeRegex"),
                iSortingCols = GetValue<int>(valueProvider, "iSortingCols"),
                sEcho = GetValue<int>(valueProvider, "sEcho")
            };

            for (int i = 0; i < obj.iColumns; i++)
            {
                obj.bSortable.Add(GetValue<bool>(valueProvider, "bSortable_" + i));
                obj.bSearchable.Add(GetValue<bool>(valueProvider, "bSearchable_" + i));
                obj.sSearchValues.Add(GetValue<string>(valueProvider, "sSearch_" + i));
                obj.bEscapeRegexColumns.Add(GetValue<bool>(valueProvider, "bEscapeRegex_" + i));
                obj.iSortCol.Add(GetValue<int>(valueProvider, "iSortCol_" + i));
                obj.sSortDir.Add(GetValue<string>(valueProvider, "sSortDir_" + i));
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

            if (context.Metadata.ModelType == typeof(DataTableParamModel)) // only encode string types
            {
                return new DataTableModelBinder();
            }

            return null;
        }
    }
}