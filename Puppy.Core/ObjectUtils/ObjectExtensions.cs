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
using System.Reflection;

namespace Puppy.Core.ObjectUtils
{
    public static class ObjectExtensions
    {
        private const string ExpressionCannotBeNullMessage = "The expression cannot be null.";

        private const string InvalidExpressionMessage = "Invalid expression.";

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
            return GetMemberName(expression.Body);
        }

        public static List<string> GetMemberNames<T>(this T instance, params Expression<Func<T, object>>[] expressions)
        {
            List<string> memberNames = new List<string>();
            foreach (var cExpression in expressions)
            {
                memberNames.Add(GetMemberName(cExpression.Body));
            }
            return memberNames;
        }

        public static string GetMemberName<T>(this T instance, Expression<Action<T>> expression)
        {
            return GetMemberName(expression.Body);
        }

        private static string GetMemberName(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentException(ExpressionCannotBeNullMessage);
            }
            if (expression is MemberExpression memberExpression)
            {
                // Reference type property or field
                return memberExpression.Member.Name;
            }
            if (expression is MethodCallExpression methodCallExpression)
            {
                // Reference type method
                return methodCallExpression.Method.Name;
            }
            if (expression is UnaryExpression unaryExpression)
            {
                // Property, field of method returning value type
                return GetMemberName(unaryExpression);
            }
            throw new ArgumentException(InvalidExpressionMessage);
        }

        private static string GetMemberName(UnaryExpression unaryExpression)
        {
            if (unaryExpression.Operand is MethodCallExpression methodExpression)
            {
                return methodExpression.Method.Name;
            }
            return ((MemberExpression)unaryExpression.Operand).Member.Name;
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
                Type t = typeof(T);

                Type u = Nullable.GetUnderlyingType(t);

                if (u != null)
                {
                    return (obj == null) ? default(T) : (T)Convert.ChangeType(obj, u);
                }
                return (T)Convert.ChangeType(obj, t);
            }
            catch
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

        public static IDictionary<string, object> ToDictionary(this object obj)
        {
            var directory = new Dictionary<string, object>();

            foreach (var propertyInfo in obj.GetType().GetProperties())
            {
                directory[propertyInfo.Name] = propertyInfo.GetValue(obj);
            }
            return directory;
        }
    }
}