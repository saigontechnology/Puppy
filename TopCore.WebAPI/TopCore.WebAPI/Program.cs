using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace TopCore.WebAPI
{
    public class Program
    {
        private static string LaunchUrl { get; set; }

        public static void Main(string[] args)
        {
            IWebHost host = BuildHost();

            // [Important] Run Browser before Host Run
            StartBrowser();

            // Let Dance
            host.Run();
        }

        private static IWebHost BuildHost()
        {
            IWebHostBuilder hostBuilder =
                new WebHostBuilder()
                    .UseKestrel()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration()
                    .UseStartup<Startup>();

            // Fetch Launch URL by Environment
            FetchLaunchUrlByEnvironment();

            // Set URL Listener
            hostBuilder.UseUrls(LaunchUrl);
            IWebHost host = hostBuilder.Build();

            return host;
        }

        private static void FetchLaunchUrlByEnvironment()
        {
            string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var launchSettings = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Properties"))
                .AddJsonFile("launchSettings.json", optional: true)
                .Build();
            LaunchUrl = launchSettings.GetSection($"profiles:{environmentName}:launchUrl").Value;
        }

        /// <summary>
        ///     Start Browser by CMD with separate thread 
        /// </summary>
        private static void StartBrowser()
        {
            new Thread(() =>
            {
                try
                {
                    OpenBrowser();
                }
                catch
                {
                    Debug.WriteLine($"{nameof(StartBrowser)} catch!");
                }
            }).Start();
        }

        /// <summary>
        ///     Open browser depend on Platform 
        /// </summary>
        private static void OpenBrowser()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo("cmd", $"/c start {LaunchUrl}"));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", LaunchUrl);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", LaunchUrl);
            }
            Debug.WriteLine($"{nameof(OpenBrowser)} not cover!");
        }
    }
}