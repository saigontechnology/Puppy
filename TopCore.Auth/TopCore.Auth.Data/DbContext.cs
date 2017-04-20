#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Auth.Data </Project>
//     <File>
//         <Name> TopCoreAuthDbContext </Name>
//         <Created> 06 Apr 17 1:09:03 AM </Created>
//         <Key> 698007b1-38ad-45b0-b7d0-f22069fe2fab </Key>
//     </File>
//     <Summary>
//         DbContext
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TopCore.Auth.Data.Factory;
using TopCore.Auth.Domain.Entities;
using TopCore.Auth.Domain.Interfaces.Data;
using TopCore.Framework.Core;
using TopCore.Framework.DependencyInjection.Attributes;
using TopCore.Framework.EF;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace TopCore.Auth.Data
{
    [PerRequestDependency(ServiceType = typeof(IDbContext))]
    public class DbContext : IdentityDbContext<User>, IDbContext, IConfigurationDbContext, IPersistedGrantDbContext
    {
        private ConfigurationStoreOptions _configurationStoreOptions;

        private OperationalStoreOptions _operationalStoreOptions;

        public DbContext()
        {
        }

        public DbContext(DbContextOptions<DbContext> options, ConfigurationStoreOptions configurationStoreOptions, OperationalStoreOptions operationalStoreOptions) : base(options)
        {
            _configurationStoreOptions = configurationStoreOptions ?? throw new ArgumentNullException(nameof(configurationStoreOptions));
            _operationalStoreOptions = operationalStoreOptions ?? throw new ArgumentNullException(nameof(operationalStoreOptions));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                string connectionString = ConfigHelper.GetValue("appsettings.json", $"ConnectionStrings:{environmentName}");
                optionsBuilder.UseSqlServer(connectionString, o => o.MigrationsAssembly(typeof(IDataModule).GetTypeInfo().Assembly.GetName().Name));

                _configurationStoreOptions = new ConfigurationStoreOptions
                {
                    DefaultSchema = "dbo"
                };

                _operationalStoreOptions = new OperationalStoreOptions
                {
                    DefaultSchema = "dbo"
                };
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureClientContext(_configurationStoreOptions);
            modelBuilder.ConfigureResourcesContext(_configurationStoreOptions);
            modelBuilder.ConfigurePersistedGrantContext(_operationalStoreOptions);
            base.OnModelCreating(modelBuilder);
        }

        #region DbSet

        public DbSet<Client> Clients { get; set; }

        public DbSet<IdentityResource> IdentityResources { get; set; }

        public DbSet<ApiResource> ApiResources { get; set; }

        public DbSet<PersistedGrant> PersistedGrants { get; set; }

        #endregion

        #region Database

        public bool EnsureDatabaseCreated()
        {
            return Database.EnsureCreated();
        }

        public Task<bool> EnsureDatabaseCreatedAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return Database.EnsureCreatedAsync(cancellationToken);
        }

        public bool EnsureDatabaseDeleted()
        {
            return Database.EnsureDeleted();
        }

        public Task<bool> EnsureDatabaseDeletedAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return Database.EnsureDeletedAsync(cancellationToken);
        }

        public void MigrateDatabase()
        {
            Database.Migrate();
        }

        public Task MigrateDatabaseAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return Database.MigrateAsync(cancellationToken);
        }

        #endregion

        #region Save Changes

        public override int SaveChanges()
        {
            StandardizeSaveChangeData(ChangeTracker);
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            StandardizeSaveChangeData(ChangeTracker);
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public Task<int> SaveChangesAsync()
        {
            StandardizeSaveChangeData(ChangeTracker);
            return SaveChangesAsync(new CancellationToken());
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            StandardizeSaveChangeData(ChangeTracker);
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public void StandardizeSaveChangeData(ChangeTracker changeTracker)
        {
            var entities = changeTracker.Entries().Where(x => x.Entity is IBaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted));

            DateTime utcNow = DateTime.UtcNow;

            foreach (var entity in entities)
            {
                switch (entity.State)
                {
                    case EntityState.Added:
                    {
                        entity.Property(nameof(IBaseEntity.CreatedOnUtc)).CurrentValue = utcNow;
                        entity.Property(nameof(IBaseEntity.LastUpdatedOnUtc)).CurrentValue = null;
                        entity.Property(nameof(IBaseEntity.IsDeleted)).CurrentValue = false;
                        entity.Property(nameof(IBaseEntity.DeletedOnUtc)).CurrentValue = null;
                        break;
                    }
                    case EntityState.Modified:
                    {
                        entity.Property(nameof(IBaseEntity.LastUpdatedOnUtc)).CurrentValue = utcNow;
                        break;
                    }
                }
            }
        }

        #endregion

        #region Count and Any

        public bool Any<T>() where T : class
        {
            return Set<T>().Any();
        }

        public bool Any<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return Set<T>().Any(predicate);
        }

        public int Count<T>() where T : class
        {
            return Set<T>().Count();
        }

        public int Count<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return Set<T>().Count(predicate);
        }

        #endregion

        #region Where

        public IQueryable<T> AllIncluding<T>(params Expression<Func<T, object>>[] includeProperties) where T : class
        {
            IQueryable<T> query = Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public IQueryable<T> Where<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return Set<T>().Where(predicate);
        }

        public IQueryable<T> Where<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties) where T : class
        {
            return AllIncluding(includeProperties).Where(predicate);
        }

        public T Single<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return Where(predicate).FirstOrDefault();
        }

        public T Single<T>(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties) where T : class
        {
            return Where(predicate, includeProperties).FirstOrDefault();
        }

        #endregion

        #region Get

        public IQueryable<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false) where TEntity : class, IBaseEntity
        {
            IQueryable<TEntity> query = Set<TEntity>().Where(predicate);
            return isIncludeDeleted ? query : query.Where(x => !x.IsDeleted);
        }

        public IQueryable<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class, IBaseEntity
        {
            IQueryable<TEntity> query = Get(predicate, isIncludeDeleted);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public TEntity GetSingle<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false) where TEntity : class, IBaseEntity
        {
            return Get(predicate, isIncludeDeleted).FirstOrDefault();
        }

        public TEntity GetSingle<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class, IBaseEntity
        {
            return Get(predicate, isIncludeDeleted, includeProperties).FirstOrDefault();
        }

        #endregion

        #region Add

        public new T Add<T>(T entity) where T : class
        {
            entity = Set<T>().Add(entity).Entity;
            return entity;
        }

        public new Task AddAsync<T>(T entity, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            return Set<T>().AddAsync(entity, cancellationToken);
        }

        public void AddRange<T>(params T[] entities) where T : class
        {
            Set<T>().AddRange(entities);
        }

        public void AddRange<T>(IEnumerable<T> entities) where T : class
        {
            Set<T>().AddRange(entities);
        }

        public Task AddRangeAsync<T>(params T[] entities) where T : class
        {
            return Set<T>().AddRangeAsync(entities);
        }

        public Task AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            return Set<T>().AddRangeAsync(entities, cancellationToken);
        }

        #endregion

        #region Update

        public new T Update<T>(T entity) where T : class
        {
            entity = Set<T>().Update(entity).Entity;
            return entity;
        }

        public void UpdateRange<T>(params T[] entities) where T : class
        {
            Set<T>().UpdateRange(entities);
        }

        public void UpdateRange<T>(IEnumerable<T> entities) where T : class
        {
            Set<T>().UpdateRange(entities);
        }

        #endregion

        #region Remove

        public new void Remove<T>(T entity) where T : class
        {
            Set<T>().RemoveRange(entity);
        }

        public void RemoveWhere<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var query = Where(predicate);
            RemoveRange(query);
        }

        public void RemoveRange<T>(ICollection<T> entities, bool isPhysicalDelete = false) where T : class, IBaseEntity
        {
            if (!isPhysicalDelete)
            {
                foreach (var entity in entities)
                {
                    entity.IsDeleted = true;
                    entity.DeletedOnUtc = DateTime.UtcNow;
                    Set<T>().Update(entity);
                }
            }
            else
            {
                Set<T>().RemoveRange(entities);
            }
        }

        public void RemoveRange<T>(bool isPhysicalDelete = false, params T[] entities) where T : class, IBaseEntity
        {
            RemoveRange(entities, isPhysicalDelete);
        }

        public void RemoveRange<T>(params T[] entities) where T : class
        {
            Set<T>().RemoveRange(entities);
        }

        public void RemoveRange<T>(IEnumerable<T> entities) where T : class
        {
            Set<T>().RemoveRange(entities);
        }

        #endregion

        #region Delete

        public void Delete<TEntity>(TEntity entity, bool isPhysicalDelete = false) where TEntity : class, IBaseEntity
        {
            if (!isPhysicalDelete)
            {
                entity.IsDeleted = true;
                entity.DeletedOnUtc = DateTime.UtcNow;
                Update(entity);
            }
            Set<TEntity>().Remove(entity);
        }

        public void DeleteRange<TEntity>(IEnumerable<TEntity> entities, bool isPhysicalDelete = false) where TEntity : class, IBaseEntity
        {
            if (!isPhysicalDelete)
            {
                foreach (var entity in entities)
                {
                    Delete(entity);
                }
            }
            else
            {
                Set<TEntity>().RemoveRange(entities);
            }
        }

        public void DeleteRange<TEntity>(bool isPhysicalDelete = false, params TEntity[] entities) where TEntity : class, IBaseEntity
        {
            DeleteRange(entities, isPhysicalDelete);
        }

        public void DeleteWhere<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isPhysicalDelete = false) where TEntity : class, IBaseEntity
        {
            var query = Get(predicate, isPhysicalDelete).AsEnumerable();
            DeleteRange(query, isPhysicalDelete);
        }

        #endregion
    }
}