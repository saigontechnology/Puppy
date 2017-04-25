#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.WebAPI.Data </Project>
//     <File> 
//         <Name> IRepository </Name>
//         <Created> 15/04/2017 08:18:25 AM </Created>
//         <Key> 00994344-338a-4817-b1e0-9cf1eb6896d7 </Key>
//     </File>
//     <Summary>
//         IRepository
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using TopCore.Framework.EF;
using TopCore.Framework.EF.Interfaces;

namespace TopCore.WebAPI.Data
{
    public interface IRepository<TEntity> : IBaseEntityRepository<TEntity> where TEntity : class, IBaseEntity
    {
    }
}

