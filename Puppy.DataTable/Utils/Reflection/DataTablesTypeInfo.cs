using Puppy.DataTable.Attributes;
using Puppy.DataTable.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Puppy.DataTable.Utils.Reflection
{
    internal static class DataTablesTypeInfo
    {
        private static readonly ConcurrentDictionary<Type, DataTablePropertyInfo[]> PropertiesCache = new ConcurrentDictionary<Type, DataTablePropertyInfo[]>();

        internal static DataTablePropertyInfo[] Properties(Type type)
        {
            return PropertiesCache.GetOrAdd(type, t =>
            {
                var infos = from pi in t.GetProperties()
                            where pi.GetCustomAttribute<DataTablesExcludeAttribute>() == null
                            let attributes = (pi.GetCustomAttributes()).OfType<DataTableAttributeBase>().ToArray()
                            orderby attributes.OfType<DataTableAttribute>().Select(a => a.Order as int?).SingleOrDefault() ?? int.MaxValue
                            select new DataTablePropertyInfo(pi, attributes);
                return infos.ToArray();
            });
        }
    }

    public static class DataTablesTypeInfo<T>
    {
        public static DataTablePropertyInfo[] Properties { get; }

        internal static DataTablePropertyInfo RowId { get; }

        static DataTablesTypeInfo()
        {
            Properties = DataTablesTypeInfo.Properties(typeof(T));
            RowId = Properties.SingleOrDefault(x => x.Attributes.Any(y => y is DataTableRowIdAttribute));
        }

        public static Dictionary<string, object> ToDictionary(T row)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var pi in Properties)
            {
                dictionary[pi.PropertyInfo.Name] = pi.PropertyInfo.GetValue(row, null);
            }
            if (RowId != null)
            {
                dictionary["DT_RowID"] = RowId.PropertyInfo.GetValue(row, null);
                if (!RowId.Attributes.OfType<DataTableRowIdAttribute>().First().EmitAsColumnName)
                {
                    dictionary.Remove(RowId.PropertyInfo.Name);
                }
            }
            return dictionary;
        }

        public static Func<T, Dictionary<string, object>> ToDictionary(ResponseOptionModel<T> options = null)
        {
            if (options?.DtRowId == null)
            {
                return ToDictionary;
            }
            else
            {
                return row =>
                {
                    var dictionary = new Dictionary<string, object> { ["DT_RowID"] = options.DtRowId(row) };
                    foreach (var pi in Properties)
                    {
                        dictionary[pi.PropertyInfo.Name] = pi.PropertyInfo.GetValue(row, null);
                    }
                    return dictionary;
                };
            }
        }

        //public static OrderedDictionary ToOrderedDictionary(T row)
        //{
        //    var dictionary = new OrderedDictionary();
        //    foreach (var pi in Properties)
        //    {
        //        dictionary[pi.PropertyInfo.Name] = pi.PropertyInfo.GetValue(row, null);
        //    }
        //    return dictionary;
        //}
    }
}