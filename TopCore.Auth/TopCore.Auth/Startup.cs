using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TopCore.Auth
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            StartupHelper.Configuration = builder.Build();
            StartupHelper.Environment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            StartupHelper.Mvc.Add(services);

            StartupHelper.API.Add(services);

            StartupHelper.Log.Add(services);

            StartupHelper.Swagger.Add(services);

            StartupHelper.DependencyInjection.Add(services);

            StartupHelper.IdentityServer.Add(services, StartupHelper.Configuration.GetConnectionString(StartupHelper.Environment.EnvironmentName));

            StartupHelper.IdentityServer.SeedData(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            StartupHelper.Mvc.Use(app);

            StartupHelper.API.Use(app);

            StartupHelper.Log.Use(loggerFactory);

            StartupHelper.Swagger.Use(app);

            StartupHelper.IdentityServer.Use(app);
        }
    }
}