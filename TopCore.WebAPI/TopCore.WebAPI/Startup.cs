using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
using TopCore.Service;
using TopCore.Service.Facade;

namespace TopCore.WebAPI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddLogging();

            AddSwagger(services);

            AddDependency(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            UseLogFactory(loggerFactory);

            UseExceptionPage(app, env);

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();

            UseSwagger(app);
        }

        #region Dependency

        private void AddDependency(IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
        }

        #endregion

        #region Log

        private void UseLogFactory(ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
        }

        #endregion

        #region Exception

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

        #endregion

        #region Swagger

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

        #endregion
    }
}