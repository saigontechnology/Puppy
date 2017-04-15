#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.WebAPI.Business </Project>
//     <File> 
//         <Name> IMigrationBusiness </Name>
//         <Created> 15/04/2017 11:39:02 PM </Created>
//         <Key> 45712db1-5588-4541-b69a-dbad62a55716 </Key>
//     </File>
//     <Summary>
//         IMigrationBusiness
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System.Threading.Tasks;

namespace TopCore.WebAPI.Business
{
    public interface IMigrationBusiness
    {
        Task MigrateDatabase();
    }
}

