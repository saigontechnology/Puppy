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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Puppy.Core.StringUtils;
using Puppy.Core.TypeUtils;

namespace Puppy.DataTable.Utils.Reflection
{
    public static class EnumHelper
    {
        public static string GetDisplayNameOrDescription(this Enum value)
        {
            Type enumType = value.GetType();

            var enumValue = Enum.GetName(enumType, value);

            MemberInfo member = enumType.GetMember(enumValue)[0];

            if (member.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() is DisplayAttribute displayAttribute)
            {
                var displayName = displayAttribute.ResourceType != null ? displayAttribute.GetName() : displayAttribute.Name;

                if (!string.IsNullOrWhiteSpace(displayName))
                {
                    return displayName;
                }
            }

            if (member.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() is DescriptionAttribute descriptionAttribute)
            {
                var description = descriptionAttribute.Description;

                if (!string.IsNullOrWhiteSpace(description))
                {
                    return description;
                }
            }

            return member.Name;
        }

        public static List<string> GetListDisplayNameOrDescriptions(this Type type)
        {
            var t = type.GetNotNullableType();

            List<string> exitList = new List<string>();

            foreach (string enumName in Enum.GetNames(t))
            {
                exitList.Add(((Enum)enumName.ParseTo(t)).GetDisplayNameOrDescription());
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

            var labels = t.GetListDisplayNameOrDescriptions().Cast<object>().ToArray();

            var result = new List<object>();

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