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

using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TopCore.Auth.Data.Factory;
using TopCore.Auth.Domain.Entities;
using TopCore.Auth.Domain.Interfaces.Data;
using TopCore.Framework.DependencyInjection.Attributes;
using TopCore.Framework.EF.Mapping;

namespace TopCore.Auth.Data
{
    [PerRequestDependency(ServiceType = typeof(IDbContext))]
    public sealed class DbContext : IdentityDbContext<User>, IDbContext, IConfigurationDbContext, IPersistedGrantDbContext
    {
        private readonly int cmdTimeoutInSecond = 12000; // 20 mins

        private ConfigurationStoreOptions _configurationStoreOptions;

        private OperationalStoreOptions _operationalStoreOptions;

        public DbContext()
        {
            Database.SetCommandTimeout(cmdTimeoutInSecond);
        }

        public DbContext(DbContextOptions<DbContext> options, ConfigurationStoreOptions configurationStoreOptions, OperationalStoreOptions operationalStoreOptions) : base(options)
        {
            Database.SetCommandTimeout(cmdTimeoutInSecond);

            _configurationStoreOptions = configurationStoreOptions ?? throw new ArgumentNullException(nameof(configurationStoreOptions));
            _operationalStoreOptions = operationalStoreOptions ?? throw new ArgumentNullException(nameof(operationalStoreOptions));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true).Build();
                var connectionString = config.GetSection($"ConnectionStrings:{environmentName}").Value;
                optionsBuilder.UseSqlServer(connectionString, o => o.MigrationsAssembly(typeof(IDataModule).GetTypeInfo().Assembly.GetName().Name));

                _configurationStoreOptions = new ConfigurationStoreOptions
                {
                    DefaultSchema = "dbo"
                };

                _operationalStoreOptions = new OperationalStoreOptions
                {
                    DefaultSchema = "dbo"
                };
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ConfigureClientContext(_configurationStoreOptions);
            builder.ConfigureResourcesContext(_configurationStoreOptions);
            builder.ConfigurePersistedGrantContext(_operationalStoreOptions);
            base.OnModelCreating(builder);

            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                relationship.DeleteBehavior = DeleteBehavior.Restrict;

            // Keep under base for override and make end result
            builder.AddConfigFromAssembly(typeof(IDataModule).GetTypeInfo().Assembly);
        }

        #region DbSet

        public DbSet<IdentityServer4.EntityFramework.Entities.Client> Clients { get; set; }

        public DbSet<IdentityServer4.EntityFramework.Entities.IdentityResource> IdentityResources { get; set; }

        public DbSet<IdentityServer4.EntityFramework.Entities.ApiResource> ApiResources { get; set; }

        public DbSet<IdentityServer4.EntityFramework.Entities.PersistedGrant> PersistedGrants { get; set; }

        #endregion

        public Task<int> SaveChangesAsync()
        {
            return SaveChangesAsync(new CancellationToken());
        }
    }
}