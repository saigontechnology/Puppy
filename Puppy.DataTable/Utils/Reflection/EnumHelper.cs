#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> EnumHelper.cs </Name>
//         <Created> 26/09/17 8:17:27 PM </Created>
//         <Key> 5ed25f75-6aff-4e19-81c4-be8807c02eb3 </Key>
//     </File>
//     <Summary>
//         EnumHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Puppy.Core.StringUtils;
using Puppy.Core.TypeUtils;
using Puppy.DataTable.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Puppy.DataTable.Utils.Reflection
{
    public static class EnumHelper
    {
        public static string GetDisplayName(this Enum value)
        {
            Type enumType = value.GetType();

            var enumValue = Enum.GetName(enumType, value);

            MemberInfo member = enumType.GetMember(enumValue).FirstOrDefault();

            if (!(member.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() is DisplayAttribute displayAttribute))
            {
                return null;
            }

            var displayName = displayAttribute.ResourceType != null ? displayAttribute.GetName() : displayAttribute.Name;

            return !string.IsNullOrWhiteSpace(displayName) ? displayName : null;
        }

        public static string GetDescription(this Enum value)
        {
            Type enumType = value.GetType();

            var enumValue = Enum.GetName(enumType, value);

            MemberInfo member = enumType.GetMember(enumValue).FirstOrDefault();

            if (!(member.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() is DescriptionAttribute descriptionAttribute))
            {
                return null;
            }

            var description = descriptionAttribute.Description;

            return !string.IsNullOrWhiteSpace(description) ? description : null;
        }

        public static string GetName(this Enum value)
        {
            Type enumType = value.GetType();

            var enumValue = Enum.GetName(enumType, value);

            MemberInfo member = enumType.GetMember(enumValue).FirstOrDefault();

            return member?.Name;
        }

        public static List<string> GetListLabel(this Type type)
        {
            var t = type.GetNotNullableType();

            List<string> exitList = new List<string>();

            foreach (string enumName in Enum.GetNames(t))
            {
                Enum enumObj = (Enum)enumName.ParseTo(t);

                var label = enumObj.GetDisplayName() ?? enumObj.GetDescription() ?? enumObj.GetName();

                exitList.Add(label);
            }

            return exitList;
        }

        /// <summary>
        ///     Return array pair: value (Enum Name) and label (Display Name or Description
        ///     Attribute) of Enum Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <remarks> The method support both: Enum and Nullable Enum Type </remarks>
        public static object[] GetEnumValueLabelPair(this Type type)
        {
            var t = type.GetNotNullableType();

            var values = Enum.GetNames(t).Cast<object>().ToArray();

            var labels = t.GetListLabel().Cast<object>().ToArray();

            var result = new List<object>();

            if (type.IsNullableEnumType())
            {
                result.Add(new
                {
                    value = DataConst.Null,
                    label = string.Empty
                });
            }

            for (var x = 0; x <= values.Length - 1; x++)
            {
                result.Add(new
                {
                    value = values[x],
                    label = labels[x]
                });
            }

            return result.ToArray<object>();
        }
    }
}