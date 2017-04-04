using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;

namespace TopCore.Auth.Data.EF
{
    public class TopCoreAuthDbContext : IdentityDbContext<Entity.UserEntity>
    {
        public TopCoreAuthDbContext(DbContextOptions<TopCoreAuthDbContext> options) : base(options)
        {
            Debug.WriteLine($"{nameof(TopCoreAuthDbContext)} is Created", nameof(TopCoreAuthDbContext));
        }

        public DbSet<Entity.UserEntity> UserEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Debug.WriteLine($"{nameof(TopCoreAuthDbContext)} is Created", nameof(OnModelCreating));

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