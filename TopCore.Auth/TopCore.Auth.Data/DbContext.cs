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

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TopCore.Auth.Data.EntityMapping;
using TopCore.Auth.Domain.Data;
using TopCore.Auth.Domain.Entities;
using TopCore.Framework.DependencyInjection.Attributes;
using TopCore.Framework.EF;

namespace TopCore.Auth.Data
{
    [PerRequestDependency(ServiceType = typeof(IDbContext))]
    public class DbContext : IdentityDbContext<UserEntity>, IDbContext
    {
        public DbContext(DbContextOptions<DbContext> options) : base(options)
        {
            Console.WriteLine($"{nameof(DbContext)} is Created", nameof(DbContext));
        }

        public DbSet<UserEntity> UserEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Console.WriteLine($"{nameof(DbContext)} is Created", nameof(OnModelCreating));

            modelBuilder.AddConfiguration(new UserEntityMapping());

            // Convention Table Name is Entity Name without EntityMapping Postfix
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.Relational().TableName = entity.DisplayName().Replace("Entity", string.Empty).Replace("Mapping", string.Empty);
                Console.WriteLine($"Table {entity.Relational().TableName} is Created", nameof(DbContext));
            }

            base.OnModelCreating(modelBuilder);
        }

        #region Save Changes

        public override int SaveChanges()
        {
            StandalizeSaveChangeData(ChangeTracker);
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            StandalizeSaveChangeData(ChangeTracker);
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            StandalizeSaveChangeData(ChangeTracker);
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            StandalizeSaveChangeData(ChangeTracker);
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public void StandalizeSaveChangeData(ChangeTracker changeTracker)
        {
            var entities = changeTracker.Entries().Where(x => x.Entity is EntityBase && (x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted));

            DateTime utcNow = DateTime.UtcNow;

            foreach (var entity in entities)
            {
                if (!(entity.Entity is EntityBase) || !(entity.Entity is IdentityUserEntityBase)) continue;

                switch (entity.State)
                {
                    case EntityState.Added:
                        {
                            entity.Property(nameof(EntityBase.CreatedOnUtc)).CurrentValue = utcNow;
                            entity.Property(nameof(EntityBase.LastUpdatedOnUtc)).CurrentValue = null;
                            entity.Property(nameof(EntityBase.IsDeleted)).CurrentValue = false;
                            entity.Property(nameof(EntityBase.DeletedOnUtc)).CurrentValue = null;
                            break;
                        }
                    case EntityState.Modified:
                        {
                            entity.Property(nameof(EntityBase.LastUpdatedOnUtc)).CurrentValue = utcNow;
                            break;
                        }
                    case EntityState.Deleted:
                        {
                            entity.Property(nameof(EntityBase.IsDeleted)).CurrentValue = true;
                            entity.Property(nameof(EntityBase.DeletedOnUtc)).CurrentValue = utcNow;

                            // Force to update not delete data in physical disk
                            entity.State = EntityState.Modified;
                            break;
                        }
                }
            }
        }

        #endregion

        #region Use Base

        public new EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : EntityBase
        {
            return base.Entry(entity);
        }

        public new EntityEntry<TEntity> Add<TEntity>(TEntity entity) where TEntity : EntityBase
        {
            return base.Add(entity);
        }

        public new Task<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = new CancellationToken()) where TEntity : EntityBase
        {
            return base.AddAsync(entity, cancellationToken);
        }

        public new EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : EntityBase
        {
            return base.Attach(entity);
        }

        public new EntityEntry<TEntity> Update<TEntity>(TEntity entity) where TEntity : EntityBase
        {
            return base.Update(entity);
        }

        public new EntityEntry<TEntity> Remove<TEntity>(TEntity entity) where TEntity : EntityBase
        {
            return base.Remove(entity);
        }

        public new DbSet<TEntity> Set<TEntity>() where TEntity : EntityBase
        {
            return base.Set<TEntity>();
        }

        public new object Find(Type entityType, params object[] keyValues)
        {
            return base.Find(entityType, keyValues);
        }

        public new Task<object> FindAsync(Type entityType, params object[] keyValues)
        {
            return base.FindAsync(entityType, keyValues);
        }

        public new Task<object> FindAsync(Type entityType, object[] keyValues, CancellationToken cancellationToken)
        {
            return base.FindAsync(entityType, keyValues, cancellationToken);
        }

        public new TEntity Find<TEntity>(params object[] keyValues) where TEntity : EntityBase
        {
            return base.Find<TEntity>(keyValues);
        }

        public new Task<TEntity> FindAsync<TEntity>(params object[] keyValues) where TEntity : EntityBase
        {
            return base.FindAsync<TEntity>(keyValues);
        }

        public new Task<TEntity> FindAsync<TEntity>(object[] keyValues, CancellationToken cancellationToken) where TEntity : EntityBase
        {
            return base.FindAsync<TEntity>(keyValues);
        }

        #endregion
    }
}