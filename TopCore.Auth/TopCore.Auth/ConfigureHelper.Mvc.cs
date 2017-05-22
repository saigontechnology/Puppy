using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TopCore.Auth
{
    public static partial class ConfigureHelper
    {
        public static class Mvc
        {
            public static void Service(IServiceCollection services)
            {
                services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

                services.AddMvc()
                    .AddXmlDataContractSerializerFormatters()
                    .AddJsonOptions(options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;

                        // Indented for Development only
                        options.SerializerSettings.Formatting = Environment.IsDevelopment()
                            ? Formatting.Indented
                            : Formatting.None;

                        // Serialize Json as Camel case
                        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    });

                services.Configure<RazorViewEngineOptions>(options =>
                {
                    options.AreaViewLocationFormats.Clear();
                    options.AreaViewLocationFormats.Add("/Areas/{2}/Views/{1}/{0}.cshtml");
                    options.AreaViewLocationFormats.Add("/Views/Shared/{0}.cshtml");
                });
            }

            public static void Middleware(IApplicationBuilder app)
            {
                if (Environment.IsDevelopment())
                    app.UseBrowserLink();

                app.UseStaticFiles();

                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"assets",
                        "images", "favicons")),
                    RequestPath = new PathString("/favicons")
                });

                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Areas",
                        "Developers", "SwaggerTemplate")),
                    RequestPath = new PathString("/developers/template")
                });

                app.UseMvc(routes =>
                {
                    routes.MapRoute("areaRoute",
                        "{area:exists}/{controller=Home}/{action=Index}");

                    routes.MapRoute(
                        "default",
                        "{controller=Home}/{action=Index}/{id?}");
                });
            }
        }
    }
}