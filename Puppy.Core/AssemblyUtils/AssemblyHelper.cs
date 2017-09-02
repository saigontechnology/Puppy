#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> AssemblyHelper.cs </Name>
//         <Created> 01/08/17 12:07:12 AM </Created>
//         <Key> 4224e24e-8af9-43b7-b56a-2274afc014ae </Key>
//     </File>
//     <Summary>
//         AssemblyHelper.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;

namespace Puppy.Core.AssemblyUtils
{
    public static class AssemblyHelper
    {
        /// <summary>
        ///     Get net framework production name 
        /// </summary>
        public static readonly string NetFrameworkProductName = GetNetFrameworkProductName();

        public static List<Assembly> ListLoadedAssembly { get; } = GetRuntimeAssemblies();

        public static List<Assembly> GetRuntimeAssemblies()
        {
            var runtimeId = RuntimeEnvironment.GetRuntimeIdentifier();
            var assemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(runtimeId);
            return assemblyNames.Select(Assembly.Load).ToList();
        }

        /// <summary>
        ///     Get assembly name in <paramref name="path" /> 
        /// </summary>
        /// <param name="path">       </param>
        /// <param name="skipOnError"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAssemblyNames(string path, bool skipOnError = true)
        {
            try
            {
                return Directory.EnumerateFiles(path, "*.dll").Concat(Directory.EnumerateFiles(path, "*.exe"));
            }
            catch (Exception e)
            {
                if (!(skipOnError && (e is DirectoryNotFoundException || e is IOException || e is SecurityException || e is UnauthorizedAccessException)))
                {
                    throw;
                }

                return new string[0];
            }
        }

        /// <summary>
        ///     Get .net framework production name 
        /// </summary>
        /// <returns></returns>
        private static string GetNetFrameworkProductName()
        {
            var productAttribute = typeof(object).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyProductAttribute>();
            return productAttribute?.Product;
        }

        /// <summary>
        ///     Check <paramref name="assembly" /> is assembly or not 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static bool IsSystemAssembly(Assembly assembly)
        {
            if (NetFrameworkProductName == null) return false;
            var productAttribute = assembly.GetCustomAttribute<AssemblyProductAttribute>();
            return productAttribute != null && string.Compare(NetFrameworkProductName, productAttribute.Product, StringComparison.Ordinal) == 0;
        }

        /// <summary>
        ///     Load assembly by <paramref name="assemblyName" /> 
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="skipOnError"> </param>
        /// <returns></returns>
        public static Assembly LoadAssembly(string assemblyName, bool skipOnError = true)
        {
            try
            {
                return Assembly.Load(new AssemblyName(assemblyName));
            }
            catch (Exception e)
            {
                if (!(skipOnError && (e is FileNotFoundException || e is FileLoadException || e is BadImageFormatException)))
                {
                    throw;
                }

                return null;
            }
        }

        public static Assembly Load(AssemblyName assemblyName)
        {
            Assembly assembly = ListLoadedAssembly.FirstOrDefault(x => string.Equals(x.GetName().Name, assemblyName.Name, StringComparison.CurrentCultureIgnoreCase));

            if (assembly != null) return assembly;

            assembly = Assembly.Load(assemblyName);
            ListLoadedAssembly.Add(assembly);

            return assembly;
        }
    }
}