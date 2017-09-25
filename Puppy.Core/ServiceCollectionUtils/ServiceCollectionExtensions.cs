#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ServiceCollectionExtensions.cs </Name>
//         <Created> 11/08/17 11:48:11 PM </Created>
//         <Key> ad2f517d-0029-4570-a296-cc59d6d1a035 </Key>
//     </File>
//     <Summary>
//         ServiceCollectionExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Puppy.Core.ServiceCollectionUtils
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Removes(this IServiceCollection services, params Type[] removeTypes)
        {
            foreach (var removeType in removeTypes)
            {
                var removeTypeFound = services.FirstOrDefault(x => x.ServiceType == removeType);

                if (removeTypeFound != null)
                {
                    services.Remove(removeTypeFound);
                }
            }

            return services;
        }

        // Is Exist
        public static bool IsAdded<TService>(this IServiceCollection services) where TService : class
        {
            bool isExist = services.All(x => x.ServiceType == typeof(TService));
            return isExist;
        }

        // If Any

        public static IServiceCollection AddScopedIfAny<TService, TImplementation>(this IServiceCollection services, Func<ServiceDescriptor, bool> predicate) where TService : class where TImplementation : class, TService
        {
            if (services.Any(predicate))
            {
                services.AddScoped<TService, TImplementation>();
            }
            return services;
        }

        public static IServiceCollection AddTransientIfAny<TService, TImplementation>(this IServiceCollection services, Func<ServiceDescriptor, bool> predicate) where TService : class where TImplementation : class, TService
        {
            if (services.Any(predicate))
            {
                services.AddTransient<TService, TImplementation>();
            }
            return services;
        }

        public static IServiceCollection AddSingletonIfAny<TService, TImplementation>(this IServiceCollection services, Func<ServiceDescriptor, bool> predicate) where TService : class where TImplementation : class, TService
        {
            if (services.Any(predicate))
            {
                services.AddSingleton<TService, TImplementation>();
            }
            return services;
        }

        // If All

        public static IServiceCollection AddScopedIfAll<TService, TImplementation>(this IServiceCollection services, Func<ServiceDescriptor, bool> predicate) where TService : class where TImplementation : class, TService
        {
            if (services.All(predicate))
            {
                services.AddScoped<TService, TImplementation>();
            }
            return services;
        }

        public static IServiceCollection AddTransientIfAll<TService, TImplementation>(this IServiceCollection services, Func<ServiceDescriptor, bool> predicate) where TService : class where TImplementation : class, TService
        {
            if (services.All(predicate))
            {
                services.AddTransient<TService, TImplementation>();
            }
            return services;
        }

        public static IServiceCollection AddSingletonIfAll<TService, TImplementation>(this IServiceCollection services, Func<ServiceDescriptor, bool> predicate) where TService : class where TImplementation : class, TService
        {
            if (services.All(predicate))
            {
                services.AddSingleton<TService, TImplementation>();
            }
            return services;
        }

        // If Not Exist

        public static IServiceCollection AddScopedIfNotExist<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
        {
            services.AddScopedIfAll<TService, TImplementation>(x => x.ServiceType != typeof(TService));
            return services;
        }

        public static IServiceCollection AddTransientIfNotExist<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
        {
            services.AddTransientIfAll<TService, TImplementation>(x => x.ServiceType != typeof(TService));
            return services;
        }

        public static IServiceCollection AddSingletonIfNotExist<TService, TImplementation>(this IServiceCollection services) where TService : class where TImplementation : class, TService
        {
            services.AddSingletonIfAll<TService, TImplementation>(x => x.ServiceType != typeof(TService));
            return services;
        }
    }
}