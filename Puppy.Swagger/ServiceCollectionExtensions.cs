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
using Puppy.Core.StringUtils;
using Puppy.Swagger.Filters;
using Puppy.Web.HttpUtils;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
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
        /// <param name="isFullSchemaForType">    </param>
        /// <remarks>
        ///     Example for Xml Document File Full Path: <c>
        ///     Path.Combine(Directory.GetCurrentDirectory(), "Puppy.xml") </c>
        /// </remarks>
        /// <returns></returns>
        public static IServiceCollection AddApiDocument(this IServiceCollection services, string xmlDocumentFileFullPath, IConfiguration configuration, string configSection = Constants.DefaultConfigSection, bool isFullSchemaForType = true)
        {
            ConfigService(configuration, configSection);

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

                // Enable multiple [SwaggerResponse] with same "StatusCodes" and allow create and use
                // custom Attribute inheritance from [SwaggerResponse]
                options.OperationFilter<MultipleResponsesOperationFilter>();

                // Adds an Authorization input box to every endpoint
                //options.OperationFilter<AuthorizationInputOperationFilter>();

                // Ignore Obsolete
                options.IgnoreObsoleteProperties();
                options.IgnoreObsoleteActions();

                if (isFullSchemaForType)
                {
                    options.CustomSchemaIds(type => type.FullName);
                }

                if (SwaggerConfig.IsDescribeAllEnumsAsString)
                {
                    options.DescribeAllEnumsAsStrings();
                }

                if (SwaggerConfig.IsDescribeAllParametersInCamelCase)
                {
                    options.DescribeAllParametersInCamelCase();

                    if (SwaggerConfig.IsDescribeAllEnumsAsString)
                    {
                        options.DescribeStringEnumsInCamelCase();
                    }
                }

                // Order
                options.OrderActionsBy(apiDesc => $"[{apiDesc.HttpMethod}]{apiDesc.RelativePath}");
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
        /// <param name="services">           </param>
        /// <param name="assembly">           </param>
        /// <param name="configuration">      </param>
        /// <param name="configSection">      </param>
        /// <param name="isFullSchemaForType"></param>
        /// <remarks>
        ///     Example for Xml Document File Full Path: <c>
        ///     Path.Combine(Directory.GetCurrentDirectory(), "Puppy.xml") </c>
        /// </remarks>
        /// <returns></returns>
        public static IServiceCollection AddApiDocument(this IServiceCollection services, Assembly assembly, IConfiguration configuration, string configSection = Constants.DefaultConfigSection, bool isFullSchemaForType = true)
        {
            ConfigService(configuration, configSection);
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

                // Enable multiple [SwaggerResponse] with same "StatusCodes" and allow create and use
                // custom Attribute inheritance from [SwaggerResponse]
                options.OperationFilter<MultipleResponsesOperationFilter>();

                // Adds an Authorization input box to every endpoint
                //options.OperationFilter<AuthorizationInputOperationFilter>();

                options.IgnoreObsoleteProperties();
                options.IgnoreObsoleteActions();

                if (isFullSchemaForType)
                {
                    options.CustomSchemaIds(type => type.FullName);
                }

                if (SwaggerConfig.IsDescribeAllEnumsAsString)
                {
                    options.DescribeAllEnumsAsStrings();
                }

                if (SwaggerConfig.IsDescribeAllParametersInCamelCase)
                {
                    options.DescribeAllParametersInCamelCase();

                    if (SwaggerConfig.IsDescribeAllEnumsAsString)
                    {
                        options.DescribeStringEnumsInCamelCase();
                    }
                }

                // Order
                options.OrderActionsBy(apiDesc => $"[{apiDesc.HttpMethod}]{apiDesc.RelativePath}");
            });

            return services;
        }

        private static void ConfigService(IConfiguration configuration, string configSection)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            // Build Config
            configuration.BuildConfig(configSection);
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
            string fileProviderPath = Constants.ApiDocAssetFolderPath.GetFullPath();

            if (!Directory.Exists(fileProviderPath))
            {
                // Try to get folder in executed assembly
                fileProviderPath = Constants.ApiDocAssetFolderPath.GetFullPath(null);
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
                if (!Helper.IsSwaggerUi(context)
                    && !context.Request.IsRequestFor(SwaggerConfig.SwaggerEndpoint)
                    && !context.Request.IsRequestFor(SwaggerConfig.JsonViewerUiUrl)
                    && !context.Request.IsRequestFor(SwaggerConfig.ApiDocumentUiUrl))
                {
                    await _next.Invoke(context).ConfigureAwait(true);
                    return;
                }

                // Set cookie if need
                string requestAccessKey = context.Request.Query[SwaggerConfig.AccessKeyQueryParam];

                if (!string.IsNullOrWhiteSpace(requestAccessKey) && context.Request.Cookies[SwaggerConfig.AccessKeyQueryParam] != requestAccessKey)
                {
                    SetCookie(context, Helper.CookieAccessKeyName, requestAccessKey);
                }

                // Check Permission
                bool isCanAccess = Helper.IsCanAccessSwagger(context);

                if (!isCanAccess)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;

                    await context.Response.WriteAsync(SwaggerConfig.UnAuthorizeMessage).ConfigureAwait(true);

                    return;
                }

                if (!Helper.IsCanAccessSwagger(context))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.Headers.Clear();
                    await context.Response.WriteAsync(SwaggerConfig.UnAuthorizeMessage).ConfigureAwait(true);
                    return;
                }

                // Json Viewer Ui
                if (context.Request.IsRequestFor(SwaggerConfig.JsonViewerUiUrl))
                {
                    var jsonViewerContentResult = Helper.GetApiJsonViewerHtml();
                    context.Response.ContentType = jsonViewerContentResult.ContentType;
                    context.Response.StatusCode = jsonViewerContentResult.StatusCode ?? StatusCodes.Status200OK;
                    await context.Response.WriteAsync(jsonViewerContentResult.Content, Encoding.UTF8).ConfigureAwait(true);
                    return;
                }

                // API Document custom UI or Default UI
                if (context.Request.IsRequestFor(SwaggerConfig.ApiDocumentUiUrl) || Helper.IsSwaggerUi(context))
                {
                    var apiDocContentResult = Helper.GetApiDocHtml();
                    context.Response.ContentType = apiDocContentResult.ContentType;
                    context.Response.StatusCode = apiDocContentResult.StatusCode ?? StatusCodes.Status200OK;
                    await context.Response.WriteAsync(apiDocContentResult.Content, Encoding.UTF8).ConfigureAwait(true);
                    return;
                }

                // Return next middleware for swagger generate document
                await _next.Invoke(context).ConfigureAwait(true);
            }

            private static void SetCookie(HttpContext context, string key, string value)
            {
                context.Response.Cookies.Append(key, value, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false // allow transmit via http and https
                });
            }
        }

        internal static void BuildConfig(this IConfiguration configuration, string configSection = Constants.DefaultConfigSection)
        {
            var isHaveConfig = configuration.GetChildren().Any(x => x.Key == configSection);

            if (isHaveConfig)
            {
                SwaggerConfig.ApiDocumentUiUrl = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.ApiDocumentUiUrl)}", SwaggerConfig.ApiDocumentUiUrl);
                SwaggerConfig.JsonViewerUiUrl = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.JsonViewerUiUrl)}", SwaggerConfig.JsonViewerUiUrl);
                SwaggerConfig.ApiDocumentHtmlTitle = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.ApiDocumentHtmlTitle)}", SwaggerConfig.ApiDocumentHtmlTitle);
                SwaggerConfig.ApiDocumentUrl = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.ApiDocumentUrl)}", SwaggerConfig.ApiDocumentUrl);
                SwaggerConfig.ApiDocumentName = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.ApiDocumentName)}", SwaggerConfig.ApiDocumentName);
                SwaggerConfig.ApiDocumentJsonFile = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.ApiDocumentJsonFile)}", SwaggerConfig.ApiDocumentJsonFile);
                SwaggerConfig.AccessKey = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.AccessKey)}", SwaggerConfig.AccessKey);
                SwaggerConfig.AccessKeyQueryParam = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.AccessKeyQueryParam)}", SwaggerConfig.AccessKeyQueryParam);
                SwaggerConfig.UnAuthorizeMessage = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.UnAuthorizeMessage)}", SwaggerConfig.UnAuthorizeMessage);
                SwaggerConfig.AuthTokenKeyName = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.AuthTokenKeyName)}", SwaggerConfig.AuthTokenKeyName);
                SwaggerConfig.Contact = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.Contact)}", SwaggerConfig.Contact);
                SwaggerConfig.IsDescribeAllEnumsAsString = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.IsDescribeAllEnumsAsString)}", SwaggerConfig.IsDescribeAllEnumsAsString);
                SwaggerConfig.IsDescribeAllParametersInCamelCase = configuration.GetValue($"{configSection}:{nameof(SwaggerConfig.IsDescribeAllParametersInCamelCase)}", SwaggerConfig.IsDescribeAllParametersInCamelCase);

                if (!SwaggerConfig.ApiDocumentUiUrl.StartsWith("/"))
                {
                    throw new ArgumentException($"{nameof(SwaggerConfig.ApiDocumentUiUrl)} must start by /", nameof(SwaggerConfig.ApiDocumentUiUrl));
                }

                if (!SwaggerConfig.JsonViewerUiUrl.StartsWith("/"))
                {
                    throw new ArgumentException($"{nameof(SwaggerConfig.JsonViewerUiUrl)} must start by /", nameof(SwaggerConfig.JsonViewerUiUrl));
                }
            }
        }
    }
}