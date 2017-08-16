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
using Puppy.Core.EnvironmentUtils;
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

                    // Server
                    if (httpContext.Response.Headers.ContainsKey(ServerInfoConfig.NameHeader))
                    {
                        httpContext.Response.Headers.Remove(ServerInfoConfig.NameHeader);
                    }
                    httpContext.Response.Headers.Add(ServerInfoConfig.NameHeader, ServerInfoConfig.Name);

                    // X-Powered-By
                    if (httpContext.Response.Headers.ContainsKey(ServerInfoConfig.PoweredByHeader))
                    {
                        httpContext.Response.Headers.Remove(ServerInfoConfig.PoweredByHeader);
                    }
                    httpContext.Response.Headers.Add(ServerInfoConfig.PoweredByHeader, ServerInfoConfig.PoweredBy);

                    // X-Author-Name
                    if (httpContext.Response.Headers.ContainsKey(ServerInfoConfig.AuthorNameHeader))
                    {
                        httpContext.Response.Headers.Remove(ServerInfoConfig.AuthorNameHeader);
                    }
                    httpContext.Response.Headers.Add(ServerInfoConfig.AuthorNameHeader, ServerInfoConfig.AuthorName);

                    // X-Author-Website
                    if (httpContext.Response.Headers.ContainsKey(ServerInfoConfig.AuthorWebsiteHeader))
                    {
                        httpContext.Response.Headers.Remove(ServerInfoConfig.AuthorWebsiteHeader);
                    }
                    httpContext.Response.Headers.Add(ServerInfoConfig.AuthorWebsiteHeader, ServerInfoConfig.AuthorWebsite);

                    // X-Author-Email
                    if (httpContext.Response.Headers.ContainsKey(ServerInfoConfig.AuthorEmailHeader))
                    {
                        httpContext.Response.Headers.Remove(ServerInfoConfig.AuthorEmailHeader);
                    }
                    httpContext.Response.Headers.Add(ServerInfoConfig.AuthorEmailHeader, ServerInfoConfig.AuthorEmail);

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

                var cookieSchemaName = configuration.GetValue<string>($"{configSection}:{nameof(ServerInfoConfig.CookieSchemaName)}");
                if (!string.IsNullOrWhiteSpace(cookieSchemaName))
                {
                    ServerInfoConfig.CookieSchemaName = cookieSchemaName;
                }

                var timeZoneId = configuration.GetValue<string>($"{configSection}:{nameof(ServerInfoConfig.TimeZoneId)}");
                if (!string.IsNullOrWhiteSpace(timeZoneId))
                {
                    ServerInfoConfig.TimeZoneId = timeZoneId;
                }
            }

            if (!EnvironmentHelper.IsDevelopment()) return;

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"System Info Response: {ServerInfoConfig.NameHeader}: {ServerInfoConfig.Name}," +
                              $" {ServerInfoConfig.PoweredByHeader}: {ServerInfoConfig.PoweredBy}," +
                              $" {ServerInfoConfig.AuthorNameHeader}: {ServerInfoConfig.AuthorName}," +
                              $" {ServerInfoConfig.AuthorWebsiteHeader}: {ServerInfoConfig.AuthorWebsite}," +
                              $" {ServerInfoConfig.AuthorEmailHeader}: {ServerInfoConfig.AuthorEmail}," +
                              $" {nameof(ServerInfoConfig.CookieSchemaName)}: {ServerInfoConfig.CookieSchemaName}," +
                              $" {nameof(ServerInfoConfig.TimeZoneId)}: {ServerInfoConfig.TimeZoneId}");
            Console.ResetColor();
        }
    }
}