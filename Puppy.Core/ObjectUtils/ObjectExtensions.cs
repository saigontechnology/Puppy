#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ObjectExtensions.cs </Name>
//         <Created> 16/08/17 6:08:02 PM </Created>
//         <Key> b207be99-bed7-45d7-8d0b-c26690d5444b </Key>
//     </File>
//     <Summary>
//         ObjectExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Puppy.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Puppy.Core.ObjectUtils
{
    public static class ObjectExtensions
    {
        public static object GetPropertyValue<T>(this T instance, string propertyName)
        {
            var targetProperty = instance.GetType().GetProperty(propertyName);
            return targetProperty?.GetValue(instance);
        }

        public static void SetProperty<T>(this T entityToSet, string propertyName, object value)
        {
            var targetProperty = entityToSet.GetType().GetProperty(propertyName);
            targetProperty?.SetValue(entityToSet, value, null);
        }

        public static string GetMemberName<T>(this T instance, Expression<Func<T, object>> expression)
        {
            return ObjectHelper.GetMemberName(expression);
        }

        public static List<string> GetMemberNames<T>(this T instance, params Expression<Func<T, object>>[] expressions)
        {
            List<string> memberNames = new List<string>();
            foreach (var expression in expressions)
            {
                memberNames.Add(ObjectHelper.GetMemberName(expression));
            }
            return memberNames;
        }

        public static string GetMemberName<T>(this T instance, Expression<Action<T>> expression)
        {
            return ObjectHelper.GetMemberName(expression);
        }

        /// <summary>
        ///     Perform a deep Copy of the object. 
        /// </summary>
        /// <typeparam name="T"> The type of object being copied. </typeparam>
        /// <param name="source"> The object instance to copy. </param>
        /// <returns> The copied object. </returns>
        public static T Clone<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null))
            {
                return default(T);
            }

            // Initialize inner objects individually for example in default constructor some list
            // property initialized with some values, but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deSerializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deSerializeSettings);
        }

        public static T ConvertTo<T>(this object obj)
        {
            try
            {
                if (obj == null)
                {
                    return default(T);
                }

                if (obj is T variable)
                {
                    return variable;
                }

                Type t = typeof(T);
                Type u = Nullable.GetUnderlyingType(t);

                if (u != null)
                {
                    if (u == typeof(string))
                    {
                        return (T)(object)obj.ToString();
                    }

                    return (T)Convert.ChangeType(obj, u);
                }

                if (t == typeof(string))
                {
                    return (T)((object)obj.ToString());
                }

                if (t.IsPrimitive)
                {
                    return (T)Convert.ChangeType(obj.ToString(), t);
                }

                return (T)Convert.ChangeType(obj, t);
            }
            catch (InvalidCastException)
            {
                return default(T);
            }
        }

        public static string ToJsonString(this object obj)
        {
            if (obj is JObject jObject)
            {
                return jObject.ToString(StandardFormat.JsonSerializerSettings.Formatting);
            }
            return JsonConvert.SerializeObject(obj, StandardFormat.JsonSerializerSettings);
        }

        public static JObject ToJson(this object obj)
        {
            return JObject.FromObject(obj);
        }

        public static Dictionary<string, object> ToDictionary(this object obj)
        {
            JObject json = JObject.FromObject(obj);
            var properties = json.Properties();

            var directory = new Dictionary<string, object>();

            foreach (var property in properties)
            {
                directory[property.Name] = property.Value;
            }

            return directory;
        }

        public static T PreventReferenceLoop<T>(this T obj)
        {
            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(obj, null))
            {
                return default(T);
            }

            var json = JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            var result = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace
            });

            return result;
        }
    }
}