using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using TopCore.WebAPI.Data.Entity;

namespace TopCore.WebAPI.Data.EF
{
    public class DbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbContext(DbContextOptions<DbContext> options) : base(options)
        {
            Debug.WriteLine($"{nameof(DbContext)} is Created", nameof(DbContext));
        }

        public DbSet<UserEntity> UserEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Debug.WriteLine($"{nameof(DbContext)} is Created", nameof(OnModelCreating));

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