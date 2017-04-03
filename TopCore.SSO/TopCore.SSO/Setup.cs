#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Core.Identity </Project>
//     <File>
//         <Name> Setup </Name>
//         <Created> 02 Apr 17 11:20:26 PM </Created>
//         <Key> 4a81cf8a-d367-46ba-a3be-9083bfe9bccf </Key>
//     </File>
//     <Summary>
//         Setup
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using TopCore.SSO.Dummy;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.EntityFramework.DbContexts;
using TopCore.SSO.Models;

namespace TopCore.SSO
{
    public class Setup
    {
        public static void AddIdentityServer(IServiceCollection services, string connectionString, string migrationsAssembly)
        {
            // TopCoreIdentityDbContext for Asp Net Core Identity
            services.AddDbContext<TopCoreIdentityDbContext>(options => options.UseSqlServer(connectionString));

            // Add Identity store User into Database by Entity Framework
            services.AddIdentity<TopCoreIdentityUser, IdentityRole>().AddEntityFrameworkStores<TopCoreIdentityDbContext>();

            // Config and Operation store of Identity Server 4
            services.AddIdentityServer()
                .AddAspNetIdentity<TopCoreIdentityUser>()
                .AddTemporarySigningCredential()
                .AddConfigurationStore(builder => builder.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationsAssembly)))
                .AddOperationalStore(builder => builder.UseSqlServer(connectionString, options => options.MigrationsAssembly(migrationsAssembly)));
        }

        public static void UseIdentityServer(IApplicationBuilder app)
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

                Authority = "https://localhost:55555/",

                RequireHttpsMetadata = false,

                ClientId = "mvc",

                SaveTokens = true
            });

            app.UseIdentity();
            app.UseIdentityServer();

            InitialIdentityDatabase(app);
        }

        public static void InitialIdentityDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                // TopCoreIdentityDbContext for Asp Net Core Identity
                var topCoreIdentityDbContext = serviceScope.ServiceProvider.GetRequiredService<TopCoreIdentityDbContext>();

                // PersistedGrantDbContext for Persisted Grant of Identity Server 4
                var persistedGrantDbContext = serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();

                // ConfigurationDbContext for Configuration of Identity Server 4
                var configurationDbContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                // Execute Migrate
                topCoreIdentityDbContext.Database.Migrate();
                persistedGrantDbContext.Database.Migrate();
                configurationDbContext.Database.Migrate();

                // Initial Data
                foreach (var client in Dummy.Dummy.GetClients())
                {
                    if (configurationDbContext.Clients.FirstOrDefault(c => c.ClientId == client.ClientId) == null)
                        configurationDbContext.Clients.Add(client.ToEntity());
                }
                configurationDbContext.SaveChanges();

                if (!configurationDbContext.IdentityResources.Any())
                {
                    foreach (var resource in Dummy.Dummy.GetIdentityResources())
                    {
                        configurationDbContext.IdentityResources.Add(resource.ToEntity());
                    }
                    configurationDbContext.SaveChanges();
                }

                if (!configurationDbContext.ApiResources.Any())
                {
                    foreach (var resource in Dummy.Dummy.GetApiResources())
                    {
                        configurationDbContext.ApiResources.Add(resource.ToEntity());
                    }
                    configurationDbContext.SaveChanges();
                }

                var usermanager = serviceScope.ServiceProvider.GetRequiredService<UserManager<TopCoreIdentityUser>>();

                if (!usermanager.Users.Any())
                {
                    foreach (var topCoreIdentityUser in Dummy.Dummy.GetUsers())
                    {
                        usermanager.CreateAsync(topCoreIdentityUser, topCoreIdentityUser.Password).Wait();
                    }
                }
            }
        }
    }
}