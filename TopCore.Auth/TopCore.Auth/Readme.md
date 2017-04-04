# Important Note
> Project Created by **Top Nguyen** (http://topnguyen.net)

## Initial Database
- Setup by Command Windows of current project 
```markup
dotnet ef migrations add InitialIdentityTopCore -c TopCoreIdentityDbContext -o Identity/Migrations/TopCoreIdentityDb
dotnet ef migrations add InitialIdentityServerPersistedGrant -c PersistedGrantDbContext -o Identity/Migrations/PersistedGrantDb
dotnet ef migrations add InitialIdentityServerConfiguration -c ConfigurationDbContext -o Identity/Migrations/ConfigurationDb
```

**Don't use/run Package Manager Console to do the above action**

Like
```markup
add-migration InitialIdentityServerPersistedGrant -c PersistedGrantDbContext -o Identity/Migrations/PersistedGrantDb
```
or Try to use
```markup
update-database -v -c PersistedGrantDbContext
```
**It will hang the Console and never stop without any result.**