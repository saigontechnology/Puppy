#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ServiceCollectionExtensions.cs </Name>
//         <Created> 17/10/17 8:03:27 PM </Created>
//         <Key> 547860ad-6f4c-4e84-95e5-be86b0d73227 </Key>
//     </File>
//     <Summary>
//         ServiceCollectionExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Puppy.Core.EnvironmentUtils;
using Puppy.Core.ServiceCollectionUtils;
using System;
using System.Linq;

namespace Puppy.Elastic
{
    public static class ServiceCollectionExtensions
    {
        public const string DefaultConfigSection = "Elastic";

        private static IConfiguration _configuration;
        private static string _configSection;

        /// <summary>
        ///     [Elastic Search] Add Elastic, add "Elastic" section in your appsettings.json to
        ///     config Connection.
        /// </summary>
        /// <param name="services">     </param>
        /// <param name="configuration"></param>
        /// <param name="configSection"></param>
        /// <returns></returns>
        public static IServiceCollection AddElastic(this IServiceCollection services, IConfiguration configuration, string configSection = DefaultConfigSection)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _configSection = configSection;

            configuration.BuildConfig(configSection);

            services.AddTransientIfNotExist<IElasticContext, ElasticContext>();

            return services;
        }

        /// <summary>
        ///     [Elastic Search] Use Elastic 
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        /// <remarks>
        ///     The global config for DataTable from appsettings.json will auto reload when you
        ///     change config.
        /// </remarks>
        public static IApplicationBuilder UseElastic(this IApplicationBuilder app)
        {
            _configuration.BuildConfig(_configSection);

            ChangeToken.OnChange(_configuration.GetReloadToken, () =>
            {
                // Re-Build the config for DataTable
                _configuration.BuildConfig(_configSection);
            });

            return app;
        }

        internal static void BuildConfig(this IConfiguration configuration, string configSection = DefaultConfigSection)
        {
            var isHaveConfig = configuration.GetChildren().Any(x => x.Key == configSection);

            if (isHaveConfig)
            {
                ElasticConfig.ConnectionString = configuration.GetValue($"{configSection}:{nameof(ElasticConfig.ConnectionString)}", ElasticConfig.ConnectionString);
            }

            try
            {
                using (ElasticContext context = new ElasticContext())
                {
                }
            }
            catch (Exception)
            {
                throw new ElasticException("Cannot connect to elastic, please install elastic");
            }

            if (!EnvironmentHelper.IsDevelopment()) return;

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Elastic Connect: {ElasticConfig.ConnectionString}");
            Console.ResetColor();
        }
    }
}