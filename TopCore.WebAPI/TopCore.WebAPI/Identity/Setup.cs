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

using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using TopCore.WebAPI.Identity.Dummy;
using TopCore.WebAPI.Identity.Models;

namespace TopCore.WebAPI.Identity
{
    public class Setup
    {
        public static void AddIdentityServer(IServiceCollection services, string connectionString, string migrationsAssembly)
        {
            // Add Identity store User into Database by Entity Framework

            // TopCoreIdentityDbContext for Asp Net Core Identity
            //services.AddDbContext<TopCoreIdentityDbContext>(options => options.UseSqlServer(connectionString));

            // Config and Operation store of Identity Server 4
            services.AddIdentity<TopCoreIdentityUser, IdentityRole>().AddEntityFrameworkStores<TopCoreIdentityDbContext>();
            services.AddIdentityServer()
                .AddTemporarySigningCredential()
                .AddAspNetIdentity<TopCoreIdentityUser>()
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

                Authority = "https://localhost:44375/",

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
                //serviceScope.ServiceProvider.GetRequiredService<TopCoreIdentityDbContext>().Database.Migrate();

                // PersistedGrantDbContext for Persisted Grant of Indeity Server 4
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                // ConfigurationDbContext for Configuration of Indeity Server 4
                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}