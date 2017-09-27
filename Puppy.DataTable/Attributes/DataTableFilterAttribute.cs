using EnumsNET;
using Puppy.DataTable.Constants;
using Puppy.DataTable.Models.Config.Column;
using Puppy.DataTable.Processing.Response;

namespace Puppy.DataTable.Attributes
{
    public class DataTableFilterAttribute : DataTableAttributeBase
    {
        private readonly string _filterType;

        /// <summary>
        ///     Sets sSelector on the column (i.e. selector for custom positioning) 
        /// </summary>
        public string Selector { get; set; }

        public DataTableFilterAttribute()
        {
        }

        public DataTableFilterAttribute(string filterType) : this()
        {
            _filterType = filterType;
        }

        public DataTableFilterAttribute(FilterType filterType) : this(GetFilterTypeString(filterType))
        {
        }

        private static string GetFilterTypeString(FilterType filterType)
        {
            return filterType.AsString(EnumFormat.DisplayName);
        }

        public override void ApplyTo(ColumnModel columnModel, System.Reflection.PropertyInfo propertyInfo)
        {
            columnModel.ColumnFilter = new ColumnFilter(propertyInfo.PropertyType);

            if (Selector != null)
            {
                columnModel.ColumnFilter[PropertyConst.Selector] = Selector;
            }
            if (_filterType == FilterConst.None)
            {
                columnModel.ColumnFilter = null;
            }
            else
            {
                if (_filterType != null)
                {
                    columnModel.ColumnFilter.FilterType = _filterType;
                }
            }
        }
    }
}