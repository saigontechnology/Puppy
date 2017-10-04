#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> SqliteDbContext.cs </Name>
//         <Created> 23/08/17 10:08:35 AM </Created>
//         <Key> 38822fda-425a-4d6d-9c3e-38f916cb73a8 </Key>
//     </File>
//     <Summary>
//         SqliteDbContext.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Puppy.EF;
using Puppy.EF.Maps;
using Puppy.Logger.Core.Models;
using Puppy.Logger.SQLite.Factory;

namespace Puppy.Logger.SQLite
{
    public sealed class SqliteDbContext : BaseDbContext
    {
        /// <summary>
        ///     Set CMD timeout is 20 minutes 
        /// </summary>
        public readonly int CmdTimeoutInSecond = 12000;

        public DbSet<LogEntity> Logs { get; set; }

        public SqliteDbContext()
        {
            Database.SetCommandTimeout(CmdTimeoutInSecond);
        }

        public SqliteDbContext(DbContextOptions options) : base(options)
        {
            Database.SetCommandTimeout(CmdTimeoutInSecond);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(SqliteDbContextFactory.GetConnectionString(), o => o.MigrationsAssembly(SqliteDbContextFactory.GetMigrationAssemblyName()));
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // [Important] Keep Under Base For Override And Make End Result

            // Scan and apply Config/Mapping for Tables/Entities (from folder "Map")
            builder.AddConfigFromAssembly(SqliteDbContextFactory.GetMigrationAssembly());

            // Set Delete Behavior as Restrict in Relationship
            builder.DisableCascadingDelete();

            // Convention for Table name
            builder.RemovePluralizingTableNameConvention();

            builder.ReplaceTableNameConvention("Entity", string.Empty);
        }
    }
}