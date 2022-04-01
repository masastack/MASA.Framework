using Masa.Utils.Caching.DistributedMemory;
using Masa.Utils.Caching.DistributedMemory.DependencyInjection;
using Masa.Utils.Caching.Redis.DependencyInjection;
using Masa.Utils.Caching.Redis.Models;
using Masa.Utils.Caller.HttpClient;
using Masa.Utils.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masa.Contrib.BasicAbility.Pm
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPmCaching(this IServiceCollection services, Action<CallerOptions> callerOptions)
        {
            var options = AppSettings.GetModel<RedisConfigurationOptions>("PmRedisSettings");

            services.AddMasaRedisCache(DEFAULT_CLIENT_NAME, options).AddMasaMemoryCache();

            services.AddCaller(options =>
            {
                callerOptions.Invoke(options);
            });

            services.AddSingleton(serviceProvider =>
            {
                var factory = serviceProvider.GetRequiredService<IMemoryCacheClientFactory>();
                var callerFactory = serviceProvider.GetRequiredService<ICallerFactory>();
                var client = factory.CreateClient(DEFAULT_CLIENT_NAME);
                return PmCachingFactory.Create(client, callerFactory);
            });
        }

        public static void AddPmCaching(this IServiceCollection services, Action<RedisConfigurationOptions> configureOptions, Action<CallerOptions> callerOptions)
        {
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.AddMasaRedisCache(DEFAULT_CLIENT_NAME, configureOptions).AddMasaMemoryCache();

            services.AddCaller(options =>
            {
                callerOptions.Invoke(options);
            });

            services.AddSingleton(serviceProvider =>
            {
                var factory = serviceProvider.GetRequiredService<IMemoryCacheClientFactory>();
                var callerFactory = serviceProvider.GetRequiredService<ICallerFactory>();
                var client = factory.CreateClient(DEFAULT_CLIENT_NAME);
                return PmCachingFactory.Create(client, callerFactory);
            });
        }

        public static void AddPmCaching(this IServiceCollection services, RedisConfigurationOptions options, Action<CallerOptions> callerOptions)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            services.AddMasaRedisCache(DEFAULT_CLIENT_NAME, options).AddMasaMemoryCache();

            services.AddCaller(options =>
            {
                callerOptions.Invoke(options);
            });

            services.AddSingleton(serviceProvider =>
            {
                var factory = serviceProvider.GetRequiredService<IMemoryCacheClientFactory>();
                var callerFactory = serviceProvider.GetRequiredService<ICallerFactory>();
                var client = factory.CreateClient(DEFAULT_CLIENT_NAME);
                return PmCachingFactory.Create(client, callerFactory);
            });
        }
    }
}
