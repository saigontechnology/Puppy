using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using TopCore.SSO.Data.Entity;

namespace TopCore.SSO.Data.EF
{
    public class TopCoreDbContext : DbContext
    {
        public TopCoreDbContext(DbContextOptions<TopCoreDbContext> options) : base(options)
        {
            Debug.WriteLine($"{nameof(TopCoreDbContext)} is Created", nameof(TopCoreDbContext));
        }

        public DbSet<UserEntity> UserEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Debug.WriteLine($"{nameof(TopCoreDbContext)} is Created", nameof(OnModelCreating));

            // Convention Table Name is Entity Name without EntityMapping Postfix
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.Relational().TableName = entity.DisplayName().Replace("EntityMapping", string.Empty);
            }
            base.OnModelCreating(modelBuilder);

            Database.Migrate();
        }
    }
}