![Logo](favicon.ico)
# Puppy.Core
> Project Created by [**Top Nguyen**](http://topnguyen.net)
- Another Framework/Project use this framework.
- This framework is standalone, it mean not use any other Puppy Frameworks.
- Convention: use postfix `Extensions` for extension class, `Helper` for helper class.
	> Remember: Extensions with 's'

# External Libraries Used
```markup
  <!-- External Libraries -->
  <ItemGroup>
    <PackageReference Include="System.ValueTuple" Version="4.3.1" />
    <PackageReference Include="System.Xml.XmlSerializer" Version="4.3.0" />

    <PackageReference Include="Microsoft.Extensions.Configuration" Version="1.1.2" />
    <PackageReference Include="Microsoft.Extensions.Configuration.Binder" Version="1.1.2" />
    <PackageReference Include="Microsoft.Extensions.Caching.Abstractions" Version="1.1.2" />

    <PackageReference Include="Newtonsoft.Json" Version="10.0.3" />

    <PackageReference Include="CoreCompat.System.Drawing" Version="1.0.0-beta006" />
  </ItemGroup>
```