using Puppy.DataTable.Models;

namespace Puppy.DataTable.Attributes
{
    public class DataTablesFilterAttribute : DataTablesAttributeBase
    {
        private readonly string _filterType;

        public DataTablesFilterAttribute()
        {
        }

        public DataTablesFilterAttribute(DataTablesFilterType filterType)
            : this(GetFilterTypeString(filterType))
        {
        }

        /// <summary>
        ///     Sets sSelector on the column (i.e. selector for custom positioning) 
        /// </summary>
        public string Selector { get; set; }

        private static string GetFilterTypeString(DataTablesFilterType filterType)
        {
            return filterType.ToString().ToLower().Replace("range", "-range");
        }

        public DataTablesFilterAttribute(string filterType)
        {
            _filterType = filterType;
        }

        public override void ApplyTo(ColDefModel colDefModel, System.Reflection.PropertyInfo pi)
        {
            colDefModel.Filter = new FilterDef(pi.PropertyType);
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
        NumberRange,
        DateRange,
        Checkbox,
        Text,
        DateTimeRange
    }
}