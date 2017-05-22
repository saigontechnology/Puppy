using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Text;

namespace TopCore.Auth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.Title = "Eatup SSO";

            IWebHostBuilder hostBuilder =
                new WebHostBuilder()
                    .UseKestrel(options =>
                    {
                        options.AddServerHeader = false;
                    })
                    .UseWebRoot(Domain.Constants.System.WebRoot)
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration()
                    .UseStartup<Startup>();

            hostBuilder.Build().Run();
        }
    }
}