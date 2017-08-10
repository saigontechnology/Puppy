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
using Puppy.Core.ConfigUtils;
using Puppy.Core.EnvironmentUtils;
using Puppy.Swagger.Filters;
using Puppy.Swagger.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
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
            // Add Filter Service
            services.AddScoped<ApiDocAccessFilter>();

            // Build Config
            configuration.BuildSwaggerConfig(configSection);

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

                options.IncludeXmlComments(xmlDocumentFileFullPath);
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

            app.UseMiddleware<SwaggerAccessMiddleware>();

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

        public class SwaggerAccessMiddleware
        {
            private readonly RequestDelegate _next;

            public SwaggerAccessMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public Task Invoke(HttpContext context)
            {
                if (!IsSwaggerUi(context) && !IsSwaggerEndpoint(context))
                {
                    return _next.Invoke(context);
                }

                if (Helper.IsCanAccessSwagger(context))
                {
                    return _next.Invoke(context);
                }

                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Task.FromResult(0);
            }

            private static bool IsSwaggerUi(HttpContext httpContext)
            {
                var pathQuery = httpContext.Request.Path.Value?.Trim('/').ToLower() ?? string.Empty;
                var documentApiBaseUrl = SwaggerConfig.RoutePrefix ?? string.Empty;
                var isSwaggerUi = pathQuery == documentApiBaseUrl || pathQuery == $"{documentApiBaseUrl}/index.html";
                return isSwaggerUi;
            }

            private static bool IsSwaggerEndpoint(HttpContext httpContext)
            {
                var pathQuery = httpContext.Request.Path.Value?.Trim('/').ToLower() ?? string.Empty;
                var swaggerEndpoint = SwaggerConfig.SwaggerEndpoint.Trim('/');
                var isSwaggerEndPoint = pathQuery.StartsWith(swaggerEndpoint);
                return isSwaggerEndPoint;
            }
        }

        public static void BuildSwaggerConfig(this IConfiguration configuration, string configSection = Constants.DefaultConfigSection)
        {
            SwaggerConfig.ApiDocumentHtmlTitle = configuration.GetValue<string>($"{configSection}:{nameof(SwaggerConfig.ApiDocumentHtmlTitle)}");
            SwaggerConfig.ApiDocumentUrl = configuration.GetValue<string>($"{configSection}:{nameof(SwaggerConfig.ApiDocumentUrl)}");
            SwaggerConfig.ApiDocumentName = configuration.GetValue<string>($"{configSection}:{nameof(SwaggerConfig.ApiDocumentName)}");
            SwaggerConfig.ApiDocumentJsonFile = configuration.GetValue<string>($"{configSection}:{nameof(SwaggerConfig.ApiDocumentJsonFile)}");
            SwaggerConfig.AccessKey = configuration.GetValue<string>($"{configSection}:{nameof(SwaggerConfig.AccessKey)}");
            SwaggerConfig.AccessKeyQueryParam = configuration.GetValue<string>($"{configSection}:{nameof(SwaggerConfig.AccessKeyQueryParam)}");
            SwaggerConfig.AuthTokenKeyName = configuration.GetValue<string>($"{configSection}:{nameof(SwaggerConfig.AuthTokenKeyName)}");
            SwaggerConfig.Contact = configuration.GetSection<SwaggerContactConfigModel>($"{configSection}:{nameof(SwaggerConfig.Contact)}");

            if (!EnvironmentHelper.IsDevelopment()) return;

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"API Document Json File Endpoint: {SwaggerConfig.SwaggerEndpoint}");
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}