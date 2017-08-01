#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → TopCore </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy → Interface </Project>
//     <File>
//         <Name> IPuppy.EF.Repository.cs </Name>
//         <Created> 23 Apr 17 3:47:20 PM </Created>
//         <Key> 3e4a5d95-7957-4305-ad6f-b4452d8ccfda </Key>
//     </File>
//     <Summary>
//         IPuppy.EF.Repository.cs
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
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Include(params Expression<Func<T, object>>[] includeProperties);

        IQueryable<T> Get(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties);

        T GetSingle(Expression<Func<T, bool>> predicate = null, params Expression<Func<T, object>>[] includeProperties);

        T Add(T entity);

        void Update(T entity, params Expression<Func<T, object>>[] changedProperties);

        void Delete(T entity);

        void DeleteWhere(Expression<Func<T, bool>> predicate);

        void RefreshEntity(T entity);

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