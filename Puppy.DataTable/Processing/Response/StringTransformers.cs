using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Puppy.DataTable.Processing.Response
{
    public static class StringTransformers
    {
        internal static object GetStringedValue(Type propertyType, object value)
        {
            return Transformers.ContainsKey(propertyType)
                ? Transformers[propertyType](propertyType, value)
                : (value ?? string.Empty).ToString();
        }

        static StringTransformers()
        {
            // TODO Config DateTime Format in Config Object with appsetting.json
            RegisterFilter<DateTimeOffset>(dateTimeOffset => dateTimeOffset.ToLocalTime().ToString("g"));
            RegisterFilter<DateTime>(dateTime => dateTime.ToLocalTime().ToString("g"));
            RegisterFilter<IEnumerable<string>>(s => s.ToArray());
            RegisterFilter<IEnumerable<int>>(s => s.ToArray());
            RegisterFilter<IEnumerable<long>>(s => s.ToArray());
            RegisterFilter<IEnumerable<decimal>>(s => s.ToArray());
            RegisterFilter<IEnumerable<bool>>(s => s.ToArray());
            RegisterFilter<IEnumerable<double>>(s => s.ToArray());
            RegisterFilter<IEnumerable<object>>(s => s.Select(o => GetStringedValue(o.GetType(), o)).ToArray());
            RegisterFilter<bool>(s => s);
            RegisterFilter<object>(o => (o ?? string.Empty).ToString());
        }

        private static readonly Dictionary<Type, StringTransformer> Transformers = new Dictionary<Type, StringTransformer>();

        public delegate object GuardedValueTransformer<in TVal>(TVal value);

        public delegate object StringTransformer(Type type, object value);

        public static void RegisterFilter<TVal>(GuardedValueTransformer<TVal> filter)
        {
            if (Transformers.ContainsKey(typeof(TVal)))
                Transformers[typeof(TVal)] = Guard(filter);
            else
                Transformers.Add(typeof(TVal), Guard(filter));
        }

        private static StringTransformer Guard<TVal>(GuardedValueTransformer<TVal> transformer)
        {
            return (t, v) => !typeof(TVal).GetTypeInfo().IsAssignableFrom(t) ? null : transformer((TVal)v);
        }

        public static Dictionary<string, object> StringifyValues(Dictionary<string, object> dict)
        {
            var output = new Dictionary<string, object>();
            foreach (var row in dict)
            {
                output[row.Key] = row.Value == null ? string.Empty : GetStringedValue(row.Value.GetType(), row.Value);
            }
            return output;
        }
    }
}