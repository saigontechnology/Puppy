#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> Puppy.DependencyInjection </Project>
//     <File>
//         <Name> Helper </Name>
//         <Created> 30 Mar 17 8:18:26 PM </Created>
//         <Key> 92d2515d-04f0-4d5f-a109-e7ba1655bb42 </Key>
//     </File>
//     <Summary>
//         Helper
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace Puppy.DependencyInjection
{
    // ReSharper disable once InconsistentNaming
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddDependencyInjectionScanner(this IServiceCollection services)
        {
            services.AddSingleton<Scanner>();
            return services;
        }

        /// <summary>
        ///     Register in self assembly 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection ScanFromSelf(this IServiceCollection services)
        {
            var env = services.BuildServiceProvider().GetService<IHostingEnvironment>();
            services.ScanFromAssembly(new AssemblyName(env.ApplicationName));
            return services;
        }

        /// <summary>
        ///     Register Assembly by Name 
        /// </summary>
        /// <param name="services">    </param>
        /// <param name="assemblyName"></param>
        public static IServiceCollection ScanFromAssembly(this IServiceCollection services, AssemblyName assemblyName)
        {
            var scanner = services.GetScanner();
            scanner.RegisterAssembly(services, assemblyName);
            return services;
        }

        /// <summary>
        ///     Register all assemblies 
        /// </summary>
        /// <param name="services">      </param>
        /// <param name="searchPattern">  Search Pattern by Directory.GetFiles </param>
        /// <param name="folderFullPath"> Default is null = current execute application folder </param>
        public static IServiceCollection ScanFromAllAssemblies(this IServiceCollection services,
            string searchPattern = "*.dll", string folderFullPath = null)
        {
            var scanner = services.GetScanner();
            scanner.RegisterAllAssemblies(services, searchPattern, folderFullPath);
            return services;
        }

        /// <summary>
        ///     Write registered service information to Console 
        /// </summary>
        /// <param name="services">             </param>
        /// <param name="serviceTypeNameFilter"> ServiceType.Name.Contains([filter string]) </param>
        public static IServiceCollection WriteOut(this IServiceCollection services, string serviceTypeNameFilter = null)
        {
            var scanner = services.GetScanner();
            scanner.WriteOut(services, serviceTypeNameFilter);
            return services;
        }

        private static Scanner GetScanner(this IServiceCollection services)
        {
            var scanner = services.BuildServiceProvider().GetService<Scanner>();
            if (scanner == null)
                throw new InvalidOperationException(
                    $"Unable to resolve {nameof(Scanner)}. Did you forget to call {nameof(services)}.{nameof(AddDependencyInjectionScanner)}?");
            return scanner;
        }

        public static T Resolve<T>(this IServiceCollection services) where T : class
        {
            return services.BuildServiceProvider().GetService<T>();
        }
    }
}