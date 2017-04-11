using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using TopCore.Framework.DependencyInjection;

namespace TopCore.WebAPI
{
    public static class StartupHelper
    {
        public static IConfigurationRoot Configuration;
        public static IHostingEnvironment Environment;

        #region Helper

        internal static class DependencyInjection
        {
            public static void Add(IServiceCollection services)
            {
                services
                    .AddDependencyInjectionScanner()
                    .ScanFromAllAssemblies($"{nameof(TopCore)}.{nameof(WebAPI)}.*.dll", Path.GetFullPath(PlatformServices.Default.Application.ApplicationBasePath));

                // Write out all dependency injection services
                services.WriteOut($"{nameof(TopCore)}.{nameof(WebAPI)}");
            }
        }

        internal static class API
        {
            public static void Add(IServiceCollection services)
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

            public static void Use(IApplicationBuilder app)
            {
                app.UseCors($"{nameof(TopCore)}.{nameof(WebAPI)}");
            }
        }

        internal static class Mvc
        {
            public static void Add(IServiceCollection services)
            {
                services.AddMvc().AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                    // Indented for Development only
                    options.SerializerSettings.Formatting = Environment.IsDevelopment() ? Formatting.Indented : Formatting.None;

                    // Serialize Json as Camel case
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });
            }

            public static void Use(IApplicationBuilder app)
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

        internal static class Log
        {
            public static void Add(IServiceCollection services)
            {
            }

            public static void Use(ILoggerFactory loggerFactory)
            {
                //Log.Logger = new LoggerConfiguration()
                //    .ReadFrom.Configuration(Configuration)
                //    .CreateLogger();

                //loggerFactory.AddSerilog();
            }
        }

        internal static class Exception
        {
            public static void Use(IApplicationBuilder app)
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

        internal static class Swagger
        {
            public static void Add(IServiceCollection services)
            {
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new Info
                    {
                        Title = "Top Core SSO",
                        Version = "v1",
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
                });
            }

            public static void Use(IApplicationBuilder app)
            {
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = "api-docs/{documentName}/topcore-sso.json";
                    c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Host = httpReq.Host.Value);
                });

                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = "api";
                    c.SwaggerEndpoint("/api-docs/v1/topcore-sso.json", "Top Core SSO");
                });
            }
        }

        #endregion Helper
    }
}