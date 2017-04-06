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

using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using IdentityServer4.Models;
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

        public SeedAuthService(IDbContext dbContext, IRepository<UserEntity> userRepository)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
        }

        public Task SeedAuthDatabase()
        {
            Task migrate = _dbContext.Database.MigrateAsync();

            migrate.Wait();

            UserEntity topnguyen = new UserEntity
            {
                UserName = "topnguyen",
                PasswordHash = "123456".Sha512(),
                Email = "topnguyen92@gmail.com",
                EmailConfirmed = true,
                PhoneNumber = "0945188299",
                PhoneNumberConfirmed = true,
            };

            bool isExistTopNguyen = _userRepository.Any(x => x.UserName == topnguyen.UserName);

            if (!isExistTopNguyen)
            {
                _userRepository.Add(topnguyen);
            }

            return migrate;
        }
    }
}