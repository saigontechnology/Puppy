#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> ServiceCollectionExtensions.cs </Name>
//         <Created> 28/09/17 11:37:50 PM </Created>
//         <Key> 79e6fa88-035c-4691-a5bf-d38402fc8e44 </Key>
//     </File>
//     <Summary>
//         ServiceCollectionExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Puppy.Core.DateTimeUtils;
using Puppy.DataTable.Constants;
using System;
using System.Linq;

namespace Puppy.DataTable
{
    public static class ServiceCollectionExtensions
    {
        private static IConfiguration _configuration;
        private static string _configSection;

        /// <summary>
        ///     [DataTable] Add DataTable, add "DataTable" section in your appsettings.json to config DataTable.
        /// </summary>
        /// <param name="services">          </param>
        /// <param name="configuration">     </param>
        /// <param name="sharedResourceType">
        ///     Shared Resource Type for
        ///     <see cref="Puppy.DataTable.Attributes.DataTableAttribute.DisplayName" /> and will be
        ///     override by <see cref="Puppy.DataTable.Attributes.DataTableAttribute.DisplayNameResourceType" />
        /// </param>
        /// <param name="configSection">     </param>
        /// <returns></returns>
        public static IServiceCollection AddDataTable(this IServiceCollection services, IConfiguration configuration, Type sharedResourceType = null, string configSection = ConfigConst.DefaultConfigSection)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _configSection = configSection;

            configuration.BuildConfig(configSection);

            DataTableGlobalConfig.SharedResourceType = sharedResourceType;

            services.Configure<MvcOptions>(options =>
            {
                // [DataTable]
                options.AddDataTableModelBinderProvider();
            });

            return services;
        }

        /// <summary>
        ///     [DataTable] Use DataTable
        /// </summary>
        /// <param name="app"></param>

        /// <returns></returns>
        /// <remarks>
        ///     The global config for DataTable from appsettings.json will auto reload when you
        ///     change config.
        /// </remarks>
        public static IApplicationBuilder UseDataTable(this IApplicationBuilder app)
        {
            _configuration.BuildConfig(_configSection);

            ChangeToken.OnChange(_configuration.GetReloadToken, () =>
            {
                // Re-Build the config for DataTable
                _configuration.BuildConfig(_configSection);
            });

            return app;
        }

        internal static void BuildConfig(this IConfiguration configuration, string configSection = ConfigConst.DefaultConfigSection)
        {
            var isHaveConfig = configuration.GetChildren().Any(x => x.Key == configSection);

            if (isHaveConfig)
            {
                DataTableGlobalConfig.DateTimeTimeZone = configuration.GetValue($"{configSection}:{nameof(DataTableGlobalConfig.DateTimeTimeZone)}", DataTableGlobalConfig.DateTimeTimeZone);

                try
                {
                    DateTimeOffset.UtcNow.WithTimeZone(DataTableGlobalConfig.DateTimeTimeZone);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"{nameof(DataTableGlobalConfig.DateTimeTimeZone)} must correct DateTime TimeZone Id. {ex.Message}");
                }

                DataTableGlobalConfig.DateFormat = configuration.GetValue($"{configSection}:{nameof(DataTableGlobalConfig.DateFormat)}", DataTableGlobalConfig.DateFormat);

                try
                {
                    DateTimeOffset.UtcNow.ToString(DataTableGlobalConfig.DateFormat);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"{nameof(DataTableGlobalConfig.DateFormat)} must correct Date format. {ex.Message}");
                }

                DataTableGlobalConfig.DateTimeFormat = configuration.GetValue($"{configSection}:{nameof(DataTableGlobalConfig.DateTimeFormat)}", DataTableGlobalConfig.DateTimeFormat);

                try
                {
                    DateTimeOffset.UtcNow.ToString(DataTableGlobalConfig.DateTimeFormat);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"{nameof(DataTableGlobalConfig.DateTimeFormat)} must correct DateTime format. {ex.Message}");
                }

                DataTableGlobalConfig.RequestDateTimeFormatMode = configuration.GetValue($"{configSection}:{nameof(DataTableGlobalConfig.RequestDateTimeFormatMode)}", DataTableGlobalConfig.RequestDateTimeFormatMode);
            }
        }
    }
}