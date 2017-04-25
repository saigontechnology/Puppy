#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.WebAPI.Data.EF </Project>
//     <File>
//         <Name> Repository </Name>
//         <Created> 15/04/2017 08:18:58 AM </Created>
//         <Key> d785b888-9f8e-4f4d-aae2-b6b761fb77aa </Key>
//     </File>
//     <Summary>
//         Repository
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using TopCore.Framework.DependencyInjection.Attributes;
using TopCore.Framework.EF;

namespace TopCore.WebAPI.Data.EF
{
    [PerRequestDependency(ServiceType = typeof(IRepository<>))]
    public class Repository<TEntity> : BaseEntityRepository<TEntity>, IRepository<TEntity> where TEntity : class, IBaseEntity
    {
        public Repository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}