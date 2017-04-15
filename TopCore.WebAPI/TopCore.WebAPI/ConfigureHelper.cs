using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TopCore.Framework.DependencyInjection;

namespace TopCore.WebAPI
{
    public static class ConfigureHelper
    {
        public static IConfigurationRoot Configuration;
        public static IHostingEnvironment Environment;

        public static string DeveloperAccessKeyConfig => Configuration.GetValue<string>("Developers:AccessKey");

        public static class DependencyInjection
        {
            public static void Service(IServiceCollection services)
            {
                services
                    .AddDependencyInjectionScanner()
                    .ScanFromAllAssemblies($"{nameof(TopCore)}.{nameof(WebAPI)}.*.dll", Path.GetFullPath(PlatformServices.Default.Application.ApplicationBasePath));

                // Write out all dependency injection services
                services.WriteOut($"{nameof(TopCore)}.{nameof(WebAPI)}");
            }
        }

        public static class Api
        {
            public static void Service(IServiceCollection services)
            {
                services.AddCors(options =>
                {
                    options.AddPolicy($"{nameof(TopCore)}.{nameof(WebAPI)}",
                        policy =>
                        {
                            policy
                                .WithOrigins()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                        });
                });
            }

            public static void Middleware(IApplicationBuilder app)
            {
                app.UseCors($"{nameof(TopCore)}.{nameof(WebAPI)}");
            }
        }

        public static class Mvc
        {
            public static void Service(IServiceCollection services)
            {
                services.AddMvc()
                    .AddXmlDataContractSerializerFormatters()
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                        // Indented for Development only
                        options.SerializerSettings.Formatting = Environment.IsDevelopment()
                            ? Formatting.Indented
                            : Formatting.None;

                        // Serialize Json as Camel case
                        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    });
            }

