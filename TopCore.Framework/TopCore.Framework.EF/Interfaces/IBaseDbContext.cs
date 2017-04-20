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

        bool Any<T>() where T : class;

        bool Any<T>(Expression<Func<T, bool>> predicate) where T : class;

        int Count<T>() where T : class;

        int Count<T>(Expression<Func<T, bool>> predicate) where T : class;

        #endregion

        #region Where

        IQueryable<T> AllIncluding<T>(params Expression<Func<T, object>>[] includeProperties) where T : class;

        IQueryable<T> Where<T>(Expression<Func<T, bool>> predicate) where T : class;

        IQueryable<T> Where<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties) where T : class;

        T Single<T>(Expression<Func<T, bool>> predicate) where T : class;

        T Single<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties) where T : class;

        #endregion

        #region Add

        T Add<T>(T entity) where T : class;

        Task AddAsync<T>(T entity, CancellationToken cancellationToken = new CancellationToken()) where T : class;

        void AddRange<T>(params T[] entities) where T : class;

        void AddRange<T>(IEnumerable<T> entities) where T : class;

        Task AddRangeAsync<T>(params T[] entities) where T : class;

        Task AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = new CancellationToken()) where T : class;

        #endregion

        #region Update

        T Update<T>(T entity) where T : class;

        void UpdateRange<T>(params T[] entities) where T : class;

        void UpdateRange<T>(IEnumerable<T> entities) where T : class;

        #endregion

        #region Remove
        void Remove<T>(T entity) where T : class;

        void RemoveRange<T>(IEnumerable<T> entities) where T : class;

        void RemoveRange<T>(params T[] entities) where T : class;

        void RemoveWhere<T>(Expression<Func<T, bool>> predicate) where T : class;

        #endregion
    }
}