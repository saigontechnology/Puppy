#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Framework.DependencyInjection </Project>
//     <File>
//         <Name> AssemblyLoader </Name>
//         <Created> 30 Mar 17 10:31:40 PM </Created>
//         <Key> 04c7f83a-d08e-4da2-8b6e-ed371ec0c190 </Key>
//     </File>
//     <Summary>
//         AssemblyLoader
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.DependencyModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using TopCore.Framework.DependencyInjection.Exceptions;

namespace TopCore.Framework.DependencyInjection
{
    public class AssemblyLoader : AssemblyLoadContext
    {
        public readonly string FolderFullPath;

        public List<AssemblyName> ListLoaddedAssemblyName { get; } = new List<AssemblyName>();

        public AssemblyLoader(string folderFullPath = null)
        {
            FolderFullPath = folderFullPath;

            // Update List Loadded Assembly
            var runtimeId = RuntimeEnvironment.GetRuntimeIdentifier();
            var listLoaddedAssemblyName = DependencyContext.Default.GetRuntimeAssemblyNames(runtimeId);

            foreach (var assemblyName in listLoaddedAssemblyName)
            {
                ListLoaddedAssemblyName.Add(assemblyName);
            }
        }

        /// <summary>
        /// Load an assembly, if the assembly already loaded then return null
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        protected override Assembly Load(AssemblyName assemblyName)
        {
            Assembly assembly;

            // Check if assembly already added by Dependency (Reference)
            if (ListLoaddedAssemblyName.Any(x => x.Name.ToLower() == assemblyName.Name.ToLower()))
            {
                return null;
            }

            // Load Assembly not yet load
            var assemblyFileInfo = new FileInfo($"{FolderFullPath}{Path.DirectorySeparatorChar}{assemblyName.Name}.dll");

            if (File.Exists(assemblyFileInfo.FullName))
            {
                var assemblyLoader = new AssemblyLoader(assemblyFileInfo.DirectoryName);
                assembly = assemblyLoader.LoadFromAssemblyPath(assemblyFileInfo.FullName);
            }
            else
            {
                assembly = Assembly.Load(assemblyName);
            }

            // Add to loadded
            ListLoaddedAssemblyName.Add(assembly.GetName());

            return assembly;
        }
    }
}