﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Reflection;
using TopCore.Auth.Data.Factory;
using TopCore.Auth.Domain.Interfaces.Services;
using TopCore.Auth.Filters;
using TopCore.Framework.DependencyInjection;

namespace TopCore.Auth
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
                builder.AddUserSecrets<Startup>();

            ConfigureHelper.ConfigurationRoot = builder.Build();
            ConfigureHelper.Environment = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(ConfigureHelper.ConfigurationRoot);

            services.AddScoped<DeveloperAccessFilter>();

            services.AddScoped<ApiExceptionFilter>();

            ConfigureHelper.Cros.Service(services);

            ConfigureHelper.Swagger.Service(services);

            ConfigureHelper.Cache.Service(services);

            ConfigureHelper.IdentityServer.Service(services, ConfigureHelper.ConfigurationRoot.GetConnectionString(ConfigureHelper.Environment.EnvironmentName));

            ConfigureHelper.Mvc.Service(services);

            // keep in last service
            ConfigureHelper.DependencyInjection.Service(services);

            // Use Entity Framework
            services.AddDbContext<DbContext>(builder =>
                builder.UseSqlServer(
                    ConfigureHelper.ConfigurationRoot.GetConnectionString(ConfigureHelper.Environment.EnvironmentName),
                    options => options.MigrationsAssembly(typeof(IDataModule).GetTypeInfo().Assembly.GetName().Name)));
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory,
            IApplicationLifetime applicationLifetime)
        {
            // Currently, ASPNETCORE have a bug hit twice when change appsetting.json from 20/03/17
            // (see more: https://github.com/aspnet/Configuration/issues/624)
            ChangeToken.OnChange(ConfigureHelper.ConfigurationRoot.GetReloadToken,
                () => loggerFactory.CreateLogger<Startup>().LogWarning("Configuration Changed"));

            // [Important] The order of middleware very important for request and response handle!
            // Don't mad it

            // Startup
            applicationLifetime.ApplicationStarted.Register(OnStarted(app));

            // [Important] Use Cros first
            ConfigureHelper.Cros.Middleware(app);
            app.UseMiddleware<ConfigureHelper.Cros.ResponseMiddleware>();

            // Response Information
            app.UseMiddleware<ConfigureHelper.SystemInfoMiddleware>();
            app.UseMiddleware<ConfigureHelper.ProcessingTimeMiddleware>();

            // Exception
            ConfigureHelper.Log.Middleware(app, loggerFactory);
            ConfigureHelper.Exception.Middleware(app);

            // Swagger
            ConfigureHelper.Swagger.Middleware(app);
            app.UseMiddleware<ConfigureHelper.Swagger.AccessMiddleware>();

            // Identity Server
            ConfigureHelper.IdentityServer.Middleware(app);

            // [Final] Execute Middleware: MVC
            ConfigureHelper.Mvc.Middleware(app);
        }

        public Action OnStarted(IApplicationBuilder app)
        {
            return () =>
            {
                // Migration database, need synchronous
                ISeedAuthService seedAuthService = app.ApplicationServices.Resolve<ISeedAuthService>();
                seedAuthService.SeedAuthDatabaseAsync().Wait();
            };
        }
    }
}