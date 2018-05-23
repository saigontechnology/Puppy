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
using Puppy.Core.EnvironmentUtils;
using System;

namespace Puppy.Core.ConfigUtils
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
        public static T GetSection<T>(this IConfiguration configuration, string key = null) where T : new()
        {
            if (string.IsNullOrWhiteSpace(key))
                key = typeof(T).Name;

            var value = new T();
            configuration.GetSection(key).Bind(value);
            return value;
        }

        /// <summary>
        ///     Production, Staging will read from section:"Environment Name", else by MachineName
        ///     (if not have data read from section:"Environment Name" key)
        /// </summary>
        /// <typeparam name="T"> The type to convert the value to. </typeparam>
        /// <param name="configuration"></param>
        /// <param name="section">       The configuration section for the value to convert. </param>
        /// <returns></returns>
        public static T GetValueByMachineAndEnv<T>(this IConfiguration configuration, string section)
        {
            if (string.IsNullOrWhiteSpace(section))
            {
                throw new ArgumentException($"{nameof(section)} cannot be null or empty", nameof(section));
            }

            T value;

            if (EnvironmentHelper.IsProduction() || EnvironmentHelper.IsStaging())
            {
                value = configuration.GetValue<T>($"{section}:{EnvironmentHelper.Name}");
            }
            else
            {
                var environmentName = !string.IsNullOrWhiteSpace(EnvironmentHelper.Name) ? EnvironmentHelper.Name : EnvironmentHelper.Development;

                var defaultValue = configuration.GetValue<T>($"{section}:{environmentName}");

                value = configuration.GetValue($"{section}:{EnvironmentHelper.MachineName}", defaultValue);
            }

            return value;
        }
    }
}