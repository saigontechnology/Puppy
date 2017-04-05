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
//         TopCoreAuthDbContext
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TopCore.Auth.Domain.Data;
using TopCore.Auth.Domain.Entities;
using TopCore.Framework.DependencyInjection.Attributes;

namespace TopCore.Auth.Data
{
    [PerRequestDependency(ServiceType = typeof(ITopCoreAuthDbContext))]
    public class TopCoreAuthDbContext : IdentityDbContext<UserEntity>, ITopCoreAuthDbContext
    {
        public TopCoreAuthDbContext(DbContextOptions<TopCoreAuthDbContext> options) : base(options)
        {
            Console.WriteLine($"{nameof(TopCoreAuthDbContext)} is Created", nameof(TopCoreAuthDbContext));
        }

        public DbSet<UserEntity> UserEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Console.WriteLine($"{nameof(TopCoreAuthDbContext)} is Created", nameof(OnModelCreating));

            // Convention Table Name is Entity Name without EntityMapping Postfix
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.Relational().TableName = entity.DisplayName().Replace(nameof(EntityMapping), string.Empty);
            }

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is EntityBase && (x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted));
            DateTime utcNow = DateTime.UtcNow;
            TimeSpan timeSpan = utcNow.TimeOfDay;
            foreach (var entity in entities)
            {
                switch (entity.State)
                {
                    case EntityState.Added:
                        ((EntityBase)(entity.Entity)).CreatedOnUtc = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        ((EntityBase)(entity.Entity)).LastUpdatedOnUtc = DateTime.UtcNow;
                        break;
                    case EntityState.Deleted:
                        ((EntityBase)(entity.Entity)).DeletedOnUtc = DateTime.UtcNow;
                        break;
                    default:
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}