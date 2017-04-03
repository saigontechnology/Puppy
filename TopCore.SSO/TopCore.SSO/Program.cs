using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;

namespace TopCore.SSO
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "TopCore.SSO";

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://localhost:55555")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}