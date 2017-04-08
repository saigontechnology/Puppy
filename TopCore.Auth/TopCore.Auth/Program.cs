using Microsoft.AspNetCore.Hosting;
using System.IO;
using TopCore.Framework.Web;

namespace TopCore.Auth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IWebHostBuilder hostBuilder =
                new WebHostBuilder()
                    .UseKestrel()
                    .UseWebRoot("assets")
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration()
                    .UseStartup<Startup>();

#if DEBUG
            hostBuilder.BuildAndRunWithBrowser();
#else
            hostBuilder.BuildAndRun();
#endif
        }
    }
}