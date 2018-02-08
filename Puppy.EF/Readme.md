![Logo](favicon.ico)
# Puppy.EF
> Project Created by [**Top Nguyen**](http://topnguyen.net)

- Don't query or save change async because EF have issue [5816](https://github.com/aspnet/EntityFrameworkCore/issues/5816)

- Use Query Filter to exclude soft "Deleted" row, when use Repository with `isIncludeDeleted` = `true` I add `IgnoreQueryFilters()` => will ignore all query filters not only soft Deleted filter.
  >Due to issue of EF Core, please check [8576](https://github.com/aspnet/EntityFrameworkCore/issues/8576)

- AspNetCore 2 already support for `TransactionScope` but EF Core not yet. Please view more detail at [Stack OverFlow](https://stackoverflow.com/questions/46577551/ef-core-2-0-transactionscope-error)

- Please view more detail at [Annoucing for AspNetCore 2](https://blogs.msdn.microsoft.com/dotnet/2017/05/12/announcing-ef-core-2-0-preview-1/)

## Initial Database
Setup by Command Windows of current project 

```markup
dotnet ef migrations add Initial
dotnet ef database update  -v
```

**Don't use/run Package Manager Console to do the above action**
**It will hang the Console and never stop without any result.**

# Important Thing about csproj

```markup
  <PropertyGroup>
    <TargetFramework>netcoreapp1.1</TargetFramework>
    <ApplicationIcon>favicon.ico</ApplicationIcon>
    <Copyright>http://topnguyen.net</Copyright>
    
    <!-- Enable runtime config and runtime version, Need for entity framework DonetClioTool -->
    <GenerateRuntimeConfigurationFiles>true</GenerateRuntimeConfigurationFiles>
  </PropertyGroup>

  <!-- Entity Framework -->
  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="1.1.1" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="1.1.1" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="1.1.1" />
    <!-- START Keep Runtime version is 1.0.0-* -->
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="1.1.0" />
    <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="1.0.0-*" />
    <!-- END -->
  </ItemGroup>
```

# DbContext

```csharp
[PerRequestDependency(ServiceType = typeof(IDbContext))]
public partial class DbContext : BaseDbContext, IDbContext
{
    public DbContext()
    {
    }

    public DbContext(DbContextOptions<DbContext> options) : base(options)
    {
    }

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

        // [Important] Keep Under Base For Override And Make End Result

        // Scan and apply Config/Mapping for Tables/Entities (from folder "Map")
        builder.AddConfigFromAssembly(DbContextFactory.GetMigrationAssembly());

        // Set Delete Behavior as Restrict in Relationship
        builder.DisableCascadingDelete();

        // Convention for Table name
        builder.RemovePluralizingTableNameConvention();

        builder.ReplaceTableNameConvention("Entity", string.Empty);
    }
}
```

# DbFactory
```csharp
public class DbContextFactory : IDbContextFactory<DbContext>
{
    public DbContext Create(DbContextFactoryOptions options)
    {
        var connectionString = GetConnectionString(options);
        return CreateCoreContext(connectionString);
    }

    /// <summary>
    /// Get connection from DbContextFactoryOptions Environment
    /// </summary>
    /// <param name="options"></param>
    /// <returns></returns>
    private string GetConnectionString(DbContextFactoryOptions options)
    {
        var connectionString = ConfigHelper.GetValue("appsettings.json", $"ConnectionStrings:{options.EnvironmentName}");
        return connectionString;
    }

    private static DbContext CreateCoreContext(string connectionString)
    {
        var builder = new DbContextOptionsBuilder<DbContext>();
        builder.UseSqlServer(connectionString, optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(IDataModule).GetTypeInfo().Assembly.GetName().Name));
        return new DbContext(builder.Options);
    }
}
```

# IDbModule
```csharp
public interface IDataModule
{
}
```

# Mapping
- Sample Entity Map
```csharp
public class UserMap : EntityTypeConfiguration<UserEntity>
{
    public override void Map(EntityTypeBuilder<UserEntity> builder)
    {
        base.Map(builder);
        builder.ToTable(nameof(UserEntity));
    }
}
```