#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Auth.Service </Project>
//     <File>
//         <Name> SeedAuthService </Name>
//         <Created> 06 Apr 17 2:28:35 AM </Created>
//         <Key> 6d71a61d-7d35-4fc3-ba5f-fea3036f6250 </Key>
//     </File>
//     <Summary>
//         SeekAuthService
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopCore.Auth.Domain.Data;
using TopCore.Auth.Domain.Entities;
using TopCore.Auth.Domain.Services;
using TopCore.Framework.DependencyInjection.Attributes;

namespace TopCore.Auth.Service
{
    [PerRequestDependency(ServiceType = typeof(ISeedAuthService))]
    public class SeedAuthService : ISeedAuthService
    {
        private readonly IDbContext _dbContext;
        private readonly IRepository<UserEntity> _userRepository;

        //private readonly PersistedGrantDbContext _persistedGrantDbContext;
        private readonly ConfigurationDbContext _configurationDbContext;

        public SeedAuthService(IDbContext dbContext,
            IRepository<UserEntity> userRepository,
            //PersistedGrantDbContext persistedGrantDbContext,
            ConfigurationDbContext configurationDbContext)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
            //_persistedGrantDbContext = persistedGrantDbContext;
            _configurationDbContext = configurationDbContext;
        }

        public Task SeedAuthDatabase()
        {
            // Run migrate first
            var migrate = _dbContext.Database.MigrateAsync();
            migrate.Wait();

            SeedScope_IdentityResource();
            SeedScope_APIResource();
            SeedClient();
            SeedUser();

            return migrate;
        }

        private void SeedUser()
        {
            var user = new UserEntity
            {
                UserName = "topnguyen",
                PasswordHash = "123456".Sha512(),

                Email = "topnguyen92@gmail.com",
                EmailConfirmed = true,

                PhoneNumber = "0945188299",
                PhoneNumberConfirmed = true,

                Claims =
                {
                    new IdentityUserClaim<string>
                    {
                        ClaimType = "name",
                        ClaimValue = "Top Nguyen"
                    },
                    new IdentityUserClaim<string>
                    {
                        ClaimType = "email",
                        ClaimValue = "topnguyen92@gmail.com"
                    }
                }
            };

            var isExist = _userRepository.Any(x => x.UserName == user.UserName);

            if (!isExist)
                _userRepository.Add(user);
        }

        private void SeedClient()
        {
            var webClient = new IdentityServer4.EntityFramework.Entities.Client
            {
                Enabled = true,
                ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
                AccessTokenType = (int)AccessTokenType.Jwt,
                ClientId = "topcore_web",
                ClientName = "TopCore Web",
                ClientSecrets = new List<ClientSecret>
                {
                    new ClientSecret
                    {
                        Value = "topcoreweb".Sha256(),
                    }
                },
                AllowedGrantTypes = new List<ClientGrantType>
                {
                    new ClientGrantType
                    {
                        GrantType = GrantType.ResourceOwnerPassword
                    }
                },
                AllowedScopes = new List<ClientScope>
                {
                    new ClientScope
                    {
                        Scope =  IdentityServerConstants.StandardScopes.OpenId,
                    },
                    new ClientScope
                    {
                        Scope =  IdentityServerConstants.StandardScopes.Profile,
                    },
                    new ClientScope
                    {
                        Scope =  IdentityServerConstants.StandardScopes.OfflineAccess,
                    },
                    new ClientScope
                    {
                        Scope =  "topcore_api"
                    },
                }
            };

            var isExist = _configurationDbContext.Clients.Any(x => x.ClientId == webClient.ClientId);

            if (!isExist)
            {
                _configurationDbContext.Clients.Add(webClient);
                _configurationDbContext.SaveChanges();
            }
        }

        private void SeedScope_APIResource()
        {
            var apiResource = new IdentityServer4.EntityFramework.Entities.ApiResource
            {
                Name = "topcore_api",
                DisplayName = "TopCore API"
            };

            var isExist = _configurationDbContext.ApiResources.Any(x => x.Name == apiResource.Name);

            if (!isExist)
            {
                _configurationDbContext.ApiResources.Add(apiResource);
                _configurationDbContext.SaveChanges();
            }
        }

        private void SeedScope_IdentityResource()
        {
            var identityResource = new IdentityServer4.EntityFramework.Entities.IdentityResource
            {
                Name = IdentityServerConstants.StandardScopes.Profile,
                DisplayName = "Profile",
            };

            var isExist = _configurationDbContext.IdentityResources.Any(x => x.Name == identityResource.Name);

            if (!isExist)
            {
                _configurationDbContext.IdentityResources.Add(identityResource);

                _configurationDbContext.SaveChanges();
            }
        }
    }
}