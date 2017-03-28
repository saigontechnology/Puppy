using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using TopCore.DAL.Entity;

namespace TopCore.DAL.EF
{
    public class TopCoreContext : DbContext
    {
        public TopCoreContext(DbContextOptions<TopCoreContext> options) : base(options)
        {
            Debug.WriteLine($"{nameof(TopCoreContext)} is Created", nameof(TopCoreContext));
        }

        public DbSet<UserEntity> UserEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Debug.WriteLine($"{nameof(TopCoreContext)} is Created", nameof(OnModelCreating));

            // Convention Table Name is Entity Name without Entity Postfix
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.Relational().TableName = entity.DisplayName().Replace("Entity", string.Empty);
            }
            base.OnModelCreating(modelBuilder);

            Database.Migrate();
        }
    }
}