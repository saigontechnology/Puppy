using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace TopCore.Auth
{
    public static partial class ConfigureHelper
    {
        public static IConfigurationRoot Configuration;
        public static IHostingEnvironment Environment;

        public static string DeveloperAccessKeyConfig => Configuration.GetValue<string>("Developers:AccessKey");
    }
}