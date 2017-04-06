using System;
using System.EnterpriseServices;
using IdentityServer4.EntityFramework.DbContexts;
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
using IdentityServer4;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication;
using TopCore.Auth.Data;
using TopCore.Auth.Data.Factory;
using TopCore.Auth.Domain.Entities;
using TopCore.Auth.Domain.Services;
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
            {
                builder.AddUserSecrets<Startup>();
            }

            Configuration = builder.Build();
        }

        private static IConfigurationRoot Configuration { get; set; }

        private static IHostingEnvironment Environment { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            MvcStartupHelper.Add(services);

            SwaggerStartupHelper.Add(services);

            DependencyInjectionScannerHelper.Add(services);

            IdentityServerStartupHelper.Add(services, Configuration.GetConnectionString(Environment.EnvironmentName));

            IdentityServerStartupHelper.SeedData(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            MvcStartupHelper.Use(app);

            LogStartupHelper.Use(loggerFactory);

            SwaggerStartupHelper.Use(app);

            IdentityServerStartupHelper.Use(app);
        }

        #region Helper

        internal static class DependencyInjectionScannerHelper
        {
            public static void Add(IServiceCollection services)
            {
                services
                    .AddDependencyInjectionScanner()
                    .ScanFromAllAssemblies($"{nameof(TopCore)}.{nameof(Auth)}.*.dll", Path.GetFullPath(PlatformServices.Default.Application.ApplicationBasePath));

                // Write out all dependency injection services
                services.WriteOut($"{nameof(TopCore)}.{nameof(Auth)}");
            }
        }

        internal static class MvcStartupHelper
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
                app.UseBrowserLink();
                app.UseStaticFiles();
                app.UseMvcWithDefaultRoute();
            }
        }

        internal static class LogStartupHelper
        {
            public static void Add(IServiceCollection services)
            {
                services.AddLogging();
            }

            public static void Use(ILoggerFactory loggerFactory)
            {
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();
            }
        }

        internal static class ExceptionStartupHelper
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

        internal static class SwaggerStartupHelper
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

                    var apiDocumentFilePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "TopCore.Auth.xml");
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

        internal static class IdentityServerStartupHelper
        {
            public static void Add(IServiceCollection services, string connectionString)
            {
                services.AddCors(options =>
                {
                    options.AddPolicy($"{nameof(TopCore)}.{nameof(Auth)}",
                        policy =>
                        {
                            policy
                                .WithOrigins()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                        });
                });

                services.AddAuthentication(options => options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);

                var migrationsAssembly = typeof(IDataModule).GetTypeInfo().Assembly.GetName().Name;

                services.AddDbContext<Data.DbContext>(builder => builder.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationsAssembly)));

                // Add Identity store User into Database by Entity Framework
                services.AddIdentity<UserEntity, IdentityRole>(o =>
                {
                    o.Password.RequireDigit = false;
                    o.Password.RequireLowercase = false;
                    o.Password.RequireNonAlphanumeric = false;
                    o.Password.RequireUppercase = false;
                    o.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<Data.DbContext>()
                .AddDefaultTokenProviders();

                // Config and Operation store of Identity Server 4
                services.AddIdentityServer(options =>
                    {
                        options.UserInteraction.LoginUrl = "/login";
                        options.UserInteraction.LogoutUrl = "/logout";
                    })
                    .AddTemporarySigningCredential()
                    //.AddSigningCredential() // TODO Add Later
                    .AddConfigurationStore(builder => builder.UseSqlServer(connectionString,
                        options => options.MigrationsAssembly(migrationsAssembly)))
                    .AddOperationalStore(builder => builder.UseSqlServer(connectionString,
                        options => options.MigrationsAssembly(migrationsAssembly)))
                    .AddAspNetIdentity<UserEntity>();
            }

            public static void Use(IApplicationBuilder app)
            {
                app.UseCors($"{nameof(TopCore)}.{nameof(Auth)}");

                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

                //application to use cookie authentication
                app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationScheme = "Cookies",
                    AutomaticAuthenticate = true,
                    AutomaticChallenge = true,
                });

                //use OpenID Connect Provider (IdentityServer)
                app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
                {
                    AuthenticationScheme = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                    GetClaimsFromUserInfoEndpoint = true,
                    SignInScheme = "Cookies",
                    Authority = "https://localhost:55555/",
                    RequireHttpsMetadata = false,
                    ClientId = "topcore_web",
                    ClientSecret = "topcoreweb".Sha256(),
                    ResponseType = "code id_token",
                    DisplayName = "TopCore Web",
                    SaveTokens = true,
                    Scope =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "topcore_api",
                    }
                });

                // Use Identity Server
                app.UseIdentity();
                app.UseIdentityServer();
            }

            /// <summary>
            ///     Initial Seed Data and Database in Startup, Must be synchronus 
            /// </summary>
            /// <param name="services"></param>
            public static void SeedData(IServiceCollection services)
            {
                ISeedAuthService seedAuthService = services.Resolve<ISeedAuthService>();
                PersistedGrantDbContext persistedGrantDbContext = services.Resolve<PersistedGrantDbContext>();
                ConfigurationDbContext configurationDbContext = services.Resolve<ConfigurationDbContext>();

                seedAuthService.SeedAuthDatabase().Wait();
                persistedGrantDbContext.Database.Migrate();
                configurationDbContext.Database.Migrate();
            }
        }

        #endregion Helper
    }
}