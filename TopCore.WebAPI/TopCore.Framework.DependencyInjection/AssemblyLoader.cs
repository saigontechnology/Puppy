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

using Microsoft.Extensions.DependencyModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace TopCore.Framework.DependencyInjection
{
    public class AssemblyLoader : AssemblyLoadContext
    {
        public readonly string FolderFullPath;

        public AssemblyLoader(string folderFullPath = null)
        {
            FolderFullPath = folderFullPath ?? Path.GetDirectoryName(typeof(Scanner).GetTypeInfo().Assembly.Location);
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var dependencyContext = DependencyContext.Default;
            var compileLibrary = dependencyContext.CompileLibraries.FirstOrDefault(x => x.Name.ToLower().Contains(assemblyName.Name.ToLower()));
            if (compileLibrary != null )
            {
                //return Assembly.Load(new AssemblyName(runtimeLibraries.First().Name));
                return compileLibrary.GetType().GetTypeInfo().Assembly;
            }
            var applicationFileInfo = new FileInfo($"{FolderFullPath}{Path.DirectorySeparatorChar}{assemblyName.Name}.dll");

            if (!File.Exists(applicationFileInfo.FullName))
            {
                return Assembly.Load(assemblyName);
            }
            var assemblyLoader = new AssemblyLoader(applicationFileInfo.DirectoryName);
            return assemblyLoader.LoadFromAssemblyPath(applicationFileInfo.FullName);
        }
    }
}