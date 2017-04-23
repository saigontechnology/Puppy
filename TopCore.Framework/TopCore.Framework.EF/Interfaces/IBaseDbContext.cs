#region	License

//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Framework.EF.Interfaces </Project>
//     <File>
//         <Name> IBaseDbContext </Name>
//         <Created> 06 Apr 17 1:18:11 PM </Created>
//         <Key> 87e0de9a-30e5-4263-bb7d-198f651df622 </Key>
//     </File>
//     <Summary>
//         IBaseDbContext
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------

#endregion License

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TopCore.Framework.EF.Interfaces
{
    public interface IBaseDbContext : IDisposable, IInfrastructure<IServiceProvider>
    {
        DatabaseFacade Database { get; }

        ChangeTracker ChangeTracker { get; }

        IModel Model { get; }

        [DebuggerStepThrough]
        int SaveChanges();

        [DebuggerStepThrough]
        int SaveChanges(bool acceptAllChangesOnSuccess);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken));

        void StandardizeSaveChangeData(ChangeTracker changeTracker);

        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        EntityEntry Entry(object entity);

        EntityEntry<TEntity> Add<TEntity>(TEntity entity) where TEntity : class;

        Task<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity,
            CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class;

        EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class;

        EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : class;

        EntityEntry<TEntity> Remove<TEntity>(TEntity entity) where TEntity : class;

        EntityEntry Add(object entity);

        Task<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default(CancellationToken));

        EntityEntry Attach(object entity);

        EntityEntry Update(object entity);

        EntityEntry Remove(object entity);

        void AddRange(params object[] entities);

        Task AddRangeAsync(params object[] entities);

        void AttachRange(params object[] entities);

        void UpdateRange(params object[] entities);

        void RemoveRange(params object[] entities);

        void AddRange(IEnumerable<object> entities);

        Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = new CancellationToken());

        void AttachRange(IEnumerable<object> entities);

        void UpdateRange(IEnumerable<object> entities);

        void RemoveRange(IEnumerable<object> entities);

        DbSet<TEntity> DbSet<TEntity>() where TEntity : class;

        object Find(Type entityType, params object[] keyValues);

        Task<object> FindAsync(Type entityType, params object[] keyValues);

        Task<object> FindAsync(Type entityType, object[] keyValues, CancellationToken cancellationToken);

        TEntity Find<TEntity>(params object[] keyValues) where TEntity : class;

        Task<TEntity> FindAsync<TEntity>(params object[] keyValues) where TEntity : class;

        Task<TEntity> FindAsync<TEntity>(object[] keyValues, CancellationToken cancellationToken) where TEntity : class;
    }
}