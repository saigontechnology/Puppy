#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy → Interface </Project>
//     <File>
//         <Name> IEntityRepository.cs </Name>
//         <Created> 23 Apr 17 3:55:08 PM </Created>
//         <Key> b47adcbd-ac4a-4f10-8be1-e391588aafe4 </Key>
//     </File>
//     <Summary>
//         IEntityRepository.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Puppy.EF.Interfaces.Repository
{
    public interface IEntityBaseRepository<TEntity> where TEntity : EntityBase
    {
        IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includeProperties);

        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate = null, bool isIncludeDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties);

        TEntity GetSingle(Expression<Func<TEntity, bool>> predicate = null, bool isIncludeDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties);

        TEntity Add(TEntity entity);

        void Update(TEntity entity, params Expression<Func<TEntity, object>>[] changedProperties);

        void Delete(TEntity entity, bool isPhysicalDelete = false);

        void DeleteWhere(Expression<Func<TEntity, bool>> predicate, bool isPhysicalDelete = false);

        void RefreshEntity(TEntity entity);

        [DebuggerStepThrough]
        int SaveChanges();

        [DebuggerStepThrough]
        int SaveChanges(bool acceptAllChangesOnSuccess);

        [DebuggerStepThrough]
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        [DebuggerStepThrough]
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));
    }
}