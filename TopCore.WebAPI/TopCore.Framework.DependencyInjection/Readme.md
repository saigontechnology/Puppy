# Important Note
- Set main project (Web.API) build output with all configuration is ".\bin\" (.\bin\TopCore.WebAPI.xml for XML to use swagger)
- Set all project build output to ".\bin\netcoreapp1.1\" of main project to do the scan.
- Right Click on Project > Build Tab > All Configuration > Put Build output path "..\TopCore.WebAPI\bin\"
## Startup Config
     services
        .AddDependencyInjectionScanner()
        .ScanFromSelf()
        .ScanFromAllAssemblies();
     
     // or 
     services
        .AddDependencyInjectionScanner()
        .ScanFromAllAssemblies($"{nameof(TopCore)}.*.dll", Path.GetFullPath(PlatformServices.Default.Application.ApplicationBasePath));

## Class Use
- Use PerRequestDependencyAttribute for Request Scope
  - ServiceLifetime.Scoped: Shared within a single request (or Service Scope).

- Use PerResolveDependencyAttribute for Resolve Scope
  - ServiceLifetime.Transient: Created on every request for the service.

- Use SingletonDependencyAttribute for Application Scope
  - ServiceLifetime.Singleton: A single shared instance throughout your application’s lifetime. Only created once.

## Sample

    [TransientDependency(ServiceType = typeof(IEmailSender))]
    [TransientDependency(ServiceType = typeof(ISmsSender))]
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            // TODO to send an email.
            return Task.FromResult(0);
        }
        public Task SendSmsAsync(string number, string message)
        {
            // TODO send a text message.
            return Task.FromResult(0);
        }
    }

# Understanding Scope Lifetime
The Transient and Singleton scopes are very obvious, but the Scope lifetime is more confusing. Many people (yours included) assumed that this meant once-per-request or transient if outside a request. But that’s not what it means. In fact, the DI layer specific has an interface called IServiceScopeFactory to allow you to create a scope for DI injection. Calling CreateScope returns an IServiceScope which implements IDisposable. Therefore you’d commonly use a ‘using’ statement to protect it’s destruction (and ending the scope):

    using (var scope = scopeFactory.CreateScope())
    {
        // ...
    }

Then I’m creating a scope around the call to seed the database and use the scope to create the instance of my seeder (so that all the DI inside is inside a scope):
    
    using (var scope = scopeFactory.CreateScope())
    {
        var initializer = scope.ServiceProvider.GetService<WilderInitializer>();
        initializer.SeedAsync().Wait();
    }

This way the initializer doesn’t need to do the actual execution of the scope and can be DI ignorant (in case of testing or moving the seeding to inside a request later).

Here is a link to Julie’s post of the twitter conversation if you want to see how it transpired!

> http://thedatafarm.com/dotnet/twitter-education-re-aspnet-core-scope/ 