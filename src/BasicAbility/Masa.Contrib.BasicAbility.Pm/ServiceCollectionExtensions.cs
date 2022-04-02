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
            services.AddCaller(options =>
            {
                callerOptions.Invoke(options);
            });

            services.AddSingleton<IPmCaching>(serviceProvider => new PmCaching(serviceProvider.GetRequiredService<ICallerFactory>().CreateClient(DEFAULT_CLIENT_NAME)));
        }
    }
}
