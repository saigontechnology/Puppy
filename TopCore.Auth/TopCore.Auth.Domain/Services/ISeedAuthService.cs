#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Auth.Service.Interfaces </Project>
//     <File>
//         <Name> IInitialData </Name>
//         <Created> 06 Apr 17 2:26:51 AM </Created>
//         <Key> 8cf12999-1e5b-4d40-8d0e-90c7b572cf99 </Key>
//     </File>
//     <Summary>
//         IInitialData
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System.Threading.Tasks;

namespace TopCore.Auth.Domain.Services
{
    public interface ISeedAuthService
    {
        Task SeedAuthDatabase();
    }
}