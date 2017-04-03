# Important Note
> Project Created by **Top Nguyen** (http://topnguyen.net)

Edit csproj
```markup
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore" Version="1.1.1" />
    <!--
      SqlServer and Design use 1.1.1
      because it have some Library version difference with current AspCore 1.1
      (So it make migrate fail)
    -->
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="1.1.1" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="1.1.1" />
    <!-- Keep install Tools version 1.1.0 but use version 1.0.0 -->
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="1.1.0" />
    <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="1.0.0-*" />
  </ItemGroup>
```