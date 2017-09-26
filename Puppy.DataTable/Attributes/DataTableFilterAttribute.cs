using Puppy.DataTable.Models;
using Puppy.DataTable.Utils;

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

        public DataTableFilterAttribute(DataTablesFilterType filterType) : this(GetFilterTypeString(filterType))
        {
        }

        private static string GetFilterTypeString(DataTablesFilterType filterType)
        {
            return filterType.ToString().ToLower().Replace("range", "-range");
        }

        public override void ApplyTo(ColDefModel colDefModel, System.Reflection.PropertyInfo propertyInfo)
        {
            colDefModel.Filter = new FilterDef(propertyInfo.PropertyType);

            if (Selector != null)
            {
                colDefModel.Filter["sSelector"] = Selector;
            }
            if (_filterType == "none")
            {
                colDefModel.Filter = null;
            }
            else
            {
                if (_filterType != null) colDefModel.Filter.type = _filterType;
            }
        }
    }

    public enum DataTablesFilterType
    {
        None,
        Select,
        Text,
        //Checkbox,
        //NumberRange,
        //DateRange,
        //DateTimeRange
    }
}