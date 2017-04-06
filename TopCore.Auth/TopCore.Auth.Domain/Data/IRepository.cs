#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Auth.Domain.Data </Project>
//     <File>
//         <Name> IRepository </Name>
//         <Created> 06 Apr 17 1:10:50 PM </Created>
//         <Key> 555866df-387c-45fc-9832-32b572d932eb </Key>
//     </File>
//     <Summary>
//         IRepository
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using TopCore.Framework.EF;
using TopCore.Framework.EF.Interfaces;

namespace TopCore.Auth.Domain.Data
{
    public interface IRepository<TEntity> : IBaseRepository<TEntity> where TEntity : EntityBase
    {
    }
}