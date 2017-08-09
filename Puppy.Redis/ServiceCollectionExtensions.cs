#region	License
//------------------------------------------------------------------------------------------------
// <License>
//     <Copyright> 2017 © Top Nguyen → AspNetCore → Monkey </Copyright>
//     <Url> http://topnguyen.net/ </Url>
//     <Author> Top </Author>
//     <Project> Monkey </Project>
//     <File>
//         <Name> ServiceCollectionExtensions.cs </Name>
//         <Created> 27/07/17 1:26:59 AM </Created>
//         <Key> 18da2c44-f356-4e52-b761-b9f453d13e90 </Key>
//     </File>
//     <Summary>
//         ServiceCollectionExtensions.cs
//     </Summary>
// <License>
//------------------------------------------------------------------------------------------------
#endregion License

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Puppy.Core.EnvironmentUtils;
using System;

namespace Puppy.Redis
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration, string configSection = Constant.DefaultConfigSection)
        {
            // Build Config
            configuration.BuildRedisCacheConfig(configSection);

            if (string.IsNullOrWhiteSpace(RedisCacheConfig.ConnectionString))
                throw new ArgumentException($"{nameof(RedisCacheConfig.ConnectionString)} Is Null Or WhiteSpace", nameof(RedisCacheConfig.ConnectionString));

            if (string.IsNullOrWhiteSpace(RedisCacheConfig.InstanceName))
                throw new ArgumentException($"{nameof(RedisCacheConfig.InstanceName)} Is Null Or WhiteSpace", nameof(RedisCacheConfig.InstanceName));

            // Register Dependency for DistributedCache
            services.AddSingleton<IDistributedCache>(factory =>
            {
                var cache = new RedisCache(new RedisCacheOptions
                {
                    Configuration = RedisCacheConfig.ConnectionString,
                    InstanceName = RedisCacheConfig.InstanceName
                });

                return cache;
            });

            // Register Dependency for RedisCacheManager
            services.AddSingleton<IRedisCacheManager, RedisCacheManager>();

            return services;
        }

        public static IServiceCollection AddRedisCache(this IServiceCollection services, string connectionString, string instanceName)
        {
            // Build Config
            RedisCacheConfig.ConnectionString = connectionString;
            RedisCacheConfig.InstanceName = instanceName;

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException($"{nameof(connectionString)} Is Null Or WhiteSpace", nameof(connectionString));

            if (string.IsNullOrWhiteSpace(instanceName))
                throw new ArgumentException($"{nameof(instanceName)} Is Null Or WhiteSpace", nameof(instanceName));

            // Register Dependency for DistributedCache
            services.AddSingleton<IDistributedCache>(factory =>
            {
                var cache = new RedisCache(new RedisCacheOptions
                {
                    Configuration = connectionString,
                    InstanceName = instanceName
                });

                return cache;
            });

            // Register Dependency for RedisCacheManager
            services.AddSingleton<IRedisCacheManager, RedisCacheManager>();

            return services;
        }

        public static void BuildRedisCacheConfig(this IConfiguration configuration, string configSection = Constant.DefaultConfigSection)
        {
            RedisCacheConfig.ConnectionString = configuration.GetValue<string>($"{configSection}:{nameof(RedisCacheConfig.ConnectionString)}");
            RedisCacheConfig.InstanceName = configuration.GetValue<string>($"{configSection}:{nameof(RedisCacheConfig.InstanceName)}");

            if (!EnvironmentHelper.IsDevelopment()) return;

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Redis Cache Connection: {RedisCacheConfig.ConnectionString}, Instance Name: {RedisCacheConfig.InstanceName}");
            Console.ResetColor();
        }
    }
}