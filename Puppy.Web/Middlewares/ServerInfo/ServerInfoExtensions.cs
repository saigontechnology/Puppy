#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Puppy </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Puppy </Project>
//     <File>
//         <Name> SystemInfoExtensions.cs </Name>
//         <Created> 11/08/17 12:12:23 AM </Created>
//         <Key> 68cab4fd-dfa3-4274-866e-c85b70b49460 </Key>
//     </File>
//     <Summary>
//         SystemInfoExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Puppy.Core.DictionaryUtils;
using Puppy.Core.EnvironmentUtils;
using Puppy.Web.Constants;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Puppy.Web.Middlewares.ServerInfo
{
    public static class ServerInfoExtensions
    {
        public const string DefaultConfigSection = "ServerInfo";

        /// <summary>
        ///     Use server info to let client app know about server author and server time zone 
        /// </summary>
        /// <param name="app">          </param>
        /// <param name="configuration"></param>
        /// <param name="configSection"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseServerInfo(this IApplicationBuilder app, IConfiguration configuration, string configSection = DefaultConfigSection)
        {
            configuration.BuildSystemInfoConfig(configSection);
            app.UseMiddleware<SystemInfoMiddleware>();
            return app;
        }

        public class SystemInfoMiddleware
        {
            private readonly RequestDelegate _next;

            public SystemInfoMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public Task Invoke(HttpContext context)
            {
                context.Response.OnStarting(state =>
                {
                    var httpContext = (HttpContext)state;

                    httpContext.Response.Headers.AddOrUpdate(HeaderKey.Server, ServerInfoConfig.Name);
                    httpContext.Response.Headers.AddOrUpdate(HeaderKey.XPoweredBy, ServerInfoConfig.PoweredBy);
                    httpContext.Response.Headers.AddOrUpdate(HeaderKey.XAuthorName, ServerInfoConfig.AuthorName);
                    httpContext.Response.Headers.AddOrUpdate(HeaderKey.XAuthorWebsite, ServerInfoConfig.AuthorWebsite);
                    httpContext.Response.Headers.AddOrUpdate(HeaderKey.XAuthorEmail, ServerInfoConfig.AuthorEmail);

                    return Task.CompletedTask;
                }, context);

                return _next(context);
            }
        }

        public static void BuildSystemInfoConfig(this IConfiguration configuration, string configSection = DefaultConfigSection)
        {
            var isHaveConfig = configuration.GetChildren().Any(x => x.Key == configSection);
            if (isHaveConfig)
            {
                var name = configuration.GetValue<string>($"{configSection}:{nameof(ServerInfoConfig.Name)}");

                if (!string.IsNullOrWhiteSpace(name))
                {
                    ServerInfoConfig.Name = name;
                }

                var poweredBy = configuration.GetValue<string>($"{configSection}:{nameof(ServerInfoConfig.PoweredBy)}");
                if (!string.IsNullOrWhiteSpace(name))
                {
                    ServerInfoConfig.PoweredBy = poweredBy;
                }

                var authorName = configuration.GetValue<string>($"{configSection}:{nameof(ServerInfoConfig.AuthorName)}");
                if (!string.IsNullOrWhiteSpace(authorName))
                {
                    ServerInfoConfig.AuthorName = authorName;
                }

                var authorWebsite = configuration.GetValue<string>($"{configSection}:{nameof(ServerInfoConfig.AuthorWebsite)}");
                if (!string.IsNullOrWhiteSpace(authorWebsite))
                {
                    ServerInfoConfig.AuthorWebsite = authorWebsite;
                }

                var authorEmail = configuration.GetValue<string>($"{configSection}:{nameof(ServerInfoConfig.AuthorEmail)}");
                if (!string.IsNullOrWhiteSpace(authorEmail))
                {
                    ServerInfoConfig.AuthorEmail = authorEmail;
                }
            }

            if (!EnvironmentHelper.IsDevelopment()) return;

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"System Info Response: {HeaderKey.Server}: {ServerInfoConfig.Name}," +
                              $" {HeaderKey.XPoweredBy}: {ServerInfoConfig.PoweredBy}," +
                              $" {HeaderKey.XAuthorName}: {ServerInfoConfig.AuthorName}," +
                              $" {HeaderKey.XAuthorWebsite}: {ServerInfoConfig.AuthorWebsite}," +
                              $" {HeaderKey.XAuthorEmail}: {ServerInfoConfig.AuthorEmail}");
            Console.ResetColor();
        }
    }
}