# Important Note
> Project Created by **Top Nguyen** (http://topnguyen.net)

## Initial Database
Setup by Command Windows of current project 

```markup
dotnet ef migrations add InitialTopCoreAuth -c DbContext -o Migrations/TopCoreAuthDb
dotnet ef database update -c DbContext -v

dotnet ef migrations add InitialPersistedGrant -c PersistedGrantDbContext -o Migrations/PersistedGrantDb
dotnet ef database update -c PersistedGrantDbContext -v

dotnet ef migrations add InitialConfiguration -c ConfigurationDbContext -o Migrations/ConfigurationDb
dotnet ef database update -c ConfigurationDbContext -v
```


**Don't use/run Package Manager Console to do the above action**

Like
```markup
add-migration InitialPersistedGrant -c PersistedGrantDbContext -o Migrations/PersistedGrantDb
```

or Try to use
```markup
update-database -v -c PersistedGrantDbContext
```
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

  <!-- Use Identity Server -->
  <ItemGroup>
    <PackageReference Include="IdentityServer4" Version="1.4.2" />
    <PackageReference Include="IdentityServer4.AspNetIdentity" Version="1.0.1" />
    <PackageReference Include="IdentityServer4.EntityFramework" Version="1.0.1" />
  </ItemGroup>

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