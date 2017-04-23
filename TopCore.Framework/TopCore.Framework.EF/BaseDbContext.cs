#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Author> Top </Author>
//     <Project> TopCore.Framework.EF </Project>
//     <File>
//         <Name> BaseDbContext </Name>
//         <Created> 06 Apr 17 1:09:03 AM </Created>
//         <Key> 698007b1-38ad-45b0-b7d0-f22069fe2fab </Key>
//     </File>
//     <Summary>
//         BaseDbContext
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TopCore.Framework.EF.Interfaces;

namespace TopCore.Framework.EF
{
    public class BaseDbContext : DbContext, IBaseDbContext
    {
        protected BaseDbContext()
        {
        }

        public BaseDbContext(DbContextOptions options) : base(options)
        {
        }

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

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            StandardizeSaveChangeData(ChangeTracker);
            return base.SaveChangesAsync(cancellationToken);
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
    }
}