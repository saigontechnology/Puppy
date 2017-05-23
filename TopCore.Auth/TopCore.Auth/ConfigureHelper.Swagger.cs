using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TopCore.Framework.Web;

namespace TopCore.Auth
{
    public static partial class ConfigureHelper
    {
        public static class Swagger
        {
            private static readonly string DocumentTitle =
                Configuration.GetValue<string>("Developers:ApiDocumentTitle");

            private static readonly string DocumentName =
                Configuration.GetValue<string>("Developers:ApiDocumentName");

            private static readonly string DocumentApiBaseUrl =
                Configuration.GetValue<string>("Developers:ApiDocumentUrl") + DocumentName;

            private static readonly string DocumentJsonFileName =
                Configuration.GetValue<string>("Developers:ApiDocumentJsonFile");

            private static readonly string DocumentUrlBase =
                DocumentApiBaseUrl.Replace(DocumentName, string.Empty).TrimEnd('/');

            private static readonly string SwaggerEndpoint = $"{DocumentUrlBase}/{DocumentName}/{DocumentJsonFileName}";

            public static void Service(IServiceCollection services)
            {
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(DocumentName, new Info
                    {
                        Title = DocumentTitle,
                        Version = DocumentName,
                        Contact = new Contact
                        {
                            Name = "Top Nguyen",
                            Email = "TopNguyen92@gmail.com",
                            Url = "http://topnguyen.net"
                        }
                    });

                    var apiDocumentFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TopCore.Auth.xml");

                    options.IncludeXmlComments(apiDocumentFilePath);
                    options.DocumentFilter<HideInDocsFilter>();
                    options.IgnoreObsoleteProperties();
                    options.IgnoreObsoleteActions();
                    options.DescribeAllEnumsAsStrings();
                    options.DescribeAllParametersInCamelCase();
                });
            }

            public static void Middleware(IApplicationBuilder app)
            {
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = DocumentUrlBase.TrimStart('/') + "/{documentName}/" + DocumentJsonFileName;
                    c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Host = httpReq.Host.Value);
                });

                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = DocumentApiBaseUrl.TrimStart('/');
                    c.SwaggerEndpoint(SwaggerEndpoint + "?key=" + DeveloperAccessKeyConfig,
                        DocumentTitle);
                });
            }

            public class AccessMiddleware
            {
                private readonly RequestDelegate _next;

                public AccessMiddleware(RequestDelegate next)
                {
                    _next = next;
                }

                public Task Invoke(HttpContext context)
                {
                    if (IsSwaggerUI(context) || IsSwaggerEndpoint(context))
                        if (!IsCanAccessSwagger(context))
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            return Task.FromResult(0);
                        }

                    return _next.Invoke(context);
                }

                private static bool IsSwaggerUI(HttpContext httpContext)
                {
                    var pathQuery = httpContext.Request.Path.Value?.Trim('/').ToLower() ?? string.Empty;
                    var documentApiBaseUrl = DocumentApiBaseUrl.Trim('/') ?? string.Empty;
                    var isSwaggerUi = pathQuery == documentApiBaseUrl ||
                                      pathQuery == $"{documentApiBaseUrl}/index.html";
                    return isSwaggerUi;
                }

                private static bool IsSwaggerEndpoint(HttpContext httpContext)
                {
                    var pathQuery = httpContext.Request.Path.Value?.Trim('/').ToLower() ?? string.Empty;
                    var swaggerEndpoint = SwaggerEndpoint.Trim('/');
                    var isSwaggerEndPoint = pathQuery.StartsWith(swaggerEndpoint);
                    return isSwaggerEndPoint;
                }

                private static bool IsCanAccessSwagger(HttpContext httpContext)
                {
                    try
                    {
                        string developerAccessKey = httpContext.Request.Query["key"];
                        var isCanAccessSwagger = string.IsNullOrWhiteSpace(DeveloperAccessKeyConfig) ||
                                                 DeveloperAccessKeyConfig == developerAccessKey;
                        return isCanAccessSwagger;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }
    }
}