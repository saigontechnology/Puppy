#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> DictionaryExtensions.cs </Name>
//         <Created> 02/09/17 10:39:31 PM </Created>
//         <Key> 77cee309-c2d8-45b3-aa5c-9251ed3f66cd </Key>
//     </File>
//     <Summary>
//         DictionaryExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;
using Puppy.Core.ObjectUtils;
using System.Collections.Generic;

namespace Puppy.Core.DictionaryUtils
{
    public static class DictionaryExtensions
    {
        /// <summary>
        ///     Safe set value to dictionary 
        /// </summary>
        /// <remarks> Update value if already exists, else is add </remarks>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="key">   </param>
        /// <param name="data">  </param>
        /// <exception cref="ArgumentNullException"> <paramref name="key" /> is null. </exception>
        /// <exception cref="KeyNotFoundException">
        ///     The property is retrieved and <paramref name="key" /> is not found.
        /// </exception>
        /// <exception cref="NotSupportedException">
        ///     The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2" />
        ///     is read-only.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </exception>
        public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue data)
        {
            if (source.ContainsKey(key))
            {
                source[key] = data;
            }
            else
            {
                source.Add(key, data);
            }
        }

        public static T GetValue<T>(this IDictionary<string, object> source, string key)
        {
            return source.TryGetValue(key, out var data) ? data.ConvertTo<T>() : default(T);
        }

        public static T GetValue<T>(this IDictionary<string, object> source, string key, T defaultValue)
        {
            if (!source.TryGetValue(key, out var data))
            {
                return defaultValue;
            }

            var result = data.ConvertTo<T>();

            return Equals(result, default(T)) ? defaultValue : result;
        }
    }
}