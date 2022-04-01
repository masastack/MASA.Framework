using Masa.BuildingBlocks.BasicAbility.Pm;
using Masa.Utils.Caching.Redis.Models;
using Masa.Utils.Caller.Core;
using Masa.Utils.Caller.HttpClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Masa.Contrib.BasicAbility.Pm.Tests
{
    [TestClass]
    public class PmCachingTest
    {
        private IPmCaching _pmCaching;

        [TestInitialize]
        public void Initialize()
        {
            var services = new ServiceCollection();

            services.AddPmCaching(redisOptions =>
            {
                redisOptions.Servers = new List<RedisServerOptions>
                {
                    new RedisServerOptions("10.10.90.37",30120)
                };
                redisOptions.DefaultDatabase = 0;
                redisOptions.Password = "p@ssw0rd";
            }, callerOptions =>
            {
                callerOptions.UseHttpClient(builder =>
                {
                    builder.Name = "CustomHttpClient";
                    builder.Configure = opt => opt.BaseAddress = new Uri("https://github.com");
                });
            });

            var serviceProvider = services.BuildServiceProvider();
            _pmCaching = serviceProvider.GetRequiredService<IPmCaching>();
        }

        [TestMethod]
        public async Task GetProjectAppsListAsync()
        {
            var data = await _pmCaching.GetProjectAppsListAsync("development");

            Assert.IsTrue(data.Count >= 0);
        }
    }
}
