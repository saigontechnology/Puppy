using System;
using System.Collections.Generic;
using Puppy.DataTable.Models;

namespace Puppy.DataTable.Utils.Reflection
{
    public static class DataTableColumnExtensions
    {
        public static ColDefModel[] ColDefs(this Type t)
        {
            var propInfos = DataTablesTypeInfo.Properties(t);
            var columnList = new List<ColDefModel>();

            foreach (var dataTablesPropertyInfo in propInfos)
            {
                var colDef = new ColDefModel(dataTablesPropertyInfo.PropertyInfo.Name, dataTablesPropertyInfo.PropertyInfo.PropertyType);

                foreach (var att in dataTablesPropertyInfo.Attributes)
                {
                    att.ApplyTo(colDef, dataTablesPropertyInfo.PropertyInfo);
                }

                columnList.Add(colDef);
            }
            return columnList.ToArray();
        }

        public static ColDefModel[] ColDefs<TResult>()
        {
            return ColDefs(typeof(TResult));
        }
    }
}