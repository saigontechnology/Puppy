using Puppy.DataTable.Helpers.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Puppy.DataTable.Processing
{
    internal static class TypeFilters
    {
        private static readonly Func<string, Type, object> ParseValue =
            (input, t) => t.GetTypeInfo().IsEnum ? Enum.Parse(t, input) : Convert.ChangeType(input, t);

        internal static string FilterMethod(string q, List<object> parametersForLinqQuery, Type type)
        {
            Func<string, string, string> makeClause = (method, query) =>
            {
                parametersForLinqQuery.Add(ParseValue(query, type));
                var indexOfParameter = parametersForLinqQuery.Count - 1;
                return string.Format("{0}(@{1})", method, indexOfParameter);
            };
            if (q.StartsWith("^"))
            {
                if (q.EndsWith("$"))
                {
                    parametersForLinqQuery.Add(ParseValue(q.Substring(1, q.Length - 2), type));
                    var indexOfParameter = parametersForLinqQuery.Count - 1;
                    return string.Format("Equals((object)@{0})", indexOfParameter);
                }
                return makeClause("StartsWith", q.Substring(1));
            }
            else
            {
                if (q.EndsWith("$"))
                {
                    return makeClause("EndsWith", q.Substring(0, q.Length - 1));
                }
                return makeClause("Contains", q);
            }
        }

        public static string NumericFilter(string query, string columnName, DataTablesPropertyInfo propertyInfo, List<object> parametersForLinqQuery)
        {
            if (query.StartsWith("^")) query = query.TrimStart('^');
            if (query.EndsWith("$")) query = query.TrimEnd('$');

            if (query == "~") return string.Empty;

            if (query.Contains("~"))
            {
                var parts = query.Split('~');
                var clause = null as string;
                try
                {
                    parametersForLinqQuery.Add(ChangeType(propertyInfo, parts[0]));
                    clause = string.Format("{0} >= @{1}", columnName, parametersForLinqQuery.Count - 1);
                }
                catch (FormatException)
                {
                }

                try
                {
                    parametersForLinqQuery.Add(ChangeType(propertyInfo, parts[1]));
                    if (clause != null) clause += " and ";
                    clause += string.Format("{0} <= @{1}", columnName, parametersForLinqQuery.Count - 1);
                }
                catch (FormatException)
                {
                }
                return clause ?? "true";
            }

            try
            {
                parametersForLinqQuery.Add(ChangeType(propertyInfo, query));
                return string.Format("{0} == @{1}", columnName, parametersForLinqQuery.Count - 1);
            }
            catch (FormatException)
            {
            }
            return null;
        }

        private static object ChangeType(DataTablesPropertyInfo propertyInfo, string query)
        {
            if (propertyInfo.PropertyInfo.PropertyType.GetTypeInfo().IsGenericType && propertyInfo.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var u = Nullable.GetUnderlyingType(propertyInfo.Type);
                return Convert.ChangeType(query, u);
            }

            return Convert.ChangeType(query, propertyInfo.Type);
        }

        public static string DateTimeOffsetFilter(string query, string columnName, DataTablesPropertyInfo propertyInfo, List<object> parametersForLinqQuery)
        {
            if (query == "~") return string.Empty;
            var filterString = null as string;

            if (query.Contains("~"))
            {
                var parts = query.Split('~');

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
            else
            {
                if (DateTimeOffset.TryParse(query, out var dateTime))
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
        }

        public static string DateTimeFilter(string query, string columnName, DataTablesPropertyInfo propertyInfo, List<object> parametersForLinqQuery)
        {
            if (query == "~") return string.Empty;
            var filterString = null as string;

            if (query.Contains("~"))
            {
                var parts = query.Split('~');

                if (DateTime.TryParse(parts[0] ?? "", out var start))
                {
                    filterString = columnName + " >= @" + parametersForLinqQuery.Count;
                    parametersForLinqQuery.Add(start);
                }

                if (DateTime.TryParse(parts[1] ?? "", out var end))
                {
                    filterString = (filterString == null ? null : filterString + " and ") + columnName + " <= @" + parametersForLinqQuery.Count;
                    parametersForLinqQuery.Add(end);
                }

                return filterString ?? "";
            }
            else
            {
                if (DateTime.TryParse(query, out var dateTime))
                {
                    if (dateTime.Date == dateTime)
                    {
                        dateTime = dateTime.ToUniversalTime();
                        parametersForLinqQuery.Add(dateTime);
                        parametersForLinqQuery.Add(dateTime.AddDays(1));
                        filterString = string.Format("({0} >= @{1} and {0} < @{2})", columnName, parametersForLinqQuery.Count - 2, parametersForLinqQuery.Count - 1);
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
        }

        public static string BoolFilter(string query, string columnName, DataTablesPropertyInfo propertyInfo, List<object> parametersForLinqQuery)
        {
            query = query?.TrimStart('^').TrimEnd('$');
            var lowerCaseQuery = query?.ToLowerInvariant();
            if (lowerCaseQuery == "false" || lowerCaseQuery == "true")
            {
                if (query.ToLower() == "true") return columnName + " == true";
                return columnName + " == false";
            }
            if (propertyInfo.Type == typeof(bool?))
            {
                if (lowerCaseQuery == "null") return columnName + " == null";
            }
            return null;
        }

        public static string StringFilter(string q, string columnName, DataTablesPropertyInfo columnType, List<object> parametersForLinqQuery)
        {
            if (q == ".*") return "";
            if (q.StartsWith("^"))
            {
                if (q.EndsWith("$"))
                {
                    parametersForLinqQuery.Add(q.Substring(1, q.Length - 2));
                    var parameterArg = "@" + (parametersForLinqQuery.Count - 1);
                    return string.Format("{0} ==  {1}", columnName, parameterArg);
                }
                else
                {
                    parametersForLinqQuery.Add(q.Substring(1));
                    var parameterArg = "@" + (parametersForLinqQuery.Count - 1);
                    return string.Format("({0} != null && {0} != \"\" && ({0} ==  {1} || {0}.StartsWith({1})))", columnName, parameterArg);
                }
            }
            else
            {
                parametersForLinqQuery.Add(q);
                var parameterArg = "@" + (parametersForLinqQuery.Count - 1);
                //return string.Format("{0} ==  {1}", columnName, parameterArg);
                return
                    string.Format(
                        "({0} != null && {0} != \"\" && ({0} ==  {1} || {0}.StartsWith({1}) || {0}.Contains({1})))",
                        columnName, parameterArg);
            }
        }

        public static string EnumFilter(string q, string columnName, DataTablesPropertyInfo propertyInfo, List<object> parametersForLinqQuery)
        {
            if (q.StartsWith("^")) q = q.Substring(1);
            if (q.EndsWith("$")) q = q.Substring(0, q.Length - 1);
            parametersForLinqQuery.Add(ParseValue(q, propertyInfo.Type));
            return columnName + " == @" + (parametersForLinqQuery.Count - 1);
        }
    }
}