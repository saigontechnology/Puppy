using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using System.Reflection;
using TopCore.Framework.DependencyInjection;

namespace TopCore.WebAPI
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public IHostingEnvironment Environment { get; }

        //------------------------------------------------------------
        // Global
        //------------------------------------------------------------

        public Startup(IHostingEnvironment env)
        {
            Environment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(nameof(TopCore), policy =>
                {
                    policy.WithOrigins().AllowAnyHeader().AllowAnyMethod();
                });
            });

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                // Indented for Development only
                options.SerializerSettings.Formatting = Environment.IsDevelopment() ? Formatting.Indented : Formatting.None;

                // Serialize Json as Camel case
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            });

            services.AddDependencyInjectionScanner().ScanFromAllAssemblies($"{nameof(TopCore)}.*.dll", Path.GetFullPath(PlatformServices.Default.Application.ApplicationBasePath));

            services.AddLogging();

            AddSwagger(services);

            // Write out all dependency injection services
            services.WriteOut($"{nameof(TopCore)}");
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors(nameof(TopCore));
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            UseLogFactory(loggerFactory);

            UseExceptionPage(app, env);

            UseSwagger(app);
        }

        //------------------------------------------------------------
        // Swagger
        //------------------------------------------------------------

        private void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Title = "Top Core API",
                    Version = "v1",
                    Contact = new Contact
                    {
                        Name = "Top Nguyen",
                        Email = "TopNguyen92@gmail.com",
                        Url = "http://topnguyen.net"
                    }
                });

                options.DescribeAllEnumsAsStrings();

                var apiDocumentFilePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
                    "TopCore.WebAPI.xml");
                options.IncludeXmlComments(apiDocumentFilePath);
            });
        }

        private void UseSwagger(IApplicationBuilder app)
        {
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "api-docs/{documentName}/topcore.json";
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Host = httpReq.Host.Value);
            });

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "api";
                c.SwaggerEndpoint("/api-docs/v1/topcore.json", "Top Core API");
            });
        }

        //------------------------------------------------------------
        // Others
        //------------------------------------------------------------

        private void UseLogFactory(ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
        }

        private static void UseExceptionPage(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
        }
    }
}