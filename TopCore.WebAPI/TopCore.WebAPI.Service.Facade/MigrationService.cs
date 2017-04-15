#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.WebAPI.Service.Facade </Project>
//     <File>
//         <Name> MigrationService </Name>
//         <Created> 15/04/2017 11:47:18 PM </Created>
//         <Key> 987bd6a4-8ba8-4631-8c57-20001994aca9 </Key>
//     </File>
//     <Summary>
//         MigrationService
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System.Threading.Tasks;
using TopCore.Framework.DependencyInjection.Attributes;
using TopCore.WebAPI.Business;

namespace TopCore.WebAPI.Service.Facade
{
    [PerRequestDependency(ServiceType = typeof(IMigrationService))]
    public class MigrationService : IMigrationService
    {
        private readonly IMigrationBusiness _migrationBusiness;

        public MigrationService(IMigrationBusiness migrationBusiness)
        {
            _migrationBusiness = migrationBusiness;
        }

        public Task MigrateDatabase()
        {
            return _migrationBusiness.MigrateDatabase();
        }
    }
}