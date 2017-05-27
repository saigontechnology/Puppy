using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace TopCore.Auth
{
    public static partial class ConfigureHelper
    {
        public static IConfigurationRoot ConfigurationRoot;
        public static IHostingEnvironment Environment;

        public static string DeveloperAccessKeyConfig => ConfigurationRoot.GetValue<string>("Developers:AccessKey");
    }
}