// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.Cache.Tests;

[TestClass]
public class TestMemoryCacheProvider
{
    [TestMethod]
    public void TestGetMemoryCacheClient()
    {
        var services = new ServiceCollection();
        services.AddOidcCache(new RedisConfigurationOptions()
        {
            Servers = new List<RedisServerOptions>()
            {
                new()
            }
        });

        var serviceProvider = services.BuildServiceProvider();
        var memoryCacheProvider = serviceProvider.GetService<MemoryCacheProvider>();
        Assert.IsNotNull(memoryCacheProvider);
        var multilevelCacheClient = memoryCacheProvider.GetMemoryCacheClient();
        Assert.IsNotNull(multilevelCacheClient);
    }
}
