using Puppy.Core.StringUtils;
using Puppy.DataTable.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Puppy.DataTable.Processing
{
    internal static class TypeFilters
    {
        internal static string FilterMethod(string q, List<object> parametersForLinqQuery, Type type)
        {
            Func<string, string, string> makeClause = (method, keyword) =>
            {
                parametersForLinqQuery.Add(keyword.ParseTo(type));
                var indexOfParameter = parametersForLinqQuery.Count - 1;
                return $"{method}(@{indexOfParameter})";
            };

            if (q.StartsWith("^"))
            {
                if (q.EndsWith("$"))
                {
                    parametersForLinqQuery.Add(q.Substring(1, q.Length - 2).ParseTo(type));
                    var indexOfParameter = parametersForLinqQuery.Count - 1;
                    return $"Equals((object)@{indexOfParameter})";
                }
                return makeClause("StartsWith", q.Substring(1));
            }

            if (q.EndsWith("$"))
            {
                return makeClause("EndsWith", q.Substring(0, q.Length - 1));
            }
            return makeClause("Contains", q);
        }

        public static string NumericFilter(string query, string columnName, DataTablePropertyInfo propertyInfo, List<object> parametersForLinqQuery)
        {
            if (query.StartsWith("^")) query = query.TrimStart('^');

            if (query.EndsWith("$")) query = query.TrimEnd('$');

            if (query == "~") return string.Empty;

            string clause = null;

            if (query.Contains("~"))
            {
                var parts = query.Split('~');
                try
                {
                    parametersForLinqQuery.Add(ChangeType(propertyInfo, parts[0]));
                    clause = $"{columnName} >= @{parametersForLinqQuery.Count - 1}";
                }
                catch (FormatException)
                {
                }
                try
                {
                    parametersForLinqQuery.Add(ChangeType(propertyInfo, parts[1]));
                    if (clause != null) clause += " and ";
                    clause += $"{columnName} <= @{parametersForLinqQuery.Count - 1}";
                }
                catch (FormatException)
                {
                }
                return clause ?? "true";
            }
            try
            {
                parametersForLinqQuery.Add(ChangeType(propertyInfo, query));
                return $"{columnName} == @{parametersForLinqQuery.Count - 1}";
            }
            catch (FormatException)
            {
            }
            return null;
        }

        private static object ChangeType(DataTablePropertyInfo propertyInfo, string query)
        {
            if (propertyInfo.PropertyInfo.PropertyType.GetTypeInfo().IsGenericType && propertyInfo.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var u = Nullable.GetUnderlyingType(propertyInfo.Type);
                return Convert.ChangeType(query, u);
            }

            return Convert.ChangeType(query, propertyInfo.Type);
        }

        public static string DateTimeOffsetFilter(string keyword, string columnName, DataTablePropertyInfo propertyInfo, List<object> parametersForLinqQuery)
        {
            if (keyword == "~") return string.Empty;

            var filterString = null as string;

            if (keyword.Contains("~"))
            {
                var parts = keyword.Split('~');

                if (DateTimeOffset.TryParse(parts[0] ?? "", out var start))
                {
                    filterString = columnName + " >= @" + parametersForLinqQuery.Count;
                    parametersForLinqQuery.Add(start);
                }

                if (DateTimeOffset.TryParse(parts[1] ?? "", out var end))
                {
                    filterString = (filterString == null ? null : filterString + " and ") + columnName + " <= @" + parametersForLinqQuery.Count;
                    parametersForLinqQuery.Add(end);
                }

                return filterString ?? "";
            }

            if (DateTimeOffset.TryParse(keyword, out var dateTime))
            {
                if (dateTime.Date == dateTime)
                {
                    dateTime = dateTime.ToUniversalTime();

                    parametersForLinqQuery.Add(dateTime);
                    parametersForLinqQuery.Add(dateTime.AddDays(1));
                    filterString = string.Format("{0} >= @{1} and {0} < @{2}", columnName, parametersForLinqQuery.Count - 2, parametersForLinqQuery.Count - 1);
                }
                else
                {
                    dateTime = dateTime.ToUniversalTime();

                    filterString = string.Format("{0} == @" + parametersForLinqQuery.Count, columnName);
                    parametersForLinqQuery.Add(dateTime);
                }
            }
            return filterString;
        }

        public static string DateTimeFilter(string keyword, string columnName, DataTablePropertyInfo propertyInfo, List<object> parametersForLinqQuery)
        {
            if (keyword == "~") return string.Empty;
            var filterString = null as string;

            if (keyword.Contains("~"))
            {
                var parts = keyword.Split('~');

                if (DateTime.TryParse(parts[0] ?? string.Empty, out var start))
                {
                    filterString = columnName + " >= @" + parametersForLinqQuery.Count;
                    parametersForLinqQuery.Add(start);
                }

                if (DateTime.TryParse(parts[1] ?? string.Empty, out var end))
                {
                    filterString = (filterString == null ? null : filterString + " and ") + columnName + $" <= @{parametersForLinqQuery.Count}";
                    parametersForLinqQuery.Add(end);
                }

                return filterString ?? string.Empty;
            }

            if (DateTime.TryParse(keyword, out var dateTime))
            {
                if (dateTime.Date == dateTime)
                {
                    dateTime = dateTime.ToUniversalTime();
                    parametersForLinqQuery.Add(dateTime);
                    parametersForLinqQuery.Add(dateTime.AddDays(1));
                    filterString = $"({columnName} >= @{parametersForLinqQuery.Count - 2} and {columnName} < @{parametersForLinqQuery.Count - 1})";
                }
                else
                {
                    dateTime = dateTime.ToUniversalTime();
                    filterString = $"{columnName} == @{parametersForLinqQuery.Count}";
                    parametersForLinqQuery.Add(dateTime);
                }
            }
            return filterString;
        }

        public static string BoolFilter(string keyword, string columnName, DataTablePropertyInfo propertyInfo, List<object> parametersForLinqQuery)
        {
            keyword = keyword?.TrimStart('^').TrimEnd('$');
            var lowerCaseQuery = keyword?.ToLowerInvariant();
            if (lowerCaseQuery == "false" || lowerCaseQuery == "true")
            {
                if (keyword.ToLower() == "true") return columnName + " == true";
                return columnName + " == false";
            }
            if (propertyInfo.Type == typeof(bool?))
            {
                if (lowerCaseQuery == "null") return columnName + " == null";
            }
            return null;
        }

        public static string StringFilter(string keyword, string columnName, DataTablePropertyInfo columnType, List<object> parametersForLinqQuery)
        {
            if (keyword == ".*") return "";

            string parameterArg;

            if (keyword.StartsWith("^"))
            {
                if (keyword.EndsWith("$"))
                {
                    parametersForLinqQuery.Add(keyword.Substring(1, keyword.Length - 2));
                    parameterArg = "@" + (parametersForLinqQuery.Count - 1);
                    return $"{columnName} ==  {parameterArg}";
                }

                parametersForLinqQuery.Add(keyword.Substring(1));
                parameterArg = "@" + (parametersForLinqQuery.Count - 1);
                return $"({columnName} != null && {columnName} != \"\" && ({columnName} ==  {parameterArg} || {columnName}.StartsWith({parameterArg})))";
            }

            parametersForLinqQuery.Add(keyword);

            parameterArg = "@" + (parametersForLinqQuery.Count - 1);

            return $"({columnName} != null && {columnName} != \"\" && ({columnName} ==  {parameterArg} || {columnName}.StartsWith({parameterArg}) || {columnName}.Contains({parameterArg})))";
        }

        public static string EnumFilter(string keyword, string columnName, DataTablePropertyInfo propertyInfo, List<object> parametersForLinqQuery)
        {
            if (keyword.StartsWith("^")) keyword = keyword.Substring(1);

            if (keyword.EndsWith("$")) keyword = keyword.Substring(0, keyword.Length - 1);

            parametersForLinqQuery.Add(keyword.ParseTo(propertyInfo.Type));

            return columnName + " == @" + (parametersForLinqQuery.Count - 1);
        }
    }
}