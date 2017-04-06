#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Auth.Service </Project>
//     <File>
//         <Name> DataMigrationService </Name>
//         <Created> 06 Apr 17 2:28:35 AM </Created>
//         <Key> 6d71a61d-7d35-4fc3-ba5f-fea3036f6250 </Key>
//     </File>
//     <Summary>
//         DataMigrationService
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TopCore.Auth.Domain.Data;
using TopCore.Framework.DependencyInjection.Attributes;

namespace TopCore.Auth.Service
{
    [PerRequestDependency(ServiceType = typeof(Domain.Services.IDataMigrationService))]
    public class DataMigrationService : Domain.Services.IDataMigrationService
    {
        private readonly IDbContext _dbContext;

        public DataMigrationService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task MigrateDatabase()
        {
            return _dbContext.Database.MigrateAsync();
        }
    }
}