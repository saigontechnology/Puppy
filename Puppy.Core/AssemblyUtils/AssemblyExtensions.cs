#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> AssemblyExtensions.cs </Name>
//         <Created> 01/08/17 12:07:02 AM </Created>
//         <Key> e755dd78-8127-4368-b392-824b61db935d </Key>
//     </File>
//     <Summary>
//         AssemblyExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Puppy.Core.AssemblyUtils
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<T> GetImplementationsOf<T>(this Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(t => t.GetTypeInfo().IsClass && !t.GetTypeInfo().IsAbstract && typeof(T).IsAssignableFrom(t))
                .ToList();

            return types.Select(type => (T)Activator.CreateInstance(type)).ToList();
        }

        public static IEnumerable<T> GetImplementationsOf<T>(this IEnumerable<Assembly> assemblies)
        {
            var result = new List<T>();

            foreach (var assembly in assemblies)
            {
                result.AddRange(assembly.GetImplementationsOf<T>());
            }

            return result;
        }

        public static string GetDirectoryPath(this Assembly assembly)
        {
            UriBuilder uri = new UriBuilder(assembly.CodeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}