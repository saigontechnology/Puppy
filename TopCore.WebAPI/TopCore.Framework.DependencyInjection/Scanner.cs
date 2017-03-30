#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Framework.DependencyInjection </Project>
//     <File>
//         <Name> Scanner </Name>
//         <Created> 30 Mar 17 9:03:01 PM </Created>
//         <Key> 979b8ce8-0ebe-41e8-bba7-0540f898ed08 </Key>
//     </File>
//     <Summary>
//         Scanner
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TopCore.Framework.DependencyInjection.Attributes;

namespace TopCore.Framework.DependencyInjection
{
    public class Scanner
    {
        public readonly AssemblyLoader AssemblyLoader;

        public Scanner(string folderFullPath = null)
        {
            AssemblyLoader = new AssemblyLoader(folderFullPath);
        }

        public void RegisterAssembly(IServiceCollection services, AssemblyName assemblyName)
        {
            Assembly assembly = AssemblyLoader.LoadFromAssemblyName(assemblyName);

            foreach (var typeInfo in assembly.DefinedTypes)
            {
                var listDependencyAttribute = typeInfo.GetCustomAttributes<DependencyAttribute>();

                // Each dependency can be registered as various types
                foreach (var dependencyAttribute in listDependencyAttribute)
                {
                    var serviceDescriptor = dependencyAttribute.BuildServiceDescriptor(typeInfo);
                    services.Add(serviceDescriptor);
                }
            }
        }

        public void RegisterAllAssemblies(IServiceCollection services)
        {
            // all assemblies in assemblies resolver folder
            string folderPath = Path.GetDirectoryName(typeof(Scanner).GetTypeInfo().Assembly.Location);
            ICollection<string> listDllFileFullPath = Directory.GetFiles(folderPath, "*.dll");

            foreach (string dllFileFullPath in listDllFileFullPath)
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(dllFileFullPath);
                AssemblyName assemblyName = new AssemblyName(fileNameWithoutExtension);
                RegisterAssembly(services, assemblyName);
            }
        }
    }
}