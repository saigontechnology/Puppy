using TopCore.WebAPI.Data.EF.Factory;
using TopCore.WebAPI.Data.EF.Mapping;
using TopCore.WebAPI.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TopCore.Framework.Core;
using TopCore.Framework.DependencyInjection.Attributes;
using TopCore.Framework.EF;

namespace TopCore.WebAPI.Data.EF
{
    [PerRequestDependency(ServiceType = typeof(IDbContext))]
    public class DbContext : BaseDbContext, IDbContext
    {
        public DbContext()
        {
        }

        public DbContext(DbContextOptions<DbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                string connectionString = ConfigHelper.GetValue("appsettings.json", $"ConnectionStrings:{environmentName}");
                optionsBuilder.UseSqlServer(connectionString, o => o.MigrationsAssembly(typeof(IDataModule).GetTypeInfo().Assembly.GetName().Name));
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Keep under base for override and make end result
            builder.AddConfigFromAssembly(typeof(IDataModule).GetTypeInfo().Assembly);
        }

        #region Save Changes

        public Task<int> SaveChangesAsync()
        {
            return SaveChangesAsync(new CancellationToken());
        }

        #endregion Save Changes
    }
}