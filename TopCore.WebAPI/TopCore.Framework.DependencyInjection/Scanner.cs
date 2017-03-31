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
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using TopCore.Framework.DependencyInjection.Attributes;

namespace TopCore.Framework.DependencyInjection
{
    public class Scanner
    {
        public readonly AssemblyLoader AssemblyLoader;
        public readonly AssemblyName ThisAssemblyName;

        public Scanner(string folderFullPath = null)
        {
            AssemblyLoader = new AssemblyLoader(folderFullPath);
            ThisAssemblyName = new AssemblyName(GetType().GetTypeInfo().Assembly.FullName);
        }

        public void RegisterAssembly(IServiceCollection services, AssemblyName assemblyName)
        {
            try
            {
                Assembly assembly = AssemblyLoader.LoadFromAssemblyName(assemblyName);

                if (assembly == null)
                {
                    return;
                }

                foreach (var typeInfo in assembly.DefinedTypes)
                {
                    foreach (var customAttribute in typeInfo.GetCustomAttributes())
                    {
                        Type customAttributeType = customAttribute.GetType();

                        bool isDependencyAttribute = typeof(DependencyAttribute).IsAssignableFrom(customAttributeType);

                        if (!isDependencyAttribute)
                        {
                            continue;
                        }
                        var serviceDescriptor = ((DependencyAttribute)customAttribute).BuildServiceDescriptor(typeInfo);
                        services.Add(serviceDescriptor);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Register {assemblyName.Name} is exception, skipped. {ex}", $"{nameof(Scanner)}.{nameof(RegisterAssembly)}");
            }
        }

        /// <summary>
        ///     Auto Register all assemblies 
        /// </summary>
        /// <param name="services">      </param>
        /// <param name="searchPattern">  Search Pattern by Directory.GetFiles </param>
        /// <param name="folderFullPath"> Default is null = current execute application folder </param>
        public void RegisterAllAssemblies(IServiceCollection services, string searchPattern = "*.dll", string folderFullPath = null)
        {
            if (string.IsNullOrWhiteSpace(folderFullPath) || !File.Exists(folderFullPath))
            {
                folderFullPath = Path.GetFullPath(PlatformServices.Default.Application.ApplicationBasePath);
            }

            List<string> listDllFileFullPath = Directory.GetFiles(folderFullPath, searchPattern).ToList();

            foreach (var dllFileFullPath in listDllFileFullPath)
            {
                string dllNameWithoutExtension = Path.GetFileNameWithoutExtension(dllFileFullPath);
                RegisterAssembly(services, new AssemblyName(dllNameWithoutExtension));
            }
        }
    }
}