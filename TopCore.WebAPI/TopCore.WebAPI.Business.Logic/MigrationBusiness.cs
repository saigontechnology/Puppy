#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.WebAPI.Business.Logic </Project>
//     <File>
//         <Name> MigrationBusiness </Name>
//         <Created> 15/04/2017 11:40:43 PM </Created>
//         <Key> a2ebdc7b-1a40-4e5d-b937-226eeff9c32a </Key>
//     </File>
//     <Summary>
//         MigrationBusiness
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TopCore.Framework.DependencyInjection.Attributes;
using TopCore.WebAPI.Data;

namespace TopCore.WebAPI.Business.Logic
{
    [PerRequestDependency(ServiceType = typeof(IMigrationBusiness))]
    public class MigrationBusiness : IMigrationBusiness
    {
        private readonly IDbContext _dbContext;

        public MigrationBusiness(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task MigrateDatabase()
        {
            _dbContext.Database.MigrateAsync();

            return Task.FromResult(0);
        }
    }
}