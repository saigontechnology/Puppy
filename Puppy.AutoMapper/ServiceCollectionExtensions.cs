#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ServiceCollectionExtensions.cs </Name>
//         <Created> 25/07/17 12:14:13 AM </Created>
//         <Key> dce0afff-cecc-483f-bbf5-a0e9b60f5fd5 </Key>
//     </File>
//     <Summary>
//         ServiceCollectionExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Puppy.Core.TypeUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Puppy.AutoMapper
{
    public static class ServiceCollectionExtensions
    {
        private static readonly Action<IMapperConfigurationExpression> DefaultConfig = cfg => { };

        private static HashSet<string> ReferenceAssemblies { get; } =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "AutoMapper"
            };

        /// <summary>
        ///     Add AutoMapper with Profiles scan and Dependency Injection 
        /// </summary>
        /// <param name="services">                    </param>
        /// <param name="isAssertConfigurationIsValid">
        ///     Check all auto mapper profile is valid by <c> Mapper.AssertConfigurationIsValid(); </c>
        /// </param>
        /// <param name="isCompileMappings">           
        ///     AutoMapper lazily compiles the type map plans on first map. However, this behavior is
        ///     not always desirable, so you can tell AutoMapper to compile its mappings directly by
        ///     <c> Mapper.Configuration.CompileMappings(); </c>
        /// </param>
        /// <param name="assembliesToScan">            
        ///     List of assembliesToScan contain AutoMapper Profile, <c> null </c> for scan current
        ///     loaded/reference in main project
        /// </param>
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, bool isAssertConfigurationIsValid = true, bool isCompileMappings = true, params Assembly[] assembliesToScan)
        {
            return AddAutoMapperClasses(services, null, assembliesToScan, isAssertConfigurationIsValid, isCompileMappings);
        }

        /// <summary>
        ///     Add AutoMapper with Profiles scan and Dependency Injection 
        /// </summary>
        /// <param name="services">                    </param>
        /// <param name="isAssertConfigurationIsValid">
        ///     Check all auto mapper profile is valid by <c> Mapper.AssertConfigurationIsValid(); </c>
        /// </param>
        /// <param name="isCompileMappings">           
        ///     AutoMapper lazily compiles the type map plans on first map. However, this behavior is
        ///     not always desirable, so you can tell AutoMapper to compile its mappings directly by
        ///     <c> Mapper.Configuration.CompileMappings(); </c>
        /// </param>
        /// <returns></returns>
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, bool isAssertConfigurationIsValid = true, bool isCompileMappings = true)
        {
            return services.AddAutoMapper(null, DependencyContext.Default);
        }

        /// <summary>
        ///     Add AutoMapper with Profiles scan and Dependency Injection 
        /// </summary>
        /// <param name="services">                    </param>
        /// <param name="additionalInitAction">        </param>
        /// <param name="isAssertConfigurationIsValid">
        ///     Check all auto mapper profile is valid by <c> Mapper.AssertConfigurationIsValid(); </c>
        /// </param>
        /// <param name="isCompileMappings">           
        ///     AutoMapper lazily compiles the type map plans on first map. However, this behavior is
        ///     not always desirable, so you can tell AutoMapper to compile its mappings directly by
        ///     <c> Mapper.Configuration.CompileMappings(); </c>
        /// </param>
        /// <returns></returns>
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction, bool isAssertConfigurationIsValid = true, bool isCompileMappings = true)
        {
            return services.AddAutoMapper(additionalInitAction, DependencyContext.Default);
        }

        /// <summary>
        ///     Add AutoMapper with Profiles scan and Dependency Injection 
        /// </summary>
        /// <param name="services">                    </param>
        /// <param name="additionalInitAction">        </param>
        /// <param name="dependencyContext">           </param>
        /// <param name="isAssertConfigurationIsValid">
        ///     Check all auto mapper profile is valid by <c> Mapper.AssertConfigurationIsValid(); </c>
        /// </param>
        /// <param name="isCompileMappings">           
        ///     AutoMapper lazily compiles the type map plans on first map. However, this behavior is
        ///     not always desirable, so you can tell AutoMapper to compile its mappings directly by
        ///     <c> Mapper.Configuration.CompileMappings(); </c>
        /// </param>
        /// <returns></returns>
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction, DependencyContext dependencyContext, bool isAssertConfigurationIsValid = true, bool isCompileMappings = true)
        {
            return services.AddAutoMapper(additionalInitAction, GetCandidateAssemblies(dependencyContext));
        }

        /// <summary>
        ///     Add AutoMapper with Profiles scan and Dependency Injection 
        /// </summary>
        /// <param name="services">                    </param>
        /// <param name="additionalInitAction">        </param>
        /// <param name="isAssertConfigurationIsValid">
        ///     Check all auto mapper profile is valid by <c> Mapper.AssertConfigurationIsValid(); </c>
        /// </param>
        /// <param name="isCompileMappings">           
        ///     AutoMapper lazily compiles the type map plans on first map. However, this behavior is
        ///     not always desirable, so you can tell AutoMapper to compile its mappings directly by
        ///     <c> Mapper.Configuration.CompileMappings(); </c>
        /// </param>
        /// <param name="assemblies">                  </param>
        /// <returns></returns>
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction, bool isAssertConfigurationIsValid = true, bool isCompileMappings = true, params Assembly[] assemblies)
        {
            return AddAutoMapperClasses(services, additionalInitAction, assemblies, isAssertConfigurationIsValid, isCompileMappings);
        }

        /// <summary>
        ///     Add AutoMapper with Profiles scan and Dependency Injection 
        /// </summary>
        /// <param name="services">                    </param>
        /// <param name="additionalInitAction">        </param>
        /// <param name="assemblies">                  </param>
        /// <param name="isAssertConfigurationIsValid">
        ///     Check all auto mapper profile is valid by <c> Mapper.AssertConfigurationIsValid(); </c>
        /// </param>
        /// <param name="isCompileMappings">           
        ///     AutoMapper lazily compiles the type map plans on first map. However, this behavior is
        ///     not always desirable, so you can tell AutoMapper to compile its mappings directly by
        ///     <c> Mapper.Configuration.CompileMappings(); </c>
        /// </param>
        /// <returns></returns>
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction, IEnumerable<Assembly> assemblies, bool isAssertConfigurationIsValid = true, bool isCompileMappings = true)
        {
            return AddAutoMapperClasses(services, additionalInitAction, assemblies, isAssertConfigurationIsValid, isCompileMappings);
        }

        /// <summary>
        ///     Add AutoMapper with Profiles scan and Dependency Injection 
        /// </summary>
        /// <param name="services">                    </param>
        /// <param name="isAssertConfigurationIsValid">
        ///     Check all auto mapper profile is valid by <c> Mapper.AssertConfigurationIsValid(); </c>
        /// </param>
        /// <param name="isCompileMappings">           
        ///     AutoMapper lazily compiles the type map plans on first map. However, this behavior is
        ///     not always desirable, so you can tell AutoMapper to compile its mappings directly by
        ///     <c> Mapper.Configuration.CompileMappings(); </c>
        /// </param>
        /// <param name="profileAssemblyMarkerTypes">  </param>
        /// <returns></returns>
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, bool isAssertConfigurationIsValid = true, bool isCompileMappings = true, params Type[] profileAssemblyMarkerTypes)
        {
            return AddAutoMapperClasses(services, null, profileAssemblyMarkerTypes.GetAssemblies(), isAssertConfigurationIsValid, isCompileMappings);
        }

        /// <summary>
        ///     Add AutoMapper with Profiles scan and Dependency Injection 
        /// </summary>
        /// <param name="services">                    </param>
        /// <param name="additionalInitAction">        </param>
        /// <param name="isAssertConfigurationIsValid">
        ///     Check all auto mapper profile is valid by <c> Mapper.AssertConfigurationIsValid(); </c>
        /// </param>
        /// <param name="isCompileMappings">           
        ///     AutoMapper lazily compiles the type map plans on first map. However, this behavior is
        ///     not always desirable, so you can tell AutoMapper to compile its mappings directly by
        ///     <c> Mapper.Configuration.CompileMappings(); </c>
        /// </param>
        /// <param name="profileAssemblyMarkerTypes">  </param>
        /// <returns></returns>
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction, bool isAssertConfigurationIsValid = true, bool isCompileMappings = true, params Type[] profileAssemblyMarkerTypes)
        {
            return AddAutoMapperClasses(services, additionalInitAction, profileAssemblyMarkerTypes.GetAssemblies(), isAssertConfigurationIsValid, isCompileMappings);
        }

        /// <summary>
        ///     Add AutoMapper with Profiles scan and Dependency Injection 
        /// </summary>
        /// <param name="services">                    </param>
        /// <param name="additionalInitAction">        </param>
        /// <param name="profileAssemblyMarkerTypes">  </param>
        /// <param name="isAssertConfigurationIsValid">
        ///     Check all auto mapper profile is valid by <c> Mapper.AssertConfigurationIsValid(); </c>
        /// </param>
        /// <param name="isCompileMappings">           
        ///     AutoMapper lazily compiles the type map plans on first map. However, this behavior is
        ///     not always desirable, so you can tell AutoMapper to compile its mappings directly by
        ///     <c> Mapper.Configuration.CompileMappings(); </c>
        /// </param>
        /// <returns></returns>
        public static IServiceCollection AddAutoMapper(this IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction, IEnumerable<Type> profileAssemblyMarkerTypes, bool isAssertConfigurationIsValid = true, bool isCompileMappings = true)
        {
            return AddAutoMapperClasses(services, additionalInitAction, profileAssemblyMarkerTypes.GetAssemblies(), isAssertConfigurationIsValid, isCompileMappings);
        }

        private static IServiceCollection AddAutoMapperClasses(IServiceCollection services, Action<IMapperConfigurationExpression> additionalInitAction, IEnumerable<Assembly> assembliesToScan, bool isAssertConfigurationIsValid = true, bool isCompileMappings = true)
        {
            additionalInitAction = additionalInitAction ?? DefaultConfig;
            assembliesToScan = assembliesToScan as Assembly[] ?? assembliesToScan.ToArray();

            // Scan Assemblies
            var allTypes = assembliesToScan
                .Where(a => a.GetName().Name != nameof(AutoMapper))
                .SelectMany(a => a.DefinedTypes)
                .ToArray();

            var profiles =
                allTypes
                    .Where(t => typeof(Profile).GetTypeInfo().IsAssignableFrom(t))
                    .Where(t => !t.IsAbstract);

            Mapper.Initialize(cfg =>
            {
                additionalInitAction(cfg);

                foreach (var profile in profiles.Select(t => t.AsType()))
                {
                    cfg.AddProfile(profile);
                }
            });

            // Assert Config
            if (isAssertConfigurationIsValid)
                Mapper.AssertConfigurationIsValid();

            if (isCompileMappings)
                Mapper.Configuration.CompileMappings();

            // Dependency Injection
            var openTypes = new[]
            {
                typeof(IValueResolver<,,>),
                typeof(IMemberValueResolver<,,,>),
                typeof(ITypeConverter<,>)
            };

            var dependencyTypes = openTypes.SelectMany(openType => allTypes.Where(t => t.IsClass && !t.IsAbstract && t.AsType().IsImplementGenericInterface(openType)));

            foreach (var type in dependencyTypes)
            {
                services.AddTransient(type.AsType());
            }

            services.AddSingleton(Mapper.Configuration);
            return services.AddScoped<IMapper>(
                sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));
        }

        private static IEnumerable<Assembly> GetCandidateAssemblies(DependencyContext dependencyContext)
        {
            return GetCandidateLibraries(dependencyContext)
                .SelectMany(library => library.GetDefaultAssemblyNames(dependencyContext))
                .Select(Assembly.Load);
        }

        private static IEnumerable<RuntimeLibrary> GetCandidateLibraries(DependencyContext dependencyContext)
        {
            if (ReferenceAssemblies == null)
                return Enumerable.Empty<RuntimeLibrary>();

            var candidatesResolver = new CandidateResolver(dependencyContext.RuntimeLibraries, ReferenceAssemblies);
            return candidatesResolver.GetCandidates();
        }

        /// <summary>
        ///     Candidate Resolver 
        /// </summary>
        private class CandidateResolver
        {
            private readonly IDictionary<string, Dependency> _dependencies;

            public CandidateResolver(IReadOnlyList<RuntimeLibrary> dependencies, ISet<string> referenceAssemblies)
            {
                var dependenciesWithNoDuplicates = new Dictionary<string, Dependency>(StringComparer.OrdinalIgnoreCase);
                foreach (var dependency in dependencies)
                {
                    if (dependenciesWithNoDuplicates.ContainsKey(dependency.Name))
                        throw new InvalidOperationException(
                            $"A duplicate entry for library reference {dependency.Name} was found. Please check that all package references in all projects use the same casing for the same package references.");
                    dependenciesWithNoDuplicates.Add(dependency.Name,
                        CreateDependency(dependency, referenceAssemblies));
                }

                _dependencies = dependenciesWithNoDuplicates;
            }

            private Dependency CreateDependency(RuntimeLibrary library, ISet<string> referenceAssemblies)
            {
                var classification = DependencyClassification.Unknown;
                if (referenceAssemblies.Contains(library.Name))
                    classification = DependencyClassification.AutoMapperReference;

                return new Dependency(library, classification);
            }

            private DependencyClassification ComputeClassification(string dependency)
            {
                var candidateEntry = _dependencies[dependency];
                if (candidateEntry.Classification != DependencyClassification.Unknown)
                {
                    return candidateEntry.Classification;
                }
                var classification = DependencyClassification.NotCandidate;
                foreach (var candidateDependency in candidateEntry.Library.Dependencies)
                {
                    var dependencyClassification = ComputeClassification(candidateDependency.Name);
                    if (dependencyClassification == DependencyClassification.Candidate ||
                        dependencyClassification == DependencyClassification.AutoMapperReference)
                    {
                        classification = DependencyClassification.Candidate;
                        break;
                    }
                }

                candidateEntry.Classification = classification;

                return classification;
            }

            public IEnumerable<RuntimeLibrary> GetCandidates()
            {
                foreach (var dependency in _dependencies)
                    if (ComputeClassification(dependency.Key) == DependencyClassification.Candidate)
                        yield return dependency.Value.Library;
            }

            private class Dependency
            {
                public Dependency(RuntimeLibrary library, DependencyClassification classification)
                {
                    Library = library;
                    Classification = classification;
                }

                public RuntimeLibrary Library { get; }

                public DependencyClassification Classification { get; set; }

                public override string ToString()
                {
                    return $"Library: {Library.Name}, Classification: {Classification}";
                }
            }

            private enum DependencyClassification
            {
                Unknown = 0,
                Candidate = 1,
                NotCandidate = 2,
                AutoMapperReference = 3
            }
        }
    }
}