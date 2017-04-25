using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TopCore.Framework.DependencyInjection;
using TopCore.WebAPI.Data.EF.Factory;
using TopCore.WebAPI.Service;

namespace TopCore.WebAPI
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

            // Use Entity Framework
            services.AddDbContext<Data.EF.DbContext>(builder =>
                builder.UseSqlServer(
                    ConfigureHelper.Configuration.GetConnectionString(ConfigureHelper.Environment.EnvironmentName),
                    options => options.MigrationsAssembly(typeof(IDataModule).GetTypeInfo().Assembly.GetName().Name)));

            IMigrationService migrationService = services.Resolve<IMigrationService>();

            // migration database, need synchronous
            migrationService.MigrateDatabase().Wait();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            // The order of middleware very important for request and response handle! Don't mad it

            // [First] Authenticate and Authorize Request
            app.UseMiddleware<ConfigureHelper.Swagger.AccessMiddleware>();
            app.UseMiddleware<ConfigureHelper.Log.AccessMiddleware>();

            // [2] Logger and Exception handle
            ConfigureHelper.Log.Middleware(app, loggerFactory);
            ConfigureHelper.Exception.Middleware(app);

            // [3] Track Time Execute
            app.UseMiddleware<ConfigureHelper.ProcessingTimeMiddleware>();

            // [5] External UI Middleware
            ConfigureHelper.Swagger.Middleware(app);

            // [Final] Response
            app.UseMiddleware<ConfigureHelper.SystemInfoMiddleware>();

            // [Final] Execute Middleware: MVC and API
            ConfigureHelper.Mvc.Middleware(app);
            ConfigureHelper.Api.Middleware(app);
        }
    }
}