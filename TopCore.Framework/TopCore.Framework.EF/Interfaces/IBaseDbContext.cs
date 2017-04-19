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
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace TopCore.Framework.EF.Interfaces
{
    public interface IBaseDbContext : IDisposable
    {
        #region Database

        /// <summary>
        ///     <para> Ensures that the database for the context exists. If it exists, no action is taken. If it does not exist then the database and all its schema are created. If the database exists, then no effort is made to ensure it is compatible with the model for this context. </para>
        ///     <para> Note that this API does not use migrations to create the database. In addition, the database that is created cannot be later updated using migrations. If you are targeting a relational database and using migrations, you can use the DbContext.Database.Migrate() method to ensure the database is created and all migrations are applied. </para>
        /// </summary>
        /// <returns> True if the database is created, false if it already existed. </returns>
        bool EnsureDatabaseCreated();

        /// <summary>
        ///     <para> Asynchronously ensures that the database for the context exists. If it exists, no action is taken. If it does not exist then the database and all its schema are created. If the database exists, then no effort is made to ensure it is compatible with the model for this context. </para>
        ///     <para> Note that this API does not use migrations to create the database. In addition, the database that is created cannot be later updated using migrations. If you are targeting a relational database and using migrations, you can use the DbContext.Database.Migrate() method to ensure the database is created and all migrations are applied. </para>
        /// </summary>
        /// <param name="cancellationToken"> A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete. </param>
        /// <returns> A task that represents the asynchronous save operation. The task result contains true if the database is created, false if it already existed. </returns>
        Task<bool> EnsureDatabaseCreatedAsync(CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        ///     <para> Ensures that the database for the context does not exist. If it does not exist, no action is taken. If it does exist then the database is deleted. </para>
        ///     <para> Warning: The entire database is deleted an no effort is made to remove just the database objects that are used by the model for this context. </para>
        /// </summary>
        /// <returns> True if the database is deleted, false if it did not exist. </returns>
        bool EnsureDatabaseDeleted();

        /// <summary>
        ///     <para> Asynchronously ensures that the database for the context does not exist. If it does not exist, no action is taken. If it does exist then the database is deleted. </para>
        ///     <para> Warning: The entire database is deleted an no effort is made to remove just the database objects that are used by the model for this context. </para>
        /// </summary>
        /// <param name="cancellationToken"> A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete. </param>
        /// <returns> A task that represents the asynchronous save operation. The task result contains true if the database is deleted, false if it did not exist. </returns>
        Task<bool> EnsureDatabaseDeletedAsync(CancellationToken cancellationToken = new CancellationToken());

        void MigrateDatabase();

        Task MigrateDatabaseAsync(CancellationToken cancellationToken = new CancellationToken());

        #endregion

        #region Save Change

        [DebuggerStepThrough]
        int SaveChanges();

        [DebuggerStepThrough]
        int SaveChanges(bool acceptAllChangesOnSuccess);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken());

        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken());

        #endregion

        #region Count and Any

        bool Any<TEntity>() where TEntity : class;

        bool Any<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

        int Count<TEntity>() where TEntity : class;

        int Count<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

        #endregion

        #region Get

        IQueryable<TEntity> AllIncluding<TEntity>(params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class;

        IQueryable<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false) where TEntity : class, IEntityBase;

        IQueryable<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class, IEntityBase;

        TEntity GetSingle<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false) where TEntity : class, IEntityBase;

        TEntity GetSingle<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class, IEntityBase;

        #endregion

        #region Add

        TEntity Add<TEntity>(TEntity entity) where TEntity : class;

        Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class;

        void AddRange<TEntity>(params TEntity[] entities) where TEntity : class;

        void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        Task AddRangeAsync<TEntity>(params TEntity[] entities) where TEntity : class;

        Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class;

        #endregion

        #region Update

        TEntity Update<TEntity>(TEntity entity) where TEntity : class;

        void UpdateRange<TEntity>(params TEntity[] entities) where TEntity : class;

        void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        #endregion

        #region Remove

        void RemoveWhere<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isPhysicalDelete = false) where TEntity : class, IEntityBase;

        TEntity Remove<TEntity>(TEntity entity, bool isPhysicalDelete = false) where TEntity : class, IEntityBase;

        void RemoveRange<TEntity>(ICollection<TEntity> entities, bool isPhysicalDelete = false) where TEntity : class, IEntityBase;

        void RemoveRange<TEntity>(bool isPhysicalDelete = false, params TEntity[] entities) where TEntity : class, IEntityBase;

        void RemoveRange<TEntity>(params TEntity[] entities) where TEntity : class;

        void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

        #endregion
    }
}