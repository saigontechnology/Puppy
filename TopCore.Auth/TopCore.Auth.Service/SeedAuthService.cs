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

using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
        private readonly IRepository<ApiResourceEntity> _apiResourceRepository;
        private readonly IRepository<ClientEntity> _clientRepository;
        private readonly IDbContext _dbContext;
        private readonly IRepository<IdentityResourceEntity> _identityResourceRepository;
        private readonly IRepository<UserEntity> _userRepository;

        public SeedAuthService(IDbContext dbContext,
            IRepository<UserEntity> userRepository,
            IRepository<ClientEntity> clientRepository,
            IRepository<ApiResourceEntity> apiResourceRepository,
            IRepository<IdentityResourceEntity> identityResourceRepository)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
            _clientRepository = clientRepository;
            _apiResourceRepository = apiResourceRepository;
            _identityResourceRepository = identityResourceRepository;
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
            var webClient = new ClientEntity
            {
                ClientId = "topcoreweb",
                ClientName = "TopCore Web",
                AccessTokenType = (int)AccessTokenType.Jwt,
                ClientSecrets = new List<ClientSecret>
                {
                    new ClientSecret
                    {
                       Value = "topcoreweb".Sha256()
                    }
                },
                AllowedScopes = new List<ClientScope>
                {
                    new ClientScope
                    {
                        Scope =  "api"
                    }
                },
            };

            var isExist = _clientRepository.Any(x => x.ClientName == webClient.ClientName);

            if (!isExist)
                _clientRepository.Add(webClient);
        }

        private void SeedScope_APIResource()
        {
            var apiResource = new ApiResourceEntity
            {
                Name = "topcoreapi",
                DisplayName = "TopCore API"
            };

            var isExist = _apiResourceRepository.Any(x => x.Name == apiResource.Name);

            if (!isExist)
                _apiResourceRepository.Add(apiResource);
        }

        private void SeedScope_IdentityResource()
        {
            //"profile", "Profile", new List<string>
            //{
            //    "name", "email"
            //}
            var identityResource = new IdentityResourceEntity
            {
                Name = "profile",
                DisplayName = "Profile",
                UserClaims = new List<IdentityClaim>
                {
                    new IdentityClaim
                    {
                        Type = "name"
                    },
                    new IdentityClaim
                    {
                        Type = "email"
                    }
                }
            };

            var isExist = _identityResourceRepository.Any(x => x.Name == identityResource.Name);

            if (!isExist)
                _identityResourceRepository.Add(identityResource);
        }
    }
}