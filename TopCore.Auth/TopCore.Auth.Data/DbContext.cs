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

using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using TopCore.Auth.Data.Factory;
using TopCore.Auth.Domain.Data;
using TopCore.Auth.Domain.Entities;
using TopCore.Framework.Core;
using TopCore.Framework.DependencyInjection.Attributes;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace TopCore.Auth.Data
{
    [PerRequestDependency(ServiceType = typeof(IDbContext))]
    public class DbContext : IdentityDbContext<User>, IDbContext, IConfigurationDbContext, IPersistedGrantDbContext
    {
        private ConfigurationStoreOptions _configurationStoreOptions;

        private OperationalStoreOptions _operationalStoreOptions;

        public DbContext()
        {
        }

        public DbContext(DbContextOptions<DbContext> options, ConfigurationStoreOptions configurationStoreOptions, OperationalStoreOptions operationalStoreOptions) : base(options)
        {
            _configurationStoreOptions = configurationStoreOptions ?? throw new ArgumentNullException(nameof(configurationStoreOptions));
            _operationalStoreOptions = operationalStoreOptions ?? throw new ArgumentNullException(nameof(operationalStoreOptions));
        }

        public DbSet<Client> Clients { get; set; }

        public DbSet<IdentityResource> IdentityResources { get; set; }

        public DbSet<ApiResource> ApiResources { get; set; }

        public DbSet<PersistedGrant> PersistedGrants { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                string connectionString = ConfigHelper.GetValue("appsettings.json", $"ConnectionStrings:{environmentName}");
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureClientContext(_configurationStoreOptions);
            modelBuilder.ConfigureResourcesContext(_configurationStoreOptions);
            modelBuilder.ConfigurePersistedGrantContext(_operationalStoreOptions);
            base.OnModelCreating(modelBuilder);
        }

        #region Save Changes

        public Task<int> SaveChangesAsync()
        {
            return SaveChangesAsync(new CancellationToken());
        }

        #endregion
    }
}