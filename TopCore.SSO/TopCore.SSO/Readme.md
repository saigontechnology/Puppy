# Important Note
> Project Created by **Top Nguyen** (http://topnguyen.net)

## Identity Setup and Note
1. Nuget Package Install by csproj
Edit .csproj file and put this code below to enable Entity Framework with design and tool
```markup
    <PackageReference Include="IdentityServer4" Version="1.4.2" />
    <PackageReference Include="IdentityServer4.AspNetIdentity" Version="1.0.1" />
    <PackageReference Include="IdentityServer4.EntityFramework" Version="1.0.1" />
    <PackageReference Include="Microsoft.AspNetCore.Cors" Version="1.1.1" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.OpenIdConnect" Version="1.1.1" />
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="1.1.1" />
    <!--
      SqlServer and Design use 1.1.0
      because it have some Library version difference with current AspCore 1.1
      (So it make migrate fail)
    -->
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="1.1.1" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="1.1.1" />
    <!-- Keep install Tools version 1.1.0 but use version 1.0.0 -->
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="1.1.0" />
    <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="1.0.0-*" />
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
        options.AddPolicy(nameof(TopCore.SSO), policy =>
        {
            policy.WithOrigins().AllowAnyHeader().AllowAnyMethod();
        });
    });
```
3. Initial Database
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

## Startup.cs

```c#
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using System.Reflection;
using TopCore.Framework.DependencyInjection;

namespace TopCore.SSO
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public IHostingEnvironment Environment { get; }

        //------------------------------------------------------------
        // Global
        //------------------------------------------------------------

        public Startup(IHostingEnvironment env)
        {
            Environment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Setup).GetTypeInfo().Assembly.GetName().Name;
            Setup.AddIdentityServer(services, Configuration.GetConnectionString("Identity"), migrationsAssembly);

            services.AddCors(options =>
            {
                options.AddPolicy(nameof(TopCore.SSO), policy =>
                {
                    policy.WithOrigins().AllowAnyHeader().AllowAnyMethod();
                });
            });

            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                // Indented for Development only
                options.SerializerSettings.Formatting = Environment.IsDevelopment() ? Formatting.Indented : Formatting.None;

                // Serialize Json as Camel case
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            });

            services.AddDependencyInjectionScanner().ScanFromAllAssemblies($"{nameof(TopCore.SSO)}.*.dll", Path.GetFullPath(PlatformServices.Default.Application.ApplicationBasePath));

            services.AddLogging();

            AddSwagger(services);

            // Write out all dependency injection services
            services.WriteOut($"{nameof(TopCore.SSO)}");
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors(nameof(TopCore.SSO));
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            UseLogFactory(loggerFactory);

            UseExceptionPage(app, env);

            UseSwagger(app);

            Setup.UseIdentityServer(app);
        }

        //------------------------------------------------------------
        // Swagger
        //------------------------------------------------------------

        private void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Title = "Top Core API",
                    Version = "v1",
                    Contact = new Contact
                    {
                        Name = "Top Nguyen",
                        Email = "TopNguyen92@gmail.com",
                        Url = "http://topnguyen.net"
                    }
                });

                options.DescribeAllEnumsAsStrings();

                var apiDocumentFilePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
                    "TopCore.SSO.xml");
                options.IncludeXmlComments(apiDocumentFilePath);
            });
        }

        private void UseSwagger(IApplicationBuilder app)
        {
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "api-docs/{documentName}/TopCore.SSO.json";
                c.PreSerializeFilters.Add((swaggerDoc, httpReq) => swaggerDoc.Host = httpReq.Host.Value);
            });

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "api";
                c.SwaggerEndpoint("/api-docs/v1/TopCore.SSO.json", "Top Core API");
            });
        }

        //------------------------------------------------------------
        // Others
        //------------------------------------------------------------

        private void UseLogFactory(ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
        }

        private static void UseExceptionPage(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
        }
    }
}
```

## UserController.cs
```c#
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TopCore.SSO.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IIdentityServerInteractionService _interaction;

        public UserController(UserManager<IdentityUser> userManager, IIdentityServerInteractionService interaction)
        {
            _userManager = userManager;
            _interaction = interaction;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                var identityUser = await _userManager.FindByNameAsync(model.Username).ConfigureAwait(true);

                if (identityUser != null && await _userManager.CheckPasswordAsync(identityUser, model.Password).ConfigureAwait(true))
                {
                    AuthenticationProperties props = null;

                    if (model.RememberLogin)
                    {
                        props = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(1)
                        };
                    };

                    await HttpContext.Authentication.SignInAsync(identityUser.Id, identityUser.UserName).ConfigureAwait(true);

                    if (_interaction.IsValidReturnUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    return Redirect("~/");
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        {
            var info = await HttpContext.Authentication.GetAuthenticateInfoAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme).ConfigureAwait(true);
            var tempUser = info?.Principal;
            if (tempUser == null)
            {
                throw new Exception("External authentication error");
            }

            var claims = tempUser.Claims.ToList();

            var userIdClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
            if (userIdClaim == null)
            {
                userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            }
            if (userIdClaim == null)
            {
                throw new Exception("Unknown userid");
            }

            claims.Remove(userIdClaim);
            var provider = info.Properties.Items["scheme"];
            var userId = userIdClaim.Value;

            var user = await _userManager.FindByLoginAsync(provider, userId).ConfigureAwait(true);
            if (user == null)
            {
                user = new IdentityUser { UserName = Guid.NewGuid().ToString() };
                await _userManager.CreateAsync(user).ConfigureAwait(true);
                await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, userId, provider)).ConfigureAwait(true);
            }

            var additionalClaims = new List<Claim>();

            var sid = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                additionalClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            await HttpContext.Authentication
                .SignInAsync(user.Id, user.UserName, provider, additionalClaims.ToArray()).ConfigureAwait(true);

            await HttpContext.Authentication
                .SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme).ConfigureAwait(true);

            return Redirect(_interaction.IsValidReturnUrl(returnUrl) ? returnUrl : "~/");
        }
    }

    public class LoginInputModel
    {
        public bool RememberLogin { get; set; }
        public string ReturnUrl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
```
applicationsetting.json
```markup
{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "ConnectionStrings": {
    "Identity": "Server=.;Database=TopCoreIdentity;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```