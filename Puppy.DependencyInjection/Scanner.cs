#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> Puppy.DependencyInjection </Project>
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
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.PlatformAbstractions;
using Puppy.DependencyInjection.Attributes;
using Puppy.DependencyInjection.Exceptions;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Puppy.DependencyInjection
{
    public class Scanner
    {
        public AssemblyLoader AssemblyLoader;

        public Scanner()
        {
            AssemblyLoader = new AssemblyLoader();
        }

        /// <summary>
        ///     Register Assembly by Name
        /// </summary>
        /// <param name="services">    </param>
        /// <param name="assemblyName"></param>
        public void RegisterAssembly(IServiceCollection services, AssemblyName assemblyName)
        {
            var assembly = AssemblyLoader.LoadFromAssemblyName(assemblyName);

            foreach (var typeInfo in assembly.DefinedTypes)
                foreach (var customAttribute in typeInfo.GetCustomAttributes())
                {
                    var customAttributeType = customAttribute.GetType();

                    var isDependencyAttribute = typeof(DependencyAttribute).IsAssignableFrom(customAttributeType);

                    if (!isDependencyAttribute)
                    {
                        continue;
                    }

                    var serviceDescriptor = ((DependencyAttribute)customAttribute).BuildServiceDescriptor(typeInfo);

                    // Check is service already register from difference implementation => throw exception

                    var isAlreadyDifferenceImplementation = services.Any(
                        x =>
                            x.ServiceType.FullName == serviceDescriptor.ServiceType.FullName &&
                            x.ImplementationType != serviceDescriptor.ImplementationType);

                    if (isAlreadyDifferenceImplementation)
                    {
                        var implementationRegister =
                            services
                                .Single(x => x.ServiceType.FullName == serviceDescriptor.ServiceType.FullName)
                                .ImplementationType;

                        throw new ConflictRegistrationException($"Conflict register, ${serviceDescriptor.ImplementationType} try to register for {serviceDescriptor.ServiceType.FullName}. It already register by {implementationRegister.FullName} before.");
                    }

                    // Check is service already register from same implementation => remove existing,
                    // replace by new one life time cycle

                    var isAlreadySameImplementation = services.Any(
                        x =>
                            x.ServiceType.FullName == serviceDescriptor.ServiceType.FullName &&
                            x.ImplementationType == serviceDescriptor.ImplementationType);

                    if (isAlreadySameImplementation)
                    {
                        services = services.Replace(serviceDescriptor);
                    }
                    else
                    {
                        services.Add(serviceDescriptor);
                    }
                }
        }

        /// <summary>
        ///     Register all assemblies
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

            // Update assembly loader with folder path
            AssemblyLoader = new AssemblyLoader(folderFullPath);

            var listDllFileFullPath = Directory.GetFiles(folderFullPath, searchPattern).ToList();

            foreach (var dllFileFullPath in listDllFileFullPath)
            {
                var dllNameWithoutExtension = Path.GetFileNameWithoutExtension(dllFileFullPath);
                RegisterAssembly(services, new AssemblyName(dllNameWithoutExtension));
            }
        }

        /// <summary>
        ///     Write registered service information to Console
        /// </summary>
        /// <param name="services">         </param>
        /// <param name="serviceNameFilter"> null to get all </param>
        public void WriteOut(IServiceCollection services, string serviceNameFilter = null)
        {
            var listServiceDescriptors = services.ToList();

            if (!string.IsNullOrWhiteSpace(serviceNameFilter))
            {
                listServiceDescriptors =
                    listServiceDescriptors
                        .Where(x => x.ServiceType.FullName.Contains(serviceNameFilter))
                        .ToList();
            }

            var consoleTextColor = ConsoleColor.Yellow;
            var consoleDimColor = ConsoleColor.DarkGray;

            Console.WriteLine();
            Console.WriteLine($"{new string('-', 110)}");
            Console.WriteLine();

            Console.ForegroundColor = consoleTextColor;
            Console.WriteLine($"Puppy DI > Registered {listServiceDescriptors.Count} Services.");
            Console.WriteLine();

            var noMaxLength = listServiceDescriptors.Count.ToString().Length;

            if (noMaxLength < "No.".Length)
            {
                noMaxLength = "No.".Length;
            }

            var serviceNameMaxLength = listServiceDescriptors.Select(x => GetNormForServiceAdded(x.ServiceType.Name).Length).Max();
            if (serviceNameMaxLength < "Service".Length)
            {
                serviceNameMaxLength = "Service".Length;
            }

            var implementationNameMaxLength = listServiceDescriptors.Select(x => GetNormForServiceAdded(x.ImplementationType?.Name).Length).Max();
            if (implementationNameMaxLength < "Implementation".Length)
            {
                implementationNameMaxLength = "Implementation".Length;
            }

            var lifeTimeMaxLength = listServiceDescriptors.Select(x => x.Lifetime.ToString().Length).Max();
            if (lifeTimeMaxLength < "Lifetime".Length)
            {
                lifeTimeMaxLength = "Lifetime".Length;
            }

            // Header

            Console.ResetColor();
            Console.Write("    ");
            Console.Write("No.".PadRight(noMaxLength));
            Console.Write("    |    ");
            Console.Write("Service".PadRight(serviceNameMaxLength));
            Console.Write("    |    ");
            Console.Write("Implementation".PadRight(implementationNameMaxLength));
            Console.Write("    |    ");
            Console.Write("Lifetime");
            Console.WriteLine();

            Console.WriteLine($"{new string('-', 4 + noMaxLength + serviceNameMaxLength + implementationNameMaxLength + lifeTimeMaxLength + "    |    ".Length * 3)}");

            for (var index = 0; index < listServiceDescriptors.Count; index++)
            {
                var service = listServiceDescriptors[index];

                var no = index + 1;

                // No

                Console.ResetColor();
                Console.Write("    ");
                Console.Write($"{no}".PadRight(noMaxLength));

                Console.ResetColor();
                Console.Write("    |    ");

                // Service

                Console.ForegroundColor = consoleTextColor;
                Console.Write(GetNormForServiceAdded(service.ServiceType?.Name).PadRight(serviceNameMaxLength));

                Console.ResetColor();
                Console.Write("    |    ");

                // Implementation

                Console.ForegroundColor = consoleDimColor;
                Console.Write(GetNormForServiceAdded(service.ImplementationType?.Name).PadRight(implementationNameMaxLength));

                Console.ResetColor();
                Console.Write("    |    ");

                // Lifetime

                Console.ForegroundColor = consoleTextColor;
                Console.WriteLine(service.Lifetime.ToString().PadRight(lifeTimeMaxLength));
            }

            Console.WriteLine();
            Console.ResetColor();
            Console.WriteLine($"{new string('-', 110)}");
        }

        private static string GetNormForServiceAdded(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "-";
            }

            // Replace To Readable Generic if can
            return Regex.Replace(value, @"`\d", "<T>");
        }
    }
}