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

        public DbSet<Client> Clients { get; set; }

        public DbSet<IdentityResource> IdentityResources { get; set; }

        public DbSet<ApiResource> ApiResources { get; set; }

        public DbSet<PersistedGrant> PersistedGrants { get; set; }

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
            var entities = changeTracker.Entries().Where(x => x.Entity is IEntityBase && (x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted));

            DateTime utcNow = DateTime.UtcNow;

            foreach (var entity in entities)
            {
                switch (entity.State)
                {
                    case EntityState.Added:
                        {
                            entity.Property(nameof(IEntityBase.CreatedOnUtc)).CurrentValue = utcNow;
                            entity.Property(nameof(IEntityBase.LastUpdatedOnUtc)).CurrentValue = null;
                            entity.Property(nameof(IEntityBase.IsDeleted)).CurrentValue = false;
                            entity.Property(nameof(IEntityBase.DeletedOnUtc)).CurrentValue = null;
                            break;
                        }
                    case EntityState.Modified:
                        {
                            entity.Property(nameof(IEntityBase.LastUpdatedOnUtc)).CurrentValue = utcNow;
                            break;
                        }
                }
            }
        }

        #endregion

        #region Count and Any

        public bool Any<TEntity>() where TEntity : class
        {
            return Set<TEntity>().Any();
        }

        public bool Any<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return Set<TEntity>().Any(predicate);
        }

        public int Count<TEntity>() where TEntity : class
        {
            return Set<TEntity>().Count();
        }

        public int Count<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return Set<TEntity>().Count(predicate);
        }

        #endregion

        #region Get

        public IQueryable<TEntity> AllIncluding<TEntity>(params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class
        {
            IQueryable<TEntity> query = Set<TEntity>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public IQueryable<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false) where TEntity : class, IEntityBase
        {
            IQueryable<TEntity> query = Set<TEntity>().Where(predicate);
            return isIncludeDeleted ? query : query.Where(x => !x.IsDeleted);
        }

        public IQueryable<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class, IEntityBase
        {
            IQueryable<TEntity> query = Get(predicate, isIncludeDeleted);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public TEntity GetSingle<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false) where TEntity : class, IEntityBase
        {
            return Get(predicate, isIncludeDeleted).FirstOrDefault();
        }

        public TEntity GetSingle<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isIncludeDeleted = false, params Expression<Func<TEntity, object>>[] includeProperties) where TEntity : class, IEntityBase
        {
            return Get(predicate, isIncludeDeleted, includeProperties).FirstOrDefault();
        }

        #endregion

        #region Add

        public new TEntity Add<TEntity>(TEntity entity) where TEntity : class
        {
            entity = Set<TEntity>().Add(entity).Entity;
            return entity;
        }

        public new Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            return Set<TEntity>().AddAsync(entity, cancellationToken);
        }

        public void AddRange<TEntity>(params TEntity[] entities) where TEntity : class
        {
            Set<TEntity>().AddRange(entities);
        }

        public void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            Set<TEntity>().AddRange(entities);
        }

        public Task AddRangeAsync<TEntity>(params TEntity[] entities) where TEntity : class
        {
            return Set<TEntity>().AddRangeAsync(entities);
        }

        public Task AddRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            return Set<TEntity>().AddRangeAsync(entities, cancellationToken);
        }

        #endregion

        #region Update

        public new TEntity Update<TEntity>(TEntity entity) where TEntity : class
        {
            entity = Set<TEntity>().Update(entity).Entity;
            return entity;
        }

        public void UpdateRange<TEntity>(params TEntity[] entities) where TEntity : class
        {
            Set<TEntity>().UpdateRange(entities);
        }

        public void UpdateRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            Set<TEntity>().UpdateRange(entities);
        }

        #endregion

        #region Remove

        public void RemoveWhere<TEntity>(Expression<Func<TEntity, bool>> predicate, bool isPhysicalDelete = false) where TEntity : class, IEntityBase
        {
            var query = Get(predicate);
            RemoveRange(query, isPhysicalDelete);
        }

        public TEntity Remove<TEntity>(TEntity entity, bool isPhysicalDelete = false) where TEntity : class, IEntityBase
        {
            if (!isPhysicalDelete)
            {
                entity.IsDeleted = true;
                entity.DeletedOnUtc = DateTime.UtcNow;
                return Update(entity);
            }
            entity = Set<TEntity>().Remove(entity).Entity;
            return entity;
        }

        public void RemoveRange<TEntity>(ICollection<TEntity> entities, bool isPhysicalDelete = false) where TEntity : class, IEntityBase
        {
            if (!isPhysicalDelete)
            {
                foreach (var entity in entities)
                {
                    entity.IsDeleted = true;
                    entity.DeletedOnUtc = DateTime.UtcNow;
                    Set<TEntity>().Update(entity);
                }
            }
            else
            {
                Set<TEntity>().RemoveRange(entities);
            }
        }

        public void RemoveRange<TEntity>(bool isPhysicalDelete = false, params TEntity[] entities) where TEntity : class, IEntityBase
        {
            RemoveRange(entities, isPhysicalDelete);
        }

        public void RemoveRange<TEntity>(params TEntity[] entities) where TEntity : class
        {
            Set<TEntity>().RemoveRange(entities);
        }

        public void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            Set<TEntity>().RemoveRange(entities);
        }

        #endregion
    }
}