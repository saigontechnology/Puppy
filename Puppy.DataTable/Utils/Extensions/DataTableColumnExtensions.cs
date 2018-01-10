using Puppy.DataTable.Models.Config.Column;
using Puppy.DataTable.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puppy.DataTable.Utils.Extensions
{
    public static class DataTableColumnExtensions
    {
        public static ColumnModel[] GetColumns(this Type t)
        {
            var propInfos = DataTableTypeInfo.Properties(t);

            var columnList = new List<ColumnModel>();

            foreach (var dataTablesPropertyInfo in propInfos)
            {
                var colDef = new ColumnModel(dataTablesPropertyInfo.PropertyInfo.Name, dataTablesPropertyInfo.PropertyInfo.PropertyType);

                foreach (var att in dataTablesPropertyInfo.Attributes)
                {
                    att.ApplyTo(colDef, dataTablesPropertyInfo.PropertyInfo);
                }

                columnList.Add(colDef);
            }
            return columnList.ToArray();
        }

        public static ColumnModel[] GetColumns<TResult>()
        {
            return GetColumns(typeof(TResult));
        }
    }
}