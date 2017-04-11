#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Auth.Data </Project>
//     <File>
//         <Name> Repository </Name>
//         <Created> 06 Apr 17 1:10:29 PM </Created>
//         <Key> f6b6d77b-b212-4ca2-96d1-1d029ef23e04 </Key>
//     </File>
//     <Summary>
//         Repository
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using TopCore.Auth.Domain.Interfaces.Data;
using TopCore.Framework.DependencyInjection.Attributes;
using TopCore.Framework.EF;

namespace TopCore.Auth.Data
{
    [PerRequestDependency(ServiceType = typeof(IRepository<>))]
    public class Repository<TEntity> : BaseRepository<TEntity>, IRepository<TEntity> where TEntity : class
    {
        public Repository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}