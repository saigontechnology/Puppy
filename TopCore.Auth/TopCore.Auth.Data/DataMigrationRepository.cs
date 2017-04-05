#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Auth.Data </Project>
//     <File>
//         <Name> IDataMigrationRepository </Name>
//         <Created> 06 Apr 17 3:17:42 AM </Created>
//         <Key> d73570b1-db8c-4fed-9adb-d89df66b9920 </Key>
//     </File>
//     <Summary>
//         IDataMigrationRepository
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using TopCore.Auth.Domain.Data;
using TopCore.Framework.DependencyInjection.Attributes;

namespace TopCore.Auth.Data
{
    [PerRequestDependency(ServiceType = typeof(IDataMigrationRepository))]
	public class DataMigrationRepository: IDataMigrationRepository
	{
	    private readonly ITopCoreAuthDbContext _authDbContext;

	    public DataMigrationRepository(ITopCoreAuthDbContext authDbContext)
	    {
	        _authDbContext = authDbContext;
	    }

	    public void MigrateDatabase()
	    {
            // TODO _authDbContext
        }
    }
}