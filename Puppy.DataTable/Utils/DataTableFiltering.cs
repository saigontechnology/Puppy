using Puppy.Core.TypeUtils;
using Puppy.DataTable.Models;
using Puppy.DataTable.Processing;
using Puppy.DataTable.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Puppy.DataTable.Utils
{
    internal class DataTableFiltering
    {
        public IQueryable<T> ApplyFiltersAndSort<T>(DataTableParamModel dtParameters, IQueryable<T> data, DataTablePropertyInfo[] columns)
        {
            if (!string.IsNullOrEmpty(dtParameters.sSearch))
            {
                var parts = new List<string>();

                var parameters = new List<object>();

                for (var i = 0; i < dtParameters.iColumns; i++)
                {
                    if (dtParameters.bSearchable[i])
                    {
                        try
                        {
                            parts.Add(GetFilterClause(dtParameters.sSearch, columns[i], parameters));
                        }
                        catch (Exception)
                        {
                            // if the clause doesn't work, skip it!
                            // ex: can't parse a string to enum or datetime type
                        }
                    }
                }
                var values = parts.Where(p => p != null);
                data = data.Where(string.Join(" or ", values), parameters.ToArray());
            }
            for (int i = 0; i < dtParameters.sSearchValues.Count; i++)
            {
                if (dtParameters.bSearchable[i])
                {
                    var searchColumn = dtParameters.sSearchValues[i];
                    if (!string.IsNullOrWhiteSpace(searchColumn))
                    {
                        DataTablePropertyInfo column = FindColumn(dtParameters, columns, i);
                        var parameters = new List<object>();
                        var filterClause = GetFilterClause(searchColumn, column, parameters);
                        if (string.IsNullOrWhiteSpace(filterClause) == false)
                        {
                            data = data.Where(filterClause, parameters.ToArray());
                        }
                    }
                }
            }
            string sortString = "";
            for (int i = 0; i < dtParameters.iSortingCols; i++)
            {
                int columnNumber = dtParameters.iSortCol[i];
                DataTablePropertyInfo column = FindColumn(dtParameters, columns, columnNumber);
                string columnName = column.PropertyInfo.Name;
                string sortDir = dtParameters.sSortDir[i];
                if (i != 0)
                    sortString += ", ";
                sortString += columnName + " " + sortDir;
            }
            if (string.IsNullOrWhiteSpace(sortString))
            {
                sortString = columns[0].PropertyInfo.Name;
            }
            data = data.OrderBy(sortString);

            return data;
        }

        private static DataTablePropertyInfo FindColumn(DataTableParamModel dtParameters, DataTablePropertyInfo[] columns, int i)
        {
            if (dtParameters.sColumnNames.Any())
            {
                return columns.First(x => x.PropertyInfo.Name == dtParameters.sColumnNames[i]);
            }

            return columns[i];
        }

        public static void RegisterFilter<T>(GuardedFilter filter)
        {
            Filters.Add(Guard(arg => arg is T, filter));
        }

        public delegate string GuardedFilter(string query, string columnName, DataTablePropertyInfo columnType, List<object> parametersForLinqQuery);

        private delegate string ReturnedFilteredQueryForType(string query, string columnName, DataTablePropertyInfo columnType, List<object> parametersForLinqQuery);

        private static readonly List<ReturnedFilteredQueryForType> Filters = new List<ReturnedFilteredQueryForType>
        {
            Guard(IsBoolType, TypeFilters.BoolFilter),
            Guard(IsDateTimeType, TypeFilters.DateTimeFilter),
            Guard(IsDateTimeOffsetType, TypeFilters.DateTimeOffsetFilter),
            Guard(IsNumericType, TypeFilters.NumericFilter),
            Guard(IsEnumType, TypeFilters.EnumFilter),
            Guard(arg => arg.Type == typeof (string), TypeFilters.StringFilter),
        };

        private static ReturnedFilteredQueryForType Guard(Func<DataTablePropertyInfo, bool> guard, GuardedFilter filter)
        {
            return (q, c, t, p) => !guard(t) ? null : filter(q, c, t, p);
        }

        private static string GetFilterClause(string query, DataTablePropertyInfo column, List<object> parametersForLinqQuery)
        {
            Func<string, string> filterClause =
                queryPart =>
                    Filters
                        .Select(f => f(queryPart, column.PropertyInfo.Name, column, parametersForLinqQuery))
                        .FirstOrDefault(filterPart => filterPart != null) ?? string.Empty;

            var queryParts = query.Split('|').Select(filterClause).Where(fc => fc != "").ToArray();

            if (queryParts.Any())
            {
                return "(" + string.Join(") OR (", queryParts) + ")";
            }

            return null;
        }

        private static bool IsNumericType(DataTablePropertyInfo propertyInfo)
        {
            bool isNumericType = propertyInfo.Type.IsNumericType();
            return isNumericType;
        }

        private static bool IsEnumType(DataTablePropertyInfo propertyInfo)
        {
            bool isEnumType = propertyInfo.Type.IsEnumType() || propertyInfo.Type.IsNullableEnumType();
            return isEnumType;
        }

        private static bool IsBoolType(DataTablePropertyInfo propertyInfo)
        {
            bool isBoolType = propertyInfo.Type == typeof(bool) || propertyInfo.Type == typeof(bool?);
            return isBoolType;
        }

        private static bool IsDateTimeType(DataTablePropertyInfo propertyInfo)
        {
            bool isDateTimeType = propertyInfo.Type == typeof(DateTime) || propertyInfo.Type == typeof(DateTime?);
            return isDateTimeType;
        }

        private static bool IsDateTimeOffsetType(DataTablePropertyInfo propertyInfo)
        {
            bool isDateTimeOffsetType = propertyInfo.Type == typeof(DateTimeOffset) || propertyInfo.Type == typeof(DateTimeOffset?);
            return isDateTimeOffsetType;
        }
    }
}