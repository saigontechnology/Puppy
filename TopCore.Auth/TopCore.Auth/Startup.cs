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

            ConfigureHelper.Configuration = builder.Build();
            ConfigureHelper.Environment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureHelper.Mvc.Service(services);

            ConfigureHelper.Api.Service(services);

            ConfigureHelper.Swagger.Service(services);

            ConfigureHelper.Log.Service(services);

            ConfigureHelper.DependencyInjection.Service(services);

            ConfigureHelper.IdentityServer.Service(services, ConfigureHelper.Configuration.GetConnectionString(ConfigureHelper.Environment.EnvironmentName));

            ConfigureHelper.IdentityServer.SeedData(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            ConfigureHelper.Log.Middleware(app, loggerFactory);

            ConfigureHelper.Exception.Middleware(app);

            ConfigureHelper.IdentityServer.Middleware(app);

            ConfigureHelper.Swagger.Middleware(app);

            ConfigureHelper.Mvc.Middleware(app);

            ConfigureHelper.Api.Middleware(app);
        }
    }
}