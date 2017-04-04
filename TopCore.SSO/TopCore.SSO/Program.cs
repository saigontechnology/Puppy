using Microsoft.AspNetCore.Hosting;
using System.IO;
using TopCore.Framework.Web;

namespace TopCore.SSO
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IWebHostBuilder hostBuilder =
                new WebHostBuilder()
                    .UseKestrel()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration()
                    .UseStartup<Startup>();

            hostBuilder.RunWithBrowser();
        }
    }
}