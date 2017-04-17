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
