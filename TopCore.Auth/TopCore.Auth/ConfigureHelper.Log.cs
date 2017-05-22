using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;

namespace TopCore.Auth
{
    public static partial class ConfigureHelper
    {
        public static class Log
        {
            public static void Middleware(IApplicationBuilder app, ILoggerFactory loggerFactory)
            {
                // Write log
                Serilog.Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();
                loggerFactory.AddSerilog();
            }
        }
    }
}