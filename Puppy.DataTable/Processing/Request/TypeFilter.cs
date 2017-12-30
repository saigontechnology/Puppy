using Puppy.Core.DateTimeUtils;
using Puppy.Core.StringUtils;
using Puppy.Core.TypeUtils;
using Puppy.DataTable.Constants;
using Puppy.DataTable.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Puppy.DataTable.Processing.Request
{
    internal static class TypeFilter
    {
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

        public static string DateTimeOffsetFilter(string terms, string columnName, DataTablePropertyInfo propertyInfo, List<object> parametersForLinqQuery)
        {
            if (terms == "~") return string.Empty;

            string filterString = null;

            // Range Case
            if (terms.Contains("~"))
            {
                var parts = terms.Split('~');

                // FROM DATE TIME
                bool isValidFromDateTime = TryParse(parts[0], out DateTimeOffset fromDateTime);

                if (isValidFromDateTime)
                {
                    fromDateTime = fromDateTime.WithTimeZone(DataTableGlobalConfig.DateTimeTimeZone);

                    parametersForLinqQuery.Add(fromDateTime);

                    filterString = $"{columnName} >= @{parametersForLinqQuery.Count - 1}";
                }

                // TO DATE TIME
                bool isValidToDateTime = TryParse(parts[1], out DateTimeOffset toDateTime);

                if (!isValidToDateTime)
                {
                    return filterString ?? string.Empty;
                }

                toDateTime = toDateTime.WithTimeZone(DataTableGlobalConfig.DateTimeTimeZone);

                parametersForLinqQuery.Add(toDateTime);

                filterString = (filterString == null ? null : $"{filterString} and ") + $"{columnName} <= @{parametersForLinqQuery.Count - 1}";

                return filterString;
            }

            // Single Case
            bool isValidDateTime = TryParse(terms, out DateTimeOffset dateTime);

            if (!isValidDateTime)
            {
                return null;
            }

            dateTime = dateTime.WithTimeZone(DataTableGlobalConfig.DateTimeTimeZone);

            // DateTime only have Date value => search value in same Date
            if (dateTime.Date == dateTime)
            {
                parametersForLinqQuery.Add(dateTime);

                parametersForLinqQuery.Add(dateTime.AddDays(1));

                filterString = $"{columnName} >= @{parametersForLinqQuery.Count - 2} and {columnName} < @{parametersForLinqQuery.Count - 1}";

                return filterString;
            }

            // DateTime have Date and Time value => search value in same Date and Time.

            parametersForLinqQuery.Add(dateTime);

            // If you store DateTime in database include milliseconds => no result match. Ex: in
            // Database "2017-10-16 10:51:09.9761005 +00:00" so user search "2017-10-16 10:51" will
            // return 0 result, because not exactly same (even user give full datetime with
            // milliseconds exactly - this is Linq2SQL issue).
            filterString = $"{columnName} == @{parametersForLinqQuery.Count - 1}";

            return filterString;
        }

        public static string DateTimeFilter(string terms, string columnName, DataTablePropertyInfo propertyInfo, List<object> parametersForLinqQuery)
        {
            if (terms == "~") return string.Empty;

            string filterString = null;

            // Range Case
            if (terms.Contains("~"))
            {
                var parts = terms.Split('~');

                // FROM DATE TIME
                bool isValidFromDateTime = TryParse(parts[0], out DateTime fromDateTime);

                if (isValidFromDateTime)
                {
                    fromDateTime = fromDateTime.WithTimeZone(DataTableGlobalConfig.DateTimeTimeZone);

                    parametersForLinqQuery.Add(fromDateTime);

                    filterString = $"{columnName} >= @{parametersForLinqQuery.Count - 1}";
                }

                // TO DATE TIME
                bool isValidToDateTime = TryParse(parts[1], out DateTime toDateTime);

                if (!isValidToDateTime)
                {
                    return filterString ?? string.Empty;
                }

                toDateTime = toDateTime.WithTimeZone(DataTableGlobalConfig.DateTimeTimeZone);

                parametersForLinqQuery.Add(toDateTime);

                filterString = (filterString == null ? null : $"{filterString} and ") + $"{columnName} <= @{parametersForLinqQuery.Count - 1}";

                return filterString;
            }

            // Single Case
            bool isValidDateTime = TryParse(terms, out DateTime dateTime);

            if (!isValidDateTime)
            {
                return null;
            }

            dateTime = dateTime.WithTimeZone(DataTableGlobalConfig.DateTimeTimeZone);

            // DateTime only have Date value => search value in same Date
            if (dateTime.Date == dateTime)
            {
                parametersForLinqQuery.Add(dateTime);

                parametersForLinqQuery.Add(dateTime.AddDays(1));

                filterString = $"{columnName} >= @{parametersForLinqQuery.Count - 2} and {columnName} < @{parametersForLinqQuery.Count - 1}";

                return filterString;
            }

            // DateTime have Date and Time value => search value in same Date and Time.
            parametersForLinqQuery.Add(dateTime);

            // If you store DateTime in database include milliseconds => no result match. Ex: in
            // Database "2017-10-16 10:51:09.9761005 +00:00" so user search "2017-10-16 10:51" will
            // return 0 result, because not exactly same (even user give full datetime with
            // milliseconds exactly - this is Linq2SQL issue).
            filterString = $"{columnName} == @{parametersForLinqQuery.Count - 1}";

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

        public static string StringFilter(string terms, string columnName, DataTablePropertyInfo propertyInfo, List<object> parametersForLinqQuery)
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
                return
                    $"({columnName} != {DataConst.Null} && {columnName} != \"\" && ({columnName} ==  {parameterArg} || {columnName}.{ConditionalCost.StartsWith}({parameterArg})))";
            }

            parametersForLinqQuery.Add(terms);

            parameterArg = "@" + (parametersForLinqQuery.Count - 1);

            return
                $"({columnName} != {DataConst.Null} && {columnName} != \"\" && ({columnName} ==  {parameterArg} || {columnName}.{ConditionalCost.StartsWith}({parameterArg}) || {columnName}.{ConditionalCost.Contain}({parameterArg})))";
        }

        /// <summary>
        ///     Filter Enum by Label (Display Name ?? Description ?? Name) with conditional Equals,
        ///     StartsWith, Contains
        /// </summary>
        /// <param name="terms">                 </param>
        /// <param name="columnName">            </param>
        /// <param name="propertyInfo">          </param>
        /// <param name="parametersForLinqQuery"></param>
        /// <returns></returns>
        /// <remarks>
        ///     terms is "null" with Type is Nullable Enum work as search null value
        /// </remarks>
        public static string EnumFilter(string terms, string columnName, DataTablePropertyInfo propertyInfo, List<object> parametersForLinqQuery)
        {
            if (terms.StartsWith("^")) terms = terms.Substring(1);

            if (terms.EndsWith("$")) terms = terms.Substring(0, terms.Length - 1);

            if (propertyInfo.Type.IsNullableEnumType())
            {
                // Enum Nullable type, handle for "null" case ("null" string as null obj)
                if (DataConst.Null.Equals(terms, StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(terms))
                {
                    return $"{columnName} == {DataConst.Null}";
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(terms))
                {
                    return null;
                }
            }

            Type type = propertyInfo.Type.GetNotNullableType();

            object enumObject = null;

            string termsLowerCase = terms.ToLowerInvariant();

            // Search condition for Enum: Equals, StartsWith, Contains
            foreach (string enumName in Enum.GetNames(type))
            {
                Enum enumValue = (Enum)enumName.ParseTo(type);

                var valueLowerCase = enumName.ToLowerInvariant();

                if (valueLowerCase.Equals(termsLowerCase, StringComparison.OrdinalIgnoreCase) || valueLowerCase.StartsWith(termsLowerCase) || valueLowerCase.Contains(termsLowerCase))
                {
                    enumObject = enumValue;

                    // Found, return first found item
                    break;
                }
            }

            // Can't parse string to enum, return null
            if (enumObject == null)
            {
                return null;
            }

            parametersForLinqQuery.Add(enumObject);
            return $"{columnName} == @{parametersForLinqQuery.Count - 1}";
        }

        #region Internal Methods

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
                return $"{ConditionalCost.Equal}((object)@{indexOfParameter})";
            }

            return terms.EndsWith("$")
                ? Clause(ConditionalCost.EndsWith, terms.Substring(0, terms.Length - 1))
                : Clause(ConditionalCost.Contain, terms);
        }

        internal static object ChangeType(string terms, DataTablePropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyInfo.PropertyType.GetTypeInfo().IsGenericType &&
                propertyInfo.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var u = Nullable.GetUnderlyingType(propertyInfo.Type);
                return Convert.ChangeType(terms, u);
            }

            return Convert.ChangeType(terms, propertyInfo.Type);
        }

        internal static bool TryParse(string value, out DateTimeOffset dateTimeOffset)
        {
            value = string.IsNullOrWhiteSpace(value) ? string.Empty : value;

            if (DataTableGlobalConfig.RequestDateTimeFormatMode == DateTimeFormatMode.Auto)
            {
                if (DateTimeOffset.TryParse(value, out dateTimeOffset))
                {
                    return true;
                }
            }
            else
            {
                if (DateTimeOffset.TryParseExact(value, DataTableGlobalConfig.DateTimeFormat, null, DateTimeStyles.None, out dateTimeOffset))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool TryParse(string value, out DateTime dateTime)
        {
            value = string.IsNullOrWhiteSpace(value) ? string.Empty : value;

            if (DataTableGlobalConfig.RequestDateTimeFormatMode == DateTimeFormatMode.Auto)
            {
                if (DateTime.TryParse(value, out dateTime))
                {
                    return true;
                }
            }
            else
            {
                if (DateTime.TryParseExact(value, DataTableGlobalConfig.DateTimeFormat, null, DateTimeStyles.None, out dateTime))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion Internal Methods
    }
}