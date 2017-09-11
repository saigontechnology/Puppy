#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey </Project>
//     <File>
//         <Name> ServiceCollectionExtensions.cs </Name>
//         <Created> 31/07/17 10:43:01 PM </Created>
//         <Key> fa38efb4-82e0-41c8-9e65-9b86a7d91bcf </Key>
//     </File>
//     <Summary>
//         ServiceCollectionExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using Puppy.Core.EnvironmentUtils;
using Puppy.Swagger.Filters;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;

namespace Puppy.Swagger
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Add Swagger API Document 
        ///     <para> Xml documentation file full path generate by build main project </para>
        ///     <para>
        ///         File Path follow config from <c> .csproj </c>, not from <c> appsettings.json </c>
        ///     </para>
        /// </summary>
        /// <param name="services">               </param>
        /// <param name="xmlDocumentFileFullPath"></param>
        /// <param name="configuration">          </param>
        /// <param name="configSection">          </param>
        /// <remarks>
        ///     Example for Xml Document File Full Path: <c>
        ///     Path.Combine(Directory.GetCurrentDirectory(), "Puppy.xml") </c>
        /// </remarks>
        /// <returns></returns>
        public static IServiceCollection AddApiDocument(this IServiceCollection services, string xmlDocumentFileFullPath, IConfiguration configuration, string configSection = Constants.DefaultConfigSection)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            // Add Filter Service
            services.AddScoped<ApiDocAccessFilter>();

            // Build Config
            configuration.BuildConfig(configSection);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(SwaggerConfig.ApiDocumentName, new Info
                {
                    Title = SwaggerConfig.ApiDocumentHtmlTitle,
                    Version = SwaggerConfig.ApiDocumentName,
                    Contact = SwaggerConfig.Contact != null
                        ? new Contact
                        {
                            Name = SwaggerConfig.Contact.Name,
                            Url = SwaggerConfig.Contact.Url,
                            Email = SwaggerConfig.Contact.Email
                        }
                        : null
                });

                options.IncludeXmlCommentsIfExists(xmlDocumentFileFullPath);
                options.DocumentFilter<HideInDocsFilter>();

                // Ignore Obsolete
                options.IgnoreObsoleteProperties();
                options.IgnoreObsoleteActions();

                if (SwaggerConfig.IsDescribeAllEnumsAsString)
                {
                    options.DescribeAllEnumsAsStrings();
                }

                if (SwaggerConfig.IsDescribeAllParametersInCamelCase)
                {
                    options.DescribeAllParametersInCamelCase();
                }
            });

            return services;
        }

        /// <summary>
        ///     Add Swagger API Document 
        ///     <para> Xml documentation file full path generate by build main project </para>
        ///     <para>
        ///         File Path follow config from <c> .csproj </c>, not from <c> appsettings.json </c>
        ///     </para>
        /// </summary>
        /// <param name="services">     </param>
        /// <param name="assembly">     </param>
        /// <param name="configuration"></param>
        /// <param name="configSection"></param>
        /// <remarks>
        ///     Example for Xml Document File Full Path: <c>
        ///     Path.Combine(Directory.GetCurrentDirectory(), "Puppy.xml") </c>
        /// </remarks>
        /// <returns></returns>
        public static IServiceCollection AddApiDocument(this IServiceCollection services, Assembly assembly, IConfiguration configuration, string configSection = Constants.DefaultConfigSection)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            // Add Filter Service
            services.AddScoped<ApiDocAccessFilter>();

            // Build Config
            configuration.BuildConfig(configSection);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(SwaggerConfig.ApiDocumentName, new Info
                {
                    Title = SwaggerConfig.ApiDocumentHtmlTitle,
                    Version = SwaggerConfig.ApiDocumentName,
                    Contact = SwaggerConfig.Contact != null
                        ? new Contact
                        {
                            Name = SwaggerConfig.Contact.Name,
                            Url = SwaggerConfig.Contact.Url,
                            Email = SwaggerConfig.Contact.Email
                        }
                        : null
                });

                options.IncludeXmlCommentsIfExists(assembly);
                options.DocumentFilter<HideInDocsFilter>();

                // Ignore Obsolete
                options.IgnoreObsoleteProperties();
                options.IgnoreObsoleteActions();

                if (SwaggerConfig.IsDescribeAllEnumsAsString)
                {
                    options.DescribeAllEnumsAsStrings();
                }

                if (SwaggerConfig.IsDescribeAllParametersInCamelCase)
                {
                    options.DescribeAllParametersInCamelCase();
                }
            });

            return services;
        }

        public static IApplicationBuilder UseApiDocument(this IApplicationBuilder app)
        {
            app.UseMiddleware<SwaggerAccessMiddleware>();

            app.UseSwagger(c =>
            {
                c.RouteTemplate = SwaggerConfig.RouteTemplateEndpoint;
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Host = httpReq.Host.Value);
            });

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = SwaggerConfig.RoutePrefix;
                c.SwaggerEndpoint(SwaggerConfig.SwaggerEndpoint, SwaggerConfig.ApiDocumentHtmlTitle);
            });

            // Path and GZip for Statics Content
            string currentDirectory = Directory.GetCurrentDirectory();
            string executedAssemblyDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string fileProviderPath = Path.Combine(currentDirectory, Constants.ApiDocAssetFolderPath);

            if (!Directory.Exists(fileProviderPath))
            {
                // Try to get folder in executed assembly
                fileProviderPath = Path.Combine(executedAssemblyDirectory, Constants.ApiDocAssetFolderPath);
            }

            if (!Directory.Exists(fileProviderPath)) return app;

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(fileProviderPath),
                RequestPath = Constants.ApiDocAssetRequestPath,
                OnPrepareResponse = (context) =>
                {
                    var headers = context.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        MaxAge = Constants.ApiDocAssetMaxAgeResponseHeader
                    };
                }
            });

            return app;
        }

        /// <summary>
        ///     Keep swagger access middleware before UseSwagger and UseSwaggerUI to wrap a request 
        /// </summary>
        public class SwaggerAccessMiddleware
        {
            private readonly RequestDelegate _next;

            public SwaggerAccessMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task Invoke(HttpContext context)
            {
                if (!IsSwaggerUi(context) && !IsSwaggerEndpoint(context))
                {
                    await _next.Invoke(context).ConfigureAwait(true);
                    return;
                }

                if (Helper.IsCanAccessSwagger(context))
                {
                    await _next.Invoke(context).ConfigureAwait(true);
                    return;
                }

                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }

            private static bool IsSwaggerUi(HttpContext httpContext)
            {
                var pathQuery = httpContext.Request.Path.Value?.Trim('/').ToLower() ?? string.Empty;
                pathQuery = pathQuery.ToLowerInvariant();

                var documentApiBaseUrl = SwaggerConfig.RoutePrefix ?? string.Empty;
                documentApiBaseUrl = documentApiBaseUrl.ToLowerInvariant();

                var isSwaggerUi = pathQuery == documentApiBaseUrl || pathQuery == $"{documentApiBaseUrl}/index.html";
                return isSwaggerUi;
            }

            private static bool IsSwaggerEndpoint(HttpContext httpContext)
            {
                // get path query with out query param string
                var pathQuery = httpContext.Request.Path.Value?.Trim('/').ToLower() ?? string.Empty;
                var iPathQueryWithoutParam = pathQuery.IndexOf('?');
                pathQuery = iPathQueryWithoutParam > 0 ? pathQuery.Substring(iPathQueryWithoutParam) : pathQuery;
                pathQuery = pathQuery.ToLowerInvariant();

                // get swagger endpoint without query param string
                var swaggerEndpoint = SwaggerConfig.SwaggerEndpoint.Trim('/');
                var iSwaggerEndpointWithoutParam = swaggerEndpoint.IndexOf('?');
                swaggerEndpoint = iSwaggerEndpointWithoutParam > 0 ? swaggerEndpoint.Substring(0, iSwaggerEndpointWithoutParam) : swaggerEndpoint;
                swaggerEndpoint = swaggerEndpoint.ToLowerInvariant();

                // check quest is swagger endpoint
                var isSwaggerEndPoint = pathQuery == swaggerEndpoint;
                return isSwaggerEndPoint;
            }
        }

        public static void BuildConfig(this IConfiguration configuration, string configSection = Constants.DefaultConfigSection)
        {
            var isHaveConfig = configuration.GetChildren().Any(x => x.Key == configSection);

            if (isHaveConfig)
            {
                SwaggerConfig.ApiDocumentHtmlTitle = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.ApiDocumentHtmlTitle)}", SwaggerConfig.ApiDocumentHtmlTitle);

                SwaggerConfig.ApiDocumentUrl = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.ApiDocumentUrl)}", SwaggerConfig.ApiDocumentUrl);

                SwaggerConfig.ApiDocumentName = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.ApiDocumentName)}", SwaggerConfig.ApiDocumentName);

                SwaggerConfig.ApiDocumentJsonFile = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.ApiDocumentJsonFile)}", SwaggerConfig.ApiDocumentJsonFile);

                SwaggerConfig.AccessKey = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.AccessKey)}", SwaggerConfig.AccessKey);

                SwaggerConfig.AccessKeyQueryParam = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.AccessKeyQueryParam)}", SwaggerConfig.AccessKeyQueryParam);

                SwaggerConfig.AuthTokenKeyName = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.AuthTokenKeyName)}", SwaggerConfig.AuthTokenKeyName);

                SwaggerConfig.Contact = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.Contact)}", SwaggerConfig.Contact);

                SwaggerConfig.IsDescribeAllEnumsAsString = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.IsDescribeAllEnumsAsString)}", SwaggerConfig.IsDescribeAllEnumsAsString);

                SwaggerConfig.IsDescribeAllParametersInCamelCase = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.IsDescribeAllParametersInCamelCase)}", SwaggerConfig.IsDescribeAllParametersInCamelCase);
            }

            if (!EnvironmentHelper.IsDevelopment()) return;

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"API Document Json File Endpoint: {SwaggerConfig.SwaggerEndpoint}");
            Console.ResetColor();
        }
    }
}