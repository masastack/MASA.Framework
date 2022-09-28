// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.Cache.Tests;

[TestClass]
public class ServiceCollectionExtensionsTest
{
    [TestMethod]
    public void TestAddOidcCache()
    {
        var options = new RedisConfigurationOptions();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddOidcCache(options);
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        MemoryCacheProvider? memoryCacheProvider = serviceProvider.GetService<MemoryCacheProvider>();
        IClientCache? clientCache = serviceProvider.GetService<IClientCache>();
        IApiScopeCache? apiScope = serviceProvider.GetService<IApiScopeCache>();
        IApiResourceCache? apiResource = serviceProvider.GetService<IApiResourceCache>();
        IIdentityResourceCache? identityResource = serviceProvider.GetService<IIdentityResourceCache>();
        Assert.IsNotNull(memoryCacheProvider);
        Assert.IsNotNull(clientCache);
        Assert.IsNotNull(apiScope);
        Assert.IsNotNull(apiResource);
        Assert.IsNotNull(identityResource);
    }
}
