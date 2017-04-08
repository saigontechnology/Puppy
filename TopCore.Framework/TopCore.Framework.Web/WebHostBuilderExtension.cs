﻿#region	License

//------------------------------------------------------------------------------------------------
// <Auto-generated>
//     <Author> Top Nguyen (http://topnguyen.net) </Author>
//     <Project> TopCore.Framework.Web </Project>
//     <File> 
//         <Name> WebHostBuilderExtension </Name>
//         <Created> 28 03 2017 10:08:49 AM </Created>
//         <Key> 26578273-3F40-4631-A1E7-6C33F72AD452 </Key>
//     </File>
//     <Summary>
//         WebHostBuilderExtension
//     </Summary>
// </Auto-generated>
//------------------------------------------------------------------------------------------------

#endregion License

using Microsoft.AspNetCore.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace TopCore.Framework.Web
{
    public static class WebHostBuilderExtension
    {
        public static void BuildAndRun(this IWebHostBuilder hostBuilder)
        {
            hostBuilder.Build().Run();
        }

        /// <summary>
        ///     Set host listener for domain URL and start it in browser
        /// </summary>
        /// <param name="hostBuilder"></param>
        public static void BuildAndRunWithBrowser(this IWebHostBuilder hostBuilder)
        {
            var configFileFullPath = Path.Combine(Directory.GetCurrentDirectory(), "Properties", "launchSettings.json");
            string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var sectionQuery = $"profiles:{environmentName}:launchUrl";
            var domainUrl = Core.ConfigHelper.GetValue(configFileFullPath, sectionQuery);
            BuildAndRunWithBrowser(hostBuilder, domainUrl);
        }

        /// <summary>
        ///     Set host listener for domain URL and start it in browser
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="domainUrl"></param>
        public static void BuildAndRunWithBrowser(this IWebHostBuilder hostBuilder, string domainUrl)
        {
            // Update domain URL for builder
            hostBuilder.UseUrls(domainUrl);

            // Build Host
            var host = hostBuilder.Build();

            // Run Browser before Host Run
            StartBrowser(domainUrl);

            // Run Host
            host.Run();
        }

        #region Private Helper Methods

        /// <summary>
        ///     Start Browser by CMD with separate thread
        /// </summary>
        private static void StartBrowser(string url)
        {
            new Thread(() =>
            {
                try
                {
                    OpenBrowser(url);
                }
                catch
                {
                    Console.WriteLine($"{nameof(StartBrowser)} catch!");
                }
            }).Start();
        }

        /// <summary>
        ///     Open browser depend on Platform
        /// </summary>
        private static void OpenBrowser(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = $"/c start {url}",
                    CreateNoWindow = true
                };
                Process.Start(processStartInfo);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            Debug.WriteLine($"{nameof(OpenBrowser)} not cover!");
        }

        #endregion
    }
}