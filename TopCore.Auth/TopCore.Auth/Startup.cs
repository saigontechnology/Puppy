using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using TopCore.Auth.Data;
using TopCore.Auth.Domain.Entity;
using TopCore.Auth.Domain.Models;
using TopCore.Framework.DependencyInjection;

namespace TopCore.Auth
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Environment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
                builder.AddUserSecrets<Startup>();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public IHostingEnvironment Environment { get; }

        //------------------------------------------------------------
        // Global
        //------------------------------------------------------------

        public void ConfigureServices(IServiceCollection services)
        {
            IdentityServerStartupHelper.Add(services, Configuration.GetConnectionString(Environment.EnvironmentName));

            services.AddCors(
                options =>
                {
                    options.AddPolicy($"{nameof(TopCore)}.{nameof(Auth)}",
                        policy => { policy.WithOrigins().AllowAnyHeader().AllowAnyMethod(); });
                });

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                // Indented for Development only
                options.SerializerSettings.Formatting = Environment.IsDevelopment()
                    ? Formatting.Indented
                    : Formatting.None;

                // Serialize Json as Camel case
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            });

            services.AddDependencyInjectionScanner()
                .ScanFromAllAssemblies($"{nameof(TopCore)}.{nameof(Auth)}.*.dll",
                    Path.GetFullPath(PlatformServices.Default.Application.ApplicationBasePath));

            services.AddLogging();

            SwaggerStartupHelper.Add(services);

            // Write out all dependency injection services
            services.WriteOut($"{nameof(TopCore)}.{nameof(Auth)}");
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors($"{nameof(TopCore)}.{nameof(Auth)}");
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
            UseLogFactory(loggerFactory);
            UseExceptionPage(app, env);
            SwaggerStartupHelper.Use(app);
            IdentityServerStartupHelper.Use(app);
        }

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

        public static class SwaggerStartupHelper
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

                    var apiDocumentFilePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
                        "TopCore.Auth.xml");
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

        public static class IdentityServerStartupHelper
        {
            public static void Add(IServiceCollection services, string connectionString)
            {
                services.AddAuthentication(
                    options => options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);

                // TopCore.TopCoreIdentityDbContext for Asp Net Core Identity
                services.AddDbContext<TopCoreAuthDbContext>(options => options.UseSqlServer(connectionString));

                // Add Identity store User into Database by Entity Framework
                services.AddIdentity<UserEntity, IdentityRole>().AddEntityFrameworkStores<TopCoreAuthDbContext>();

                var migrationsAssembly = typeof(DataModule).GetTypeInfo().Assembly.GetName().Name;

                // Config and Operation store of Identity Server 4
                services.AddIdentityServer(options =>
                    {
                        options.UserInteraction.LoginUrl = "/login";
                        options.UserInteraction.LogoutUrl = "/logout";
                    })
                    .AddTemporarySigningCredential()
                    //.AddSigningCredential() // TODO Add Later
                    .AddConfigurationStore(
                        builder =>
                            builder.UseSqlServer(connectionString,
                                options => options.MigrationsAssembly(migrationsAssembly)))
                    .AddOperationalStore(
                        builder =>
                            builder.UseSqlServer(connectionString,
                                options => options.MigrationsAssembly(migrationsAssembly)))
                    .AddAspNetIdentity<UserEntity>();
            }

            public static void Use(IApplicationBuilder app)
            {
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

                //application to use cookie authentication
                app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationScheme = "Cookies",
                    AutomaticAuthenticate = true,
                    AutomaticChallenge = true
                });

                //use OpenID Connect Provider (IdentityServer)
                app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
                {
                    AuthenticationScheme = "oidc",
                    SignInScheme = "Cookies",
                    Authority = "https://localhost:55555/",
                    RequireHttpsMetadata = false,
                    ClientId = "mvc",
                    SaveTokens = true
                });

                // Use Identity Server
                app.UseIdentity();
                app.UseIdentityServer();
            }
        }
    }
}