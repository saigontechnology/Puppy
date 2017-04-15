#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.WebAPI.Service </Project>
//     <File> 
//         <Name> IMigrateService </Name>
//         <Created> 15/04/2017 11:29:32 PM </Created>
//         <Key> 026bf968-191c-4605-bbae-f3a14c5d4796 </Key>
//     </File>
//     <Summary>
//         IMigrateService
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System.Threading.Tasks;

namespace TopCore.WebAPI.Service
{
    public interface IMigrationService
    {
        Task MigrateDatabase();
    }
}

