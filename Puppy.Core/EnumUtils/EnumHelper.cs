#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> EnumHelper.cs </Name>
//         <Created> 12/09/17 5:09:19 PM </Created>
//         <Key> dd553a3a-a1b6-4013-b524-8168099531cf </Key>
//     </File>
//     <Summary>
//         EnumHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Puppy.Core.StringUtils;
using Puppy.Core.TypeUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Puppy.Core.EnumUtils
{
    public static class EnumHelper
    {
        public static string GetDisplayName(this Enum value)
        {
            Type enumType = value.GetType();

            var enumValue = Enum.GetName(enumType, value);

            MemberInfo member = enumType.GetMember(enumValue).FirstOrDefault();

            if (!(member?.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() is DisplayAttribute displayAttribute))
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

            if (!string.IsNullOrWhiteSpace(enumValue))
            {
                return enumValue;
            }

            MemberInfo member = enumType.GetMember(enumValue).FirstOrDefault();

            if (!(member?.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() is DescriptionAttribute descriptionAttribute))
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

        /// <summary>
        ///     Get Enum Label (Display Name ?? Description ?? Name) 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetLabel(this Enum value)
        {
            return value.GetDisplayName() ?? value.GetDescription() ?? value.GetName();
        }

        public static List<string> GetListLabel(this Type type)
        {
            var t = type.GetNotNullableType();

            List<string> exitList = new List<string>();

            foreach (string enumName in Enum.GetNames(t))
            {
                Enum enumValue = (Enum)enumName.ParseTo(t);

                var label = enumValue.GetLabel();

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
        /// <remarks> The method support both: Enum and Nullable Enum Type. </remarks>
        public static EnumValueLabelModel[] GetEnumValueLabelPair(this Type type)
        {
            var t = type.GetNotNullableType();

            var values = Enum.GetNames(t).Cast<object>().ToArray();

            var labels = t.GetListLabel().Cast<object>().ToArray();

            var result = new List<EnumValueLabelModel>();

            if (type.IsNullableEnumType())
            {
                result.Add(new EnumValueLabelModel
                {
                    Value = null,
                    Label = string.Empty
                });
            }

            for (var x = 0; x <= values.Length - 1; x++)
            {
                result.Add(new EnumValueLabelModel
                {
                    Value = values[x].ToString(),
                    Label = labels[x].ToString()
                });
            }

            return result.ToArray();
        }

        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }

    // ReSharper disable InconsistentNaming
    public class EnumValueLabelModel
    {
        public string Value { get; set; }

        public string Label { get; set; }
    }
}