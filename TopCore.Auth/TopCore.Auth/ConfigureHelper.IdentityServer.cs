using IdentityServer4;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using TopCore.Auth.Data.Factory;
using TopCore.Auth.Domain.Entities;
using TopCore.Auth.Service;

namespace TopCore.Auth
{
    public static partial class ConfigureHelper
    {
        public static class IdentityServer
        {
            public static void Service(IServiceCollection services, string connectionString)
            {
                services.AddAuthentication(options => options.SignInScheme =
                    CookieAuthenticationDefaults.AuthenticationScheme);

                var migrationsAssembly = typeof(IDataModule).GetTypeInfo().Assembly.GetName().Name;

                services.AddDbContext<Data.DbContext>(builder => builder.UseSqlServer(connectionString,
                    options => options.MigrationsAssembly(migrationsAssembly)));

                // Service Identity store User into Database by Entity Framework (AspNetUser)
                services.AddIdentity<User, IdentityRole>(o =>
                    {
                        o.Password.RequireDigit = false;
                        o.Password.RequireLowercase = false;
                        o.Password.RequireNonAlphanumeric = false;
                        o.Password.RequireUppercase = false;
                        o.Password.RequiredLength = ConfigurationRoot.GetValue<int>("UserSecurity:PasswordRequiredLength");
                        o.Cookies.ApplicationCookie.AuthenticationScheme = Domain.Constants.System.CookieSchemaName;
                    })
                    .AddEntityFrameworkStores<Data.DbContext>()
                    .AddDefaultTokenProviders();

                // Config and Operation store of Identity Server 4
                services.AddIdentityServer(options =>
                    {
                        options.UserInteraction.LoginUrl = "/login";
                        options.UserInteraction.LogoutUrl = "/logout";
                        options.Authentication.AuthenticationScheme = Domain.Constants.System.CookieSchemaName;
                    })
                    .AddTemporarySigningCredential()
                    //.AddSigningCredential() // TODO Service Later
                    .AddConfigurationStore(builder => builder.UseSqlServer(connectionString,
                        options => options.MigrationsAssembly(migrationsAssembly)))
                    .AddOperationalStore(builder => builder.UseSqlServer(connectionString,
                        options => options.MigrationsAssembly(migrationsAssembly)))
                    .AddAspNetIdentity<User>()
                    .AddProfileService<ProfileService>()
                    .AddResourceOwnerValidator<ResourceOwnerPasswordService>();
            }

            public static void Middleware(IApplicationBuilder app)
            {
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

                //application to use cookie authentication
                app.UseCookieAuthentication(new CookieAuthenticationOptions
                {
                    AuthenticationScheme = Domain.Constants.System.CookieSchemaName,
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
                    SignInScheme = Domain.Constants.System.CookieSchemaName,

                    Authority = ConfigurationRoot.GetValue<string>($"OpenIdAuthorityUrl:{Environment.EnvironmentName}"),
                    PostLogoutRedirectUri = "/",
                    AutomaticAuthenticate = true,
                    ClientId = "topcore_auth",
                    ClientSecret = "topcoreauth",
                    DisplayName = "Top Core Auth",
                    // required if you want to return a 403 and not a 401 for forbidden responses
                    AutomaticChallenge = true,
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
        }
    }
}