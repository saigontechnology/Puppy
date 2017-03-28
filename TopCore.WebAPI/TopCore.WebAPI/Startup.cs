using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;

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
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public string TopCoreVersion { get; } = PlatformServices.Default.Application.ApplicationVersion;

        #region ConfigureServices

        /// <summary>
        ///     This method gets called by the runtime. Use this method to add services to the container. 
        /// </summary>
        /// <param name="services"> The services. </param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            AddSwagger(services);

        }

        private void AddSwagger(IServiceCollection services)
        {
            var apiDocumentFilePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "TopCore.WebAPI.xml");
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc($"v{TopCoreVersion}", new Info { Title = nameof(TopCore), Version = $"v{TopCoreVersion}" });
                options.IncludeXmlComments(apiDocumentFilePath);
                options.DescribeAllEnumsAsStrings();
            });
        }

        #endregion ConfigureServices

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            SetupLogFactory(loggerFactory);
            SetupExceptionPage(app, env);

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });

            UseSwagger(app);
        }

        private void UseSwagger(IApplicationBuilder app)
        {
            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) => swagger.Host = httpReq.Host.Value);
            });

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/API/v{TopCoreVersion}/swagger.json", $"{nameof(TopCore)} v{TopCoreVersion}");
            });
        }

        #region Configure

        /// <summary>
        ///     This method gets called by the runtime. Use this method to configure the HTTP request pipeline. 
        /// </summary>
        /// <param name="loggerFactory"> The logger factory. </param>
        private void SetupLogFactory(ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
        }

        private static void SetupExceptionPage(IApplicationBuilder app, IHostingEnvironment env)
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

        #endregion Configure
    }
}