using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using System.Reflection;
using TopCore.Framework.DependencyInjection;
using TopCore.SSO.Identity;

namespace TopCore.SSO
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public IHostingEnvironment Environment { get; }

        public Startup(IHostingEnvironment env)
        {
            Environment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        //------------------------------------------------------------
        // Global
        //------------------------------------------------------------

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(IdentityServerHelper).GetTypeInfo().Assembly.GetName().Name;

            IdentityServerHelper.Add(services, Configuration.GetConnectionString(Environment.EnvironmentName), migrationsAssembly);

            services.AddCors(options =>
            {
                options.AddPolicy($"{nameof(TopCore)}.{nameof(SSO)}", policy =>
                {
                    policy.WithOrigins().AllowAnyHeader().AllowAnyMethod();
                });
            });

            services.AddScoped<SignInManager<TopCoreIdentityUser>, SignInManager<TopCoreIdentityUser>>();

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                // Indented for Development only
                options.SerializerSettings.Formatting = Environment.IsDevelopment() ? Formatting.Indented : Formatting.None;

                // Serialize Json as Camel case
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            });

            services.AddDependencyInjectionScanner().ScanFromAllAssemblies($"{nameof(TopCore)}.{nameof(SSO)}.*.dll", Path.GetFullPath(PlatformServices.Default.Application.ApplicationBasePath));

            services.AddLogging();

            AddSwagger(services);

            // Write out all dependency injection services
            services.WriteOut($"{nameof(TopCore)}.{nameof(SSO)}");
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors($"{nameof(TopCore)}.{nameof(SSO)}");
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
            UseLogFactory(loggerFactory);
            UseExceptionPage(app, env);
            UseSwagger(app);

            // Use Identity Server 4
            app.UseIdentity();
            IdentityServerHelper.Use(app);
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

                var apiDocumentFilePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
                    "TopCore.SSO.xml");
                options.IncludeXmlComments(apiDocumentFilePath);
            });
        }

        private void UseSwagger(IApplicationBuilder app)
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