using Puppy.DataTable.Attributes;
using Puppy.DataTable.Constants;
using Puppy.DataTable.Models.Response;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Puppy.DataTable.Utils.Reflection
{
    internal static class DataTableTypeInfo
    {
        private static readonly ConcurrentDictionary<Type, DataTablePropertyInfo[]> PropertiesCache =
            new ConcurrentDictionary<Type, DataTablePropertyInfo[]>();

        internal static DataTablePropertyInfo[] Properties(Type type)
        {
            return PropertiesCache.GetOrAdd(type, t =>
            {
                var infos = from propertyInfo in t.GetProperties()
                            where propertyInfo.GetCustomAttribute<DataTableIgnoreAttribute>() == null
                            let attributes = propertyInfo.GetCustomAttributes().OfType<DataTableAttributeBase>().ToArray()
                            orderby attributes.OfType<DataTableAttribute>().Select(a => a.Order as int?).SingleOrDefault() ?? ConfigConst.DefaultOrder
                            select new DataTablePropertyInfo(propertyInfo, attributes);
                return infos.ToArray();
            });
        }
    }

    public static class DataTableTypeInfo<T>
    {
        public static DataTablePropertyInfo[] Properties { get; }

        internal static DataTablePropertyInfo RowId { get; }

        static DataTableTypeInfo()
        {
            Properties = DataTableTypeInfo.Properties(typeof(T));
            RowId = Properties.SingleOrDefault(x => x.Attributes.Any(y => y is DataTableRowIdAttribute));
        }

        public static Dictionary<string, object> ToDictionary(T row)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var pi in Properties)
            {
                dictionary[pi.PropertyInfo.Name] = pi.PropertyInfo.GetValue(row, null);
            }

            if (RowId == null)
            {
                return dictionary;
            }

            dictionary[PropertyConst.RowId] = RowId.PropertyInfo.GetValue(row, null);

            if (!RowId.Attributes.OfType<DataTableRowIdAttribute>().First().EmitAsColumnName)
            {
                dictionary.Remove(RowId.PropertyInfo.Name);
            }

            return dictionary;
        }

        public static Func<T, Dictionary<string, object>> ToDictionary(ResponseOptionModel<T> options = null)
        {
            if (options?.DtRowId == null)
            {
                return ToDictionary;
            }

            return row =>
            {
                var dictionary = new Dictionary<string, object>
                {
                    [PropertyConst.RowId] = options.DtRowId(row)
                };

                foreach (var pi in Properties)
                {
                    dictionary[pi.PropertyInfo.Name] = pi.PropertyInfo.GetValue(row, null);
                }
                return dictionary;
            };
        }
    }
}