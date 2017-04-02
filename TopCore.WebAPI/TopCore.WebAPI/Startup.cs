using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using TopCore.Framework.DependencyInjection;

namespace TopCore.WebAPI
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
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                // Indented for Development only
                options.SerializerSettings.Formatting = Environment.IsDevelopment() ? Formatting.Indented : Formatting.None;

                // Serialize Json as Camel case
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            });

            services
                .AddDependencyInjectionScanner()
                .ScanFromAllAssemblies($"{nameof(TopCore)}.*.dll", Path.GetFullPath(PlatformServices.Default.Application.ApplicationBasePath));

            services.AddLogging();

            AddSwagger(services);

            AddIdentityServer(services);

            // Write out all dependency injection services
            services.WriteOut($"{nameof(TopCore)}");
        }

        private void AddIdentityServer(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("Identity");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // Add DB Context for application
            services.AddDbContext<TopCoreIdentityContext>(options => options.UseSqlServer(connectionString));

            // Add Identity store User into Database by Entity Framework
            services.AddIdentity<TopCoreIdentityUser, IdentityRole>().AddEntityFrameworkStores<TopCoreIdentityContext>();

            services.AddIdentityServer()
                .AddTemporarySigningCredential()
                .AddAspNetIdentity<TopCoreIdentityUser>()
                .AddOperationalStore(builder => builder.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationsAssembly)))
                .AddConfigurationStore(builder => builder.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationsAssembly)));
        }

        private static void UseIdentityServer(IApplicationBuilder app)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            //application to use cookie authentication

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookies"
            });

            //use OpenID Connect Provider (IdentityServer)

            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                AuthenticationScheme = "oidc",
                SignInScheme = "Cookies",

                Authority = "https://localhost:44375/",

                RequireHttpsMetadata = false,

                ClientId = "mvc",

                SaveTokens = true
            });
            app.UseIdentity();
            app.UseIdentityServer();
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

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            UseLogFactory(loggerFactory);

            UseExceptionPage(app, env);

            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();

            UseSwagger(app);

            UseIdentityServer(app);
        }

        private void UseLogFactory(ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
        }

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
    }

    public class TopCoreIdentityContext : IdentityDbContext
    {
        public TopCoreIdentityContext(DbContextOptions<TopCoreIdentityContext> options) : base(options)
        {
        }
    }

    public class TopCoreIdentityUser : IdentityUser
    {
        public bool IsAdmin { get; set; }
        public string DataEventRecordsRole { get; set; }
        public string SecuredFilesRole { get; set; }
        public DateTime AccountExpires { get; set; }
    }
}