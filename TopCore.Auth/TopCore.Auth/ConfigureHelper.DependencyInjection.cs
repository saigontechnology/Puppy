using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using TopCore.Framework.DependencyInjection;

namespace TopCore.Auth
{
    public static partial class ConfigureHelper
    {
        public static class DependencyInjection
        {
            public static void Service(IServiceCollection services)
            {
                var systemPrefix = $"{nameof(TopCore)}.{nameof(Auth)}";
                services
                    .AddDependencyInjectionScanner()
                    .ScanFromAllAssemblies($"{systemPrefix}.*.dll",
                        Path.GetFullPath(PlatformServices.Default.Application.ApplicationBasePath));

                // Write out all dependency injection services
                services.WriteOut(systemPrefix);
            }
        }
    }
}