using Puppy.Core.StringUtils;
using Puppy.Core.TypeUtils;
using Puppy.DataTable.Constants;
using Puppy.DataTable.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Puppy.DataTable.Processing.Request
{
    internal static class TypeFilter
    {
        internal static string FilterMethod(string terms, List<object> parametersForLinqQuery, Type type)
        {
            string Clause(string conditional, string query)
            {
                parametersForLinqQuery.Add(query.ParseTo(type));
                var indexOfParameter = parametersForLinqQuery.Count - 1;
                return $"{conditional}(@{indexOfParameter})";
            }

            if (terms.StartsWith("^"))
            {
                if (!terms.EndsWith("$"))
                {
                    return Clause(ConditionalCost.StartsWith, terms.Substring(1));
                }

                parametersForLinqQuery.Add(terms.Substring(1, terms.Length - 2).ParseTo(type));
                var indexOfParameter = parametersForLinqQuery.Count - 1;
                return $"Equals((object)@{indexOfParameter})";
            }

            return terms.EndsWith("$")
                ? Clause(ConditionalCost.EndsWith, terms.Substring(0, terms.Length - 1))
                : Clause(ConditionalCost.Contains, terms);
        }

        public static string NumericFilter(string terms, string columnName, DataTablePropertyInfo propertyInfo, List<object> parametersForLinqQuery)
        {
            if (terms.StartsWith("^")) terms = terms.TrimStart('^');

            if (terms.EndsWith("$")) terms = terms.TrimEnd('$');

            if (terms == "~") return string.Empty;

            string clause = null;

            if (terms.Contains("~"))
            {
                var parts = terms.Split('~');
                try
                {
                    parametersForLinqQuery.Add(ChangeType(parts[0], propertyInfo));
                    clause = $"{columnName} >= @{parametersForLinqQuery.Count - 1}";
                }
                catch (FormatException)
                {
                }
                try
                {
                    parametersForLinqQuery.Add(ChangeType(parts[1], propertyInfo));
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
                parametersForLinqQuery.Add(ChangeType(terms, propertyInfo));
                return $"{columnName} == @{parametersForLinqQuery.Count - 1}";
            }
            catch (FormatException)
            {
            }
            return null;
        }

        private static object ChangeType(string terms, DataTablePropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyInfo.PropertyType.GetTypeInfo().IsGenericType && propertyInfo.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var u = Nullable.GetUnderlyingType(propertyInfo.Type);
                return Convert.ChangeType(terms, u);
            }

            return Convert.ChangeType(terms, propertyInfo.Type);
        }

        public static string DateTimeOffsetFilter(string terms, string columnName, DataTablePropertyInfo propertyInfo, List<object> parametersForLinqQuery)
        {
            if (terms == "~") return string.Empty;

            var filterString = null as string;

            if (terms.Contains("~"))
            {
                var parts = terms.Split('~');

                if (DateTimeOffset.TryParse(parts[0] ?? string.Empty, out var start))
                {
                    filterString = $"{columnName} >= @{parametersForLinqQuery.Count}";
                    parametersForLinqQuery.Add(start);
                }

                if (!DateTimeOffset.TryParse(parts[1] ?? string.Empty, out var end))
                {
                    return filterString ?? string.Empty;
                }

                filterString = (filterString == null ? null : $"{filterString} and ") + columnName + $" <= @{parametersForLinqQuery.Count}";

                parametersForLinqQuery.Add(end);

                return filterString;
            }

            if (!DateTimeOffset.TryParse(terms, out var dateTime))
            {
                return null;
            }

            if (dateTime.Date == dateTime)
            {
                dateTime = dateTime.ToUniversalTime();
                parametersForLinqQuery.Add(dateTime);
                parametersForLinqQuery.Add(dateTime.AddDays(1));
                filterString = $"{columnName} >= @{parametersForLinqQuery.Count - 2} and {columnName} < @{parametersForLinqQuery.Count - 1}";
            }
            else
            {
                dateTime = dateTime.ToUniversalTime();
                filterString = $"{columnName} == @{parametersForLinqQuery.Count}";
                parametersForLinqQuery.Add(dateTime);
            }
            return filterString;
        }

        public static string DateTimeFilter(string terms, string columnName, DataTablePropertyInfo propertyInfo, List<object> parametersForLinqQuery)
        {
            if (terms == "~") return string.Empty;

            string filterString = null;

            if (terms.Contains("~"))
            {
                var parts = terms.Split('~');

                if (DateTime.TryParse(parts[0] ?? string.Empty, out var start))
                {
                    filterString = $"{columnName} >= @{parametersForLinqQuery.Count}";
                    parametersForLinqQuery.Add(start);
                }

                if (!DateTime.TryParse(parts[1] ?? string.Empty, out var end))
                {
                    return filterString ?? string.Empty;
                }

                filterString = (filterString == null ? null : $"{filterString} and ") + columnName + $" <= @{parametersForLinqQuery.Count}";
                parametersForLinqQuery.Add(end);

                return filterString;
            }

            if (DateTime.TryParse(terms, out var dateTime))
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

        public static string BoolFilter(string terms, string columnName, DataTablePropertyInfo propertyInfo, List<object> parametersForLinqQuery)
        {
            terms = terms?.TrimStart('^').TrimEnd('$');

            var termsLowerCase = terms?.ToLowerInvariant();

            if (termsLowerCase == DataConst.FalseLower || termsLowerCase == DataConst.TrueLower)
            {
                return terms.ToLower() == DataConst.TrueLower
                    ? $"{columnName} == {DataConst.TrueLower}"
                    : $"{columnName} == {DataConst.FalseLower}";
            }

            if (propertyInfo.Type != typeof(bool?))
            {
                return null;
            }

            return DataConst.Null.Equals(terms, StringComparison.CurrentCultureIgnoreCase)
                ? $"{columnName} == {DataConst.Null}"
                : null;
        }

        public static string StringFilter(string terms, string columnName, DataTablePropertyInfo columnType, List<object> parametersForLinqQuery)
        {
            if (terms == ".*") return "";

            string parameterArg;

            if (terms.StartsWith("^"))
            {
                if (terms.EndsWith("$"))
                {
                    parametersForLinqQuery.Add(terms.Substring(1, terms.Length - 2));
                    parameterArg = $"@{parametersForLinqQuery.Count - 1}";
                    return $"{columnName} == {parameterArg}";
                }

                parametersForLinqQuery.Add(terms.Substring(1));
                parameterArg = "@" + (parametersForLinqQuery.Count - 1);
                return $"({columnName} != {DataConst.Null} && {columnName} != \"\" && ({columnName} ==  {parameterArg} || {columnName}.{ConditionalCost.StartsWith}({parameterArg})))";
            }

            parametersForLinqQuery.Add(terms);

            parameterArg = "@" + (parametersForLinqQuery.Count - 1);

            return $"({columnName} != {DataConst.Null} && {columnName} != \"\" && ({columnName} ==  {parameterArg} || {columnName}.{ConditionalCost.StartsWith}({parameterArg}) || {columnName}.{ConditionalCost.Contains}({parameterArg})))";
        }

        public static string EnumFilter(string terms, string columnName, DataTablePropertyInfo propertyInfo, List<object> parametersForLinqQuery)
        {
            if (terms.StartsWith("^")) terms = terms.Substring(1);

            if (terms.EndsWith("$")) terms = terms.Substring(0, terms.Length - 1);

            if (propertyInfo.Type.IsNullableEnumType())
            {
                if (DataConst.Null.Equals(terms, StringComparison.CurrentCultureIgnoreCase))
                {
                    return $"{columnName} == {DataConst.Null}";
                }
            }

            parametersForLinqQuery.Add(terms.ParseTo(propertyInfo.Type));
            return $"{columnName} == @{parametersForLinqQuery.Count - 1}";
        }
    }
}