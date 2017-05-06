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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.PlatformAbstractions;
using TopCore.Framework.DependencyInjection.Attributes;
using TopCore.Framework.DependencyInjection.Exceptions;

namespace TopCore.Framework.DependencyInjection
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
			// Maybe throw AlreadyLoaddedAssemblyException in internal exception
			var assembly = AssemblyLoader.LoadFromAssemblyName(assemblyName);

			foreach (var typeInfo in assembly.DefinedTypes)
				foreach (var customAttribute in typeInfo.GetCustomAttributes())
				{
					var customAttributeType = customAttribute.GetType();

					var isDependencyAttribute = typeof(DependencyAttribute).IsAssignableFrom(customAttributeType);

					if (!isDependencyAttribute)
						continue;
					var serviceDescriptor = ((DependencyAttribute)customAttribute).BuildServiceDescriptor(typeInfo);

					// Check is service already register from difference implementation => throw exception
					var isAlreadyDifferenceImplementation = services.Any(
						x =>
							x.ServiceType.FullName == serviceDescriptor.ServiceType.FullName &&
							x.ImplementationType != serviceDescriptor.ImplementationType);

					if (isAlreadyDifferenceImplementation)
					{
						var implementationRegister =
							services.Single(x => x.ServiceType.FullName == serviceDescriptor.ServiceType.FullName).ImplementationType;

						throw new ConflictRegistrationException(
							$"Conflict register, ${serviceDescriptor.ImplementationType} try to register for {serviceDescriptor.ServiceType.FullName}. It already register by {implementationRegister.FullName} before.");
					}

					// Check is service already register from same implementation => remove existing, replace by new one life time cycle
					var isAlreadySameImplementation = services.Any(
						x =>
							x.ServiceType.FullName == serviceDescriptor.ServiceType.FullName &&
							x.ImplementationType == serviceDescriptor.ImplementationType);

					if (isAlreadySameImplementation)
						services = services.Replace(serviceDescriptor);
					else
						services.Add(serviceDescriptor);
				}
		}

		/// <summary>
		///     Register all assemblies 
		/// </summary>
		/// <param name="services">      </param>
		/// <param name="searchPattern">  Search Pattern by Directory.GetFiles </param>
		/// <param name="folderFullPath"> Default is null = current execute application folder </param>
		public void RegisterAllAssemblies(IServiceCollection services, string searchPattern = "*.dll",
			string folderFullPath = null)
		{
			if (string.IsNullOrWhiteSpace(folderFullPath) || !File.Exists(folderFullPath))
				folderFullPath = Path.GetFullPath(PlatformServices.Default.Application.ApplicationBasePath);

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
				listServiceDescriptors = listServiceDescriptors.Where(x => x.ServiceType.FullName.Contains(serviceNameFilter))
					.ToList();

			Console.WriteLine($"{Environment.NewLine}{new string('-', 50)}");

			Console.ForegroundColor = ConsoleColor.DarkCyan;
			Console.WriteLine($"[Total Dependency Injection {listServiceDescriptors.Count}]");
			for (var index = 0; index < listServiceDescriptors.Count; index++)
			{
				var service = listServiceDescriptors[index];
				var no = index + 1;

				var maximumCharacter =
					new List<int>
					{
						service.ServiceType?.Name?.Length ?? 0,
						service.ImplementationType?.Name?.Length ?? 0,
						GetLifeTime(service.Lifetime).Length
					}.Max();

				Console.ResetColor();
				Console.WriteLine($"{no}.");

				Console.ForegroundColor = ConsoleColor.DarkCyan;
				Console.Write($"    Service         |  ");
				Console.ResetColor();
				Console.Write($"{service.ServiceType?.Name?.PadRight(maximumCharacter)}");
				Console.ForegroundColor = ConsoleColor.DarkCyan;
				Console.WriteLine($"  |  {service.ServiceType?.FullName}");

				Console.Write($"    Implementation  |  ");
				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.Write($"{service.ImplementationType?.Name?.PadRight(maximumCharacter)}");
				Console.ForegroundColor = ConsoleColor.DarkCyan;
				Console.WriteLine($"  |  {service.ImplementationType?.FullName}");

				Console.ForegroundColor = ConsoleColor.DarkCyan;
				Console.Write($"    Lifetime        |  ");
				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.WriteLine($"[{GetLifeTime(service.Lifetime)}]");
				Console.WriteLine();
			}
			Console.ResetColor();

			Console.WriteLine($"{new string('-', 50)}{Environment.NewLine}");
		}

		private string GetLifeTime(ServiceLifetime serviceLifetime)
		{
			if (serviceLifetime == ServiceLifetime.Transient)
				return "Per Resolve";
			return
				serviceLifetime == ServiceLifetime.Scoped ? "Per Request" : "Singleton";
		}
	}
}