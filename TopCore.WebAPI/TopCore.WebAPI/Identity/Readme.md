# Important Note
> Project Created by **Top Nguyen** (http://topnguyen.net)

## Identity Setup and Note
1. Nuget Package Install by csproj
Edit .csproj file and put this code below to enable Entity Framework with design and tool
```c#
  <ItemGroup>
    <PackageReference Include="IdentityServer4" Version="1.4.2" />
    <PackageReference Include="IdentityServer4.AspNetIdentity" Version="1.0.1" />
    <PackageReference Include="IdentityServer4.EntityFramework" Version="1.0.1" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.OpenIdConnect" Version="1.1.0" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="1.1.1" />

    <!-- SqlServer and Design use 1.0.2 Instead of 1.1.0, because it have some Library version difference with current AspCore 1.1 (So it make migrate fail) -->
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="1.0.2" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="1.0.2" />
    <!-- Keep install Tools version 1.1.0 but use version 1.0.0 -->
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="1.1.0" />
    <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="1.0.0" />
  </ItemGroup>
```
List Nuget Package
  - IdentityServer4
  - IdentityServer4.AspNetIdentity
  - IdentityServer4.EntityFramework
  - Microsoft.AspNetCore.Identity.EntityFrameworkCore
  - Microsoft.AspNetCore.Authentication.OpenIdConnect
  - Microsoft.EntityFrameworkCore.SqlServer
  - Microsoft.EntityFrameworkCore.Design
  - Microsoft.EntityFrameworkCore.Tools

3. Setup Startup.cs of main project
```c#
Setup.AddIdentityServer(services, Configuration.GetConnectionString("Identity"));
services.AddCors(options =>
    {
        options.AddPolicy(nameof(TopCore), policy =>
        {
            policy.WithOrigins().AllowAnyHeader().AllowAnyMethod();
        });
    });
```
3. Initial Database
- Setup by Command Windows of current project 
```c#
dotnet ef migrations add InitialIdentityTopCore -c TopCoreIdentityDbContext -o Identity/Migrations/TopCoreIdentityDb
dotnet ef migrations add InitialIdentityServerPersistedGrant -c PersistedGrantDbContext -o Identity/Migrations/PersistedGrantDb
dotnet ef migrations add InitialIdentityServerConfiguration -c ConfigurationDbContext -o Identity/Migrations/ConfigurationDb
```

**Don't use/run Package Manager Console to do the above action**

Like
```c#
add-migration InitialIdentityServerPersistedGrant -c PersistedGrantDbContext -o Identity/Migrations/PersistedGrantDb
```
or Try to use
```c#
update-database -v -c PersistedGrantDbContext
```
**It will hang the Console and never stop without any result.**