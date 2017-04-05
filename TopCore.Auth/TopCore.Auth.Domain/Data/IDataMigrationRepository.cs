#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Auth.Data.Interfaces </Project>
//     <File>
//         <Name> IDataMigrationRepository </Name>
//         <Created> 06 Apr 17 2:28:18 AM </Created>
//         <Key> d5e0adc1-20c8-40c3-9c6a-95bc42778492 </Key>
//     </File>
//     <Summary>
//         IDataMigrationRepository
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

namespace TopCore.Auth.Domain.Data
{
	public interface IDataMigrationRepository
	{
	    void MigrateDatabase();
    }
}