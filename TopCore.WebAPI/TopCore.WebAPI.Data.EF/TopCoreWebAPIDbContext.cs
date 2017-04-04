using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using TopCore.WebAPI.Data.Entity;

namespace TopCore.WebAPI.Data.EF
{
    public class TopCoreWebAPIDbContext : DbContext
    {
        public TopCoreWebAPIDbContext(DbContextOptions<TopCoreWebAPIDbContext> options) : base(options)
        {
            Debug.WriteLine($"{nameof(TopCoreWebAPIDbContext)} is Created", nameof(TopCoreWebAPIDbContext));
        }

        public DbSet<UserEntity> UserEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Debug.WriteLine($"{nameof(TopCoreWebAPIDbContext)} is Created", nameof(OnModelCreating));

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