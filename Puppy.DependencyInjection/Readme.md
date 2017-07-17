# Important Note
> Project Created by [**Top Nguyen**](http://topnguyen.net)

- Set main project (Web.API) build output with all configuration is ".\bin\" (.\bin\\\<Main Project>.xml for XML to use swagger)

- Set all project build output to ".\bin\netcoreapp1.1\" of main project to do the scan.

- Right Click on Project > Build Tab > All Configuration > Put Build output path "..\\\<Main Project>\bin\"

- 1 Service just have only one Implement, if have more than 1 => throw ConflictRegisterException

- 1 Implement have only register for a Service, if have more than 1 => Just get last one :D (ok, let support for inheric case)

## Startup Config
```c#
services
	.AddDependencyInjectionScanner()
	.ScanFromSelf()
	.ScanFromAllAssemblies();
     
// or 
services
	.AddDependencyInjectionScanner()
	.ScanFromAllAssemblies($"{nameof(TopCore)}.*.dll", Path.GetFullPath(PlatformServices.Default.Application.ApplicationBasePath));
```

## Class Use

```c#
- Use PerRequestDependencyAttribute([PerRequestDependency]) for Request Scope
  - ServiceLifetime.Scoped: Shared within a single request (or Service Scope).

- Use PerResolveDependencyAttribute ([PerResolveDependency]) for Resolve Scope
  - ServiceLifetime.Transient: Created on every request for the service.

- Use SingletonDependencyAttribute ([SingletonDependency]) for Application Scope
  - ServiceLifetime.Singleton: A single shared instance throughout your application’s lifetime. Only created once.
```

## Get Instance

```c#
IDataMigrationService dataMigrationService = services.Resolve<IDataMigrationService>();
```

## Sample
    [PerResolveDependency(ServiceType = typeof(IEmailSender))]
    [PerRequestDependency(ServiceType = typeof(ISmsSender))]
    public class LogService : ITextLogService, IDbLogService
    {
        public Task LogTextAsync(string filePath, string message)
        {
            // TODO insert to file
            return Task.FromResult(0);
        }
        public Task LogDbAsync(string message, string level)
        {
            // TODO insert to database
            return Task.FromResult(0);
        }
    }

    // Self Register to inject and control life time cycle
    [PerRequestDependency]
    public class LogService : ITextLogService
    {
        public Task LogTextAsync(string filePath, string message)
        {
            // TODO insert to file
            return Task.FromResult(0);
        }
    }

# Understanding Scope Lifetime
The Transient and Singleton scopes are very obvious, but the Scope lifetime is more confusing. Many people (yours included) assumed that this meant once-per-request or transient if outside a request. But that’s not what it means. In fact, the DI layer specific has an interface called IServiceScopeFactory to allow you to create a scope for DI injection. Calling CreateScope returns an IServiceScope which implements IDisposable. Therefore you’d commonly use a ‘using’ statement to protect it’s destruction (and ending the scope):
```c#
    using (var scope = scopeFactory.CreateScope())
    {
        // ...
    }
```

Then I'm creating a scope around the call to seed the database and use the scope to create the instance of my seeder (so that all the DI inside is inside a scope):
```c#
    using (var scope = scopeFactory.CreateScope())
    {
        var initializer = scope.ServiceProvider.GetService<WilderInitializer>();
        initializer.SeedAsync().Wait();
    }
```

This way the initializer doesn’t need to do the actual execution of the scope and can be DI ignorant (in case of testing or moving the seeding to inside a request later).

Here is a link to Julie’s post of the twitter conversation if you want to see how it transpired!

> http://thedatafarm.com/dotnet/twitter-education-re-aspnet-core-scope/ 