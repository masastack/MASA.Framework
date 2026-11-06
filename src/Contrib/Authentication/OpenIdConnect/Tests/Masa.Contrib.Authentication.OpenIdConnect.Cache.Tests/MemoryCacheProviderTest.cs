// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.Cache.Tests;

[TestClass]
public class MemoryCacheProviderTest
{
    [TestMethod]
    public void TestGetMemoryCacheClient()
    {
        var options = new RedisConfigurationOptions()
        {
            // Each target framework uses its own Redis database, so test projects running concurrently on different TFMs do not share keys
            DefaultDatabase = Environment.Version.Major,
            ChannelPrefix = $"net{Environment.Version.Major}:",
            Servers = new List<RedisServerOptions>
            {
                new RedisServerOptions
                {
                    Host = "127.0.0.1",
                    Port = 6379
                }
            }
        };
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddOidcCache(options);
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        MemoryCacheProvider memoryCacheProvider = serviceProvider.GetRequiredService<MemoryCacheProvider>();
        var memoryCacheClient = memoryCacheProvider.GetMemoryCacheClient();
        Assert.IsNotNull(memoryCacheClient);
    }
}
