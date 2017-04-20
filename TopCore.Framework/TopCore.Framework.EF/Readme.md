# Important Note
> Project Created by **Top Nguyen** (http://topnguyen.net)

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
# Templates

## Export
1. Export Template
- VS < 2017
> - Click on "File" > Export Template

- VS >= 2017
> - Click on "Project" > Export Template

2. Select Item Template
3. Select Solution have template
4. Give Name, Icon
5. Copy Export Zip file and use it like Import steps below.

## Import
- Copy .Zip file to visual studio item Templates Without Unzip, Just keep .Zip file!
> - %userprofile%\documents\Visual Studio 2017\Templates\ItemTemplates
> - %userprofile%\documents\Visual Studio 2015\Templates\ItemTemplates

- Then Restart Visual Studio, try to use Add New Item.
- Done

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

        foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }

        // Keep under base for override and make end result
        builder.AddConfigFromAssembly(typeof(IDataModule).GetTypeInfo().Assembly);
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