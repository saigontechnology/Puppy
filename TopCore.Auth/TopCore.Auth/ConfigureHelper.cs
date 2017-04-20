using IdentityServer4;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using TopCore.Auth.Data.Factory;
using TopCore.Auth.Domain.Entities;
using TopCore.Auth.Domain.Interfaces.Services;
using TopCore.Auth.Service;
using TopCore.Framework.DependencyInjection;

namespace TopCore.Auth
{
    public static class ConfigureHelper
    {
        internal static IConfigurationRoot Configuration;
        internal static IHostingEnvironment Environment;

        internal static class DependencyInjection
        {
            public static void Service(IServiceCollection services)
            {
                services
                    .AddDependencyInjectionScanner()
                    .ScanFromAllAssemblies($"{nameof(TopCore)}.{nameof(Auth)}.*.dll", Path.GetFullPath(PlatformServices.Default.Application.ApplicationBasePath));

                // Write out all dependency injection services
                services.WriteOut($"{nameof(TopCore)}.{nameof(Auth)}");
            }
        }

        internal static class Api
        {
            public static void Service(IServiceCollection services)
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
            }

            public static void Middleware(IApplicationBuilder app)
            {
                app.UseCors($"{nameof(TopCore)}.{nameof(Auth)}");
            }
        }

        internal static class Mvc
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

        internal static class Log
        {
            public static void Service(IServiceCollection services)
            {
                string logPath = Configuration.GetValue<string>("Developers:LogUrl");
                services.AddElm(options =>
                {
                    options.Path = new PathString(logPath);
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

        internal static class Exception
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

        internal static class Swagger
        {
            private static readonly string DocumentApiBaseUrl = Configuration.GetValue<string>("Developers:ApiDocumentUrl");
            private static readonly string DocumentTitle = Configuration.GetValue<string>("Developers:ApiDocumentTitle");
            private static readonly string DocumentName = Configuration.GetValue<string>("Developers:ApiDocumentName");
            private static readonly string DocumentJsonFile = Configuration.GetValue<string>("Developers:ApiDocumentJsonFile");

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

                    var apiDocumentFilePath = Path.Combine(Directory.GetCurrentDirectory(), "TopCore.Auth.xml");
                    options.IncludeXmlComments(apiDocumentFilePath);
                });
            }

            public static void Middleware(IApplicationBuilder app)
            {
                string documentFileBase = DocumentApiBaseUrl.Replace(DocumentName, string.Empty).TrimEnd('/');
                app.UseSwagger(c =>
                {
                    c.RouteTemplate = documentFileBase.TrimStart('/') + "/{documentName}/" + DocumentJsonFile;
                    c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Host = httpReq.Host.Value);
                });

                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = DocumentApiBaseUrl.TrimStart('/');
                    c.SwaggerEndpoint($"{documentFileBase}/{DocumentName}/{DocumentJsonFile}", DocumentTitle);
                });
            }
        }

        internal static class IdentityServer
        {
            public static void Service(IServiceCollection services, string connectionString)
            {
                services.AddAuthentication(options => options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);

                var migrationsAssembly = typeof(IDataModule).GetTypeInfo().Assembly.GetName().Name;

                services.AddDbContext<Data.DbContext>(builder => builder.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationsAssembly)));

                // Service Identity store User into Database by Entity Framework (AspNetUser)
                services.AddIdentity<User, IdentityRole>(o =>
                    {
                        o.Password.RequireDigit = false;
                        o.Password.RequireLowercase = false;
                        o.Password.RequireNonAlphanumeric = false;
                        o.Password.RequireUppercase = false;
                        o.Password.RequiredLength = 6;
                        o.Cookies.ApplicationCookie.AuthenticationScheme = Domain.Constants.Web.CookieSchemaName;
                    })
                    .AddEntityFrameworkStores<Data.DbContext>()
                    .AddDefaultTokenProviders();

                // Config and Operation store of Identity Server 4
                services.AddIdentityServer(options =>
                    {
                        options.UserInteraction.LoginUrl = "/login";
                        options.UserInteraction.LogoutUrl = "/logout";
                        options.Authentication.AuthenticationScheme = Domain.Constants.Web.CookieSchemaName;
                    })
                    .AddTemporarySigningCredential()
                    //.AddSigningCredential() // TODO Service Later
                    .AddConfigurationStore(builder => builder.UseSqlServer(connectionString,
                        options => options.MigrationsAssembly(migrationsAssembly)))
                    .AddOperationalStore(builder => builder.UseSqlServer(connectionString,
                        options => options.MigrationsAssembly(migrationsAssembly)))
                    .AddAspNetIdentity<User>()
                    .AddProfileService<ProfileService>();
            }

            public static void Middleware(IApplicationBuilder app)
            {
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

                //application to use cookie authentication
                app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationScheme = Domain.Constants.Web.CookieSchemaName,
                    AutomaticAuthenticate = true,
                    AutomaticChallenge = true,
                    LoginPath = new PathString("/Account/Login"),
                    LogoutPath = new PathString("/Account/Login"),
                    AccessDeniedPath = new PathString("/Account/Forbidden"),
                    SlidingExpiration = true,
                    ExpireTimeSpan = TimeSpan.FromDays(30)
                });

                //use OpenID Connect Provider (IdentityServer)
                app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
                {
                    RequireHttpsMetadata = false,
                    AuthenticationScheme = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                    GetClaimsFromUserInfoEndpoint = true,
                    SignInScheme = Domain.Constants.Web.CookieSchemaName,

                    Authority = Configuration.GetValue<string>($"OpenIdAuthorityUrl:{Environment.EnvironmentName}"),
                    PostLogoutRedirectUri = "/",

                    ClientId = "topcore_auth",
                    ClientSecret = "topcoreauth",

                    DisplayName = "Top Core Auth",
                    SaveTokens = true,
                    Scope =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                    }
                });

                // Middleware Identity Server
                app.UseIdentity();
                app.UseIdentityServer();
            }

            /// <summary>
            ///     Initial Seed Data and Database in Startup, Must be synchronous 
            /// </summary>
            /// <param name="services"></param>
            public static void SeedData(IServiceCollection services)
            {
                ISeedAuthService seedAuthService = services.Resolve<ISeedAuthService>();
                seedAuthService.SeedAuthDatabaseAsync().Wait();
            }
        }
    }
}