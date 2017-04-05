#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Auth.Service </Project>
//     <File>
//         <Name> InitialDataService </Name>
//         <Created> 06 Apr 17 2:28:35 AM </Created>
//         <Key> 6d71a61d-7d35-4fc3-ba5f-fea3036f6250 </Key>
//     </File>
//     <Summary>
//         InitialDataService
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using TopCore.Auth.Data;
using TopCore.Framework.DependencyInjection.Attributes;

namespace TopCore.Auth.Service
{
    [PerRequestDependency(ServiceType = typeof(Domain.Services.IDataMigrationService))]
    public class IDataMigrationService : Domain.Services.IDataMigrationService
    {
        private readonly IDataMigrationRepository _dataMigrationRepository;

        public IDataMigrationService(IDataMigrationRepository dataMigrationRepository)
        {
            _dataMigrationRepository = dataMigrationRepository;
        }


        public void MigrateDatabase()
        {
            _dataMigrationRepository.MigrateDatabase();

        }
    }
}