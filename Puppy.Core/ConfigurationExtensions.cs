#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ConfigurationExtensions.cs </Name>
//         <Created> 07/06/2017 9:49:12 PM </Created>
//         <Key> c19ed78a-7097-4afd-88d5-ea587f568843 </Key>
//     </File>
//     <Summary>
//         ConfigurationExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.Extensions.Configuration;
using System;

namespace Puppy.Core
{
    /// <summary>
    ///     <see cref="IConfiguration" /> 
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        ///     Gets a configuration sub-section with the specified key and binds it with the
        ///     specified type.
        /// </summary>
        /// <typeparam name="T"> The type of the configuration section to bind to. </typeparam>
        /// <param name="configuration"> The configuration. </param>
        /// <param name="key">          
        ///     The section key. If <c> null </c>, the name of the type <typeparamref name="T" /> is used.
        /// </param>
        /// <returns> The bound object. </returns>
        public static T GetSection<T>(this IConfiguration configuration, string key = null)
            where T : new()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (key == null)
            {
                key = typeof(T).Name;
            }

            var section = new T();
            configuration.GetSection(key).Bind(section);
            return section;
        }
    }
}