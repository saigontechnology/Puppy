using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TopCore.DAL.Entity;

namespace TopCore.DAL
{
    public class TopCoreContext : DbContext
    {
        public TopCoreContext(DbContextOptions<TopCoreContext> options) : base(options)
        {
        }

        public DbSet<UserEntity> UserEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Convention Table Name is Entity Name without Entity Postfix
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
                entity.Relational().TableName = entity.DisplayName().Replace("Entity", string.Empty);
            base.OnModelCreating(modelBuilder);
        }
    }
}