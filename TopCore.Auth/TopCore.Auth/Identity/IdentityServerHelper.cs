#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Auth </Project>
//     <File>
//         <Name> IdentityServerHelper </Name>
//         <Created> 02 Apr 17 11:20:26 PM </Created>
//         <Key> 4a81cf8a-d367-46ba-a3be-9083bfe9bccf </Key>
//     </File>
//     <Summary>
//         IdentityServerHelper
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using TopCore.Auth.Core.Models;

namespace TopCore.Auth.Identity
{
    public static class IdentityServerHelper
    {
        public static void Add(IServiceCollection services, string connectionString)
        {

            services.AddAuthentication(options => options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);

            // TopCore.TopCoreIdentityDbContext for Asp Net Core Identity
            //services.AddDbContext<TopCoreIdentityDbContext>(options => options.UseSqlServer(connectionString));

            // Add Identity store User into Database by Entity Framework
            //services.AddIdentity<UserEntity, IdentityRole>().AddEntityFrameworkStores<TopCoreIdentityDbContext>();

            var migrationsAssembly = typeof(DataModule).GetTypeInfo().Assembly.GetName().Name;

            // Config and Operation store of Identity Server 4
            services.AddIdentityServer(options =>
                {
                    options.UserInteraction.LoginUrl = "/login";
                    options.UserInteraction.LogoutUrl = "/logout";
                })
                .AddTemporarySigningCredential()
                //.AddSigningCredential()
                .AddConfigurationStore(
                    builder =>
                        builder.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationsAssembly)))
                .AddOperationalStore(
                    builder =>
                        builder.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationsAssembly)))
                //.AddAspNetIdentity<UserEntity>()
                ;
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

        //public static void InitData(IApplicationBuilder app)
        //{
        //    using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
        //    {
        //        // TopCore.TopCoreIdentityDbContext for Asp Net Core Identity
        //        var TopCoreIdentityDbContext = serviceScope.ServiceProvider.GetRequiredService<TopCoreIdentityDbContext>();

        //        // PersistedGrantDbContext for Persisted Grant of Identity Server 4
        //        var persistedGrantDbContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();

        //        // ConfigurationDbContext for Configuration of Identity Server 4
        //        var configurationDbContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

        //        // Execute Migrate
        //        TopCoreIdentityDbContext.Database.Migrate();
        //        persistedGrantDbContext.Database.Migrate();
        //        configurationDbContext.Database.Migrate();
        //    }
        //}
    }
}