            public static void Middleware(IApplicationBuilder app)
            {
                if (Environment.IsDevelopment())
                {
                    app.UseBrowserLink();
                }
                app.UseStaticFiles();

                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"assets", "images", "favicons")),
                    RequestPath = new PathString("/favicons")
                });

                app.UseMvcWithDefaultRoute();
            }
        }

        public static class Log
        {
            private static readonly string LogPath = Configuration.GetValue<string>("Developers:LogUrl") + "/" + DeveloperAccessKeyConfig;

            public class AccessMiddleware
            {
                private readonly RequestDelegate _next;

                public AccessMiddleware(RequestDelegate next)
                {
                    _next = next;
                }

                public async Task Invoke(HttpContext context)
                {
                    if (IsLogUI(context))
                    {
                        if (!IsCanAccessLogUI(context))
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            return;
                        }
                    }

                    await _next.Invoke(context).ConfigureAwait(true);
                }

                private bool IsLogUI(HttpContext httpContext)
                {
                    string requestPath = httpContext.Request.Path.Value?.Trim('/').ToLower() ?? string.Empty;

                    string logEndPoint = LogPath.Trim('/');
                    logEndPoint =
                        string.IsNullOrWhiteSpace(logEndPoint)
                            ? string.Empty
                            : logEndPoint.Substring(0, logEndPoint.LastIndexOf("/"));

                    bool isLogUi = requestPath.StartsWith(logEndPoint);
                    return isLogUi;
                }

                private bool IsCanAccessLogUI(HttpContext httpContext)
                {
                    try
                    {
                        string developerAccessKey = httpContext.Request.Path.Value.Substring(httpContext.Request.Path.Value.LastIndexOf("/") + 1, DeveloperAccessKeyConfig?.Length ?? 0); // + 1 for ignore "/"
                        bool isCanAccessLogUi = string.IsNullOrWhiteSpace(DeveloperAccessKeyConfig) || DeveloperAccessKeyConfig == developerAccessKey;
                        return isCanAccessLogUi;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

            public static void Service(IServiceCollection services)
            {
                services.AddElm(options =>
                {
                    options.Path = PathString.FromUriComponent(LogPath);
                    options.Filter = (name, level) => level >= LogLevel.Error;
                });
            }

            public static void Middleware(IApplicationBuilder app, ILoggerFactory loggerFactory)
            {
                // Write log
                Serilog.Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();
                loggerFactory.AddSerilog();

                // Log page
                app.UseElmPage();
                app.UseElmCapture();
            }
        }

        public static class Exception
        {
            public static void Middleware(IApplicationBuilder app)
            {
                if (Environment.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                }
                else
                {
                    app.UseExceptionHandler("/Home/Error");
                }
            }
        }

        public static class Swagger
        {
            private static readonly string DocumentApiBaseUrl = Configuration.GetValue<string>("Developers:ApiDocumentUrl");
            private static readonly string DocumentTitle = Configuration.GetValue<string>("Developers:ApiDocumentTitle");
            private static readonly string DocumentName = Configuration.GetValue<string>("Developers:ApiDocumentName");
            private static readonly string DocumentJsonFileName = Configuration.GetValue<string>("Developers:ApiDocumentJsonFile");
            private static readonly string DocumentUrlBase = DocumentApiBaseUrl.Replace(DocumentName, string.Empty).TrimEnd('/');
            private static readonly string SwaggerEndpoint = $"{DocumentUrlBase}/{DocumentName}/{DocumentJsonFileName}";

            public class AccessMiddleware
            {
                private readonly RequestDelegate _next;

                public AccessMiddleware(RequestDelegate next)
                {
                    _next = next;
                }

                public async Task Invoke(HttpContext context)
                {
                    if (IsSwaggerUI(context) || IsSwaggerEndpoint(context))
                    {
                        if (!IsCanAccessSwagger(context))
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            return;
                        }
                    }

                    await _next.Invoke(context).ConfigureAwait(true);
                }

                private bool IsSwaggerUI(HttpContext httpContext)
                {
                    string pathQuery = httpContext.Request.Path.Value?.Trim('/').ToLower() ?? string.Empty;
                    string documentApiBaseUrl = DocumentApiBaseUrl.Trim('/') ?? string.Empty;
                    bool isSwaggerUi = pathQuery == documentApiBaseUrl || pathQuery == $"{documentApiBaseUrl}/index.html";
                    return isSwaggerUi;
                }

                private bool IsSwaggerEndpoint(HttpContext httpContext)
                {
                    string pathQuery = httpContext.Request.Path.Value?.Trim('/').ToLower() ?? string.Empty;
                    string swaggerEndpoint = SwaggerEndpoint.Trim('/');
                    bool isSwaggerEndPoint = pathQuery.StartsWith(swaggerEndpoint);
                    return isSwaggerEndPoint;
                }

                private bool IsCanAccessSwagger(HttpContext httpContext)
                {
                    try
                    {
                        string developerAccessKey = httpContext.Request.Query["key"];
                        bool isCanAccessSwagger = string.IsNullOrWhiteSpace(DeveloperAccessKeyConfig) || DeveloperAccessKeyConfig == developerAccessKey;
                        return isCanAccessSwagger;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }

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

                    options.DescribeAllEnumsAsStrings();

                    var apiDocumentFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TopCore.WebAPI.xml");
                    options.IncludeXmlComments(apiDocumentFilePath);
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
                    c.SwaggerEndpoint(SwaggerEndpoint + "?key=" + DeveloperAccessKeyConfig, DocumentTitle);
                });
            }
        }

        public class ProcessingTimeMiddleware
        {
            private readonly RequestDelegate _next;

            public ProcessingTimeMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task Invoke(HttpContext context)
            {
                var watch = new Stopwatch();
                context.Response.OnStarting(state =>
                {
                    var httpContext = (HttpContext)state;
                    watch.Stop();
                    string elapsedMilliseconds = watch.ElapsedMilliseconds.ToString();
                    httpContext.Response.Headers.Add("X-Processing-Time-Milliseconds",
                        new StringValues(elapsedMilliseconds));
                    return Task.FromResult(0);
                }, context);

                watch.Start();
                await _next(context).ConfigureAwait(true);
            }
        }

        public class SystemInfoMiddleware
        {
            private readonly RequestDelegate _next;

            public SystemInfoMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task Invoke(HttpContext context)
            {
                context.Response.OnStarting(state =>
                {
                    context.Response.Headers.Add("Server", new StringValues("OWL"));
                    context.Response.Headers.Add("X-Powered-By", new StringValues("http://topnguyen.net"));
                    return Task.FromResult(0);
                }, context);

                await _next(context).ConfigureAwait(true);
            }
        }
    }
}