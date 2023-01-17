// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Tests;

[TestClass]
public class SyncCacheTest
{
    private Mock<IApiResourceCache> _apiResourceCache;
    private Mock<IClientCache> _clientCache;
    private Mock<IApiScopeCache> _apiScopeCache;
    private Mock<IIdentityResourceCache> _identityResourceCache;

    [TestInitialize]
    public void Initialize()
    {
        _apiResourceCache = new();
        _clientCache = new();
        _apiScopeCache = new();
        _identityResourceCache = new();
    }

    [TestMethod]
    public async Task TestSyncApiResourceCacheAsync()
    {
        var services = InitializingData();
        await using var serviceProvider = services.BuildServiceProvider();
        var oidcDbContext = new OidcDbContext(serviceProvider.GetRequiredService<CustomDbContext>());
        var syncCache = new SyncCache(oidcDbContext, serviceProvider);
        var resource = new ApiResource("不存入数据库", "不存入数据库", "不存入数据库", "", default, default, default, default);
        await syncCache.SyncApiResourceCacheAsync(resource.Id);
        _apiResourceCache.Verify(a => a.SetAsync(It.IsAny<ApiResource>()), Times.Never);
    }

    [TestMethod]
    public async Task TestSyncApiResourceCache2Async()
    {
        var services = InitializingData();
        services.AddSingleton(_ => _apiResourceCache.Object);
        await using var serviceProvider = services.BuildServiceProvider();
        var oidcDbContext = new OidcDbContext(serviceProvider.GetRequiredService<CustomDbContext>());
        var syncCache = new SyncCache(oidcDbContext, serviceProvider);
        var resource = new ApiResource("不存入数据库", "不存入数据库", "不存入数据库", "", default, default, default, default);
        await syncCache.SyncApiResourceCacheAsync(resource.Id);
        _apiResourceCache.Verify(a => a.SetAsync(It.IsAny<ApiResource>()), Times.Never);
    }

    [TestMethod]
    public async Task TestSyncApiResourceCache3Async()
    {
        var services = InitializingData();
        services.AddSingleton(_ => _apiResourceCache.Object);
        await using var serviceProvider = services.BuildServiceProvider();
        var customDbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        var oidcDbContext = new OidcDbContext(customDbContext);
        var syncCache = new SyncCache(oidcDbContext, serviceProvider);
        var apiResource = new ApiResource("ceshi", "测试", "这是个测试", "", default, default, default, default);
        await customDbContext.ApiResources.AddAsync(apiResource);
        await customDbContext.SaveChangesAsync();
        await syncCache.SyncApiResourceCacheAsync(apiResource.Id);
        _apiResourceCache.Verify(a => a.SetAsync(It.IsAny<ApiResource>()), Times.Once);
    }

    [TestMethod]
    public async Task TestSyncApiScopeCacheAsync()
    {
        var services = InitializingData();
        await using var serviceProvider = services.BuildServiceProvider();
        var customDbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        var oidcDbContext = new OidcDbContext(customDbContext);
        var syncCache = new SyncCache(oidcDbContext, serviceProvider);
        var scope = new ApiScope("不存入数据库");
        await syncCache.SyncApiScopeCacheAsync(scope.Id);
        _apiScopeCache.Verify(a => a.SetAsync(It.IsAny<ApiScope>()), Times.Never);
    }

    [TestMethod]
    public async Task TestSyncApiScopeCache2Async()
    {
        var services = InitializingData();
        services.AddSingleton(_ => _apiScopeCache.Object);
        await using var serviceProvider = services.BuildServiceProvider();
        var customDbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        var oidcDbContext = new OidcDbContext(customDbContext);
        var syncCache = new SyncCache(oidcDbContext, serviceProvider);
        var scope = new ApiScope("不存入数据库");
        await syncCache.SyncApiScopeCacheAsync(scope.Id);
        _apiScopeCache.Verify(a => a.SetAsync(It.IsAny<ApiScope>()), Times.Never);
    }

    [TestMethod]
    public async Task TestSyncApiScopeCache3Async()
    {
        var services = InitializingData();
        services.AddSingleton(_ => _apiScopeCache.Object);
        await using var serviceProvider = services.BuildServiceProvider();
        var customDbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        var oidcDbContext = new OidcDbContext(customDbContext);
        var syncCache = new SyncCache(oidcDbContext, serviceProvider);
        var scope = new ApiScope("masa");
        await customDbContext.ApiScopes.AddAsync(scope);
        await customDbContext.SaveChangesAsync();
        await syncCache.SyncApiScopeCacheAsync(scope.Id);
        _apiScopeCache.Verify(a => a.SetAsync(It.IsAny<ApiScope>()), Times.Once);
    }

    [TestMethod]
    public async Task TestSyncIdentityResourceCacheAsync()
    {
        var services = InitializingData();
        await using var serviceProvider = services.BuildServiceProvider();
        var customDbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        var oidcDbContext = new OidcDbContext(customDbContext);
        var syncCache = new SyncCache(oidcDbContext, serviceProvider);
        var resource = new IdentityResource("不存入数据库", "不存入数据库", "不存入数据库", default, default, default, default, default);
        await syncCache.SyncIdentityResourceCacheAsync(resource.Id);
        _identityResourceCache.Verify(a => a.SetRangeAsync(It.IsAny<List<IdentityResource>>()), Times.Never);
    }

    [TestMethod]
    public async Task TestSyncIdentityResourceCache2Async()
    {
        var services = InitializingData();
        services.AddSingleton(_ => _identityResourceCache.Object);
        await using var serviceProvider = services.BuildServiceProvider();
        var customDbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        var oidcDbContext = new OidcDbContext(customDbContext);
        var syncCache = new SyncCache(oidcDbContext, serviceProvider);
        var resource = new IdentityResource("不存入数据库", "不存入数据库", "不存入数据库", default, default, default, default, default);
        await syncCache.SyncIdentityResourceCacheAsync(resource.Id);
        _identityResourceCache.Verify(a => a.SetRangeAsync(It.IsAny<List<IdentityResource>>()), Times.Never);
    }

    [TestMethod]
    public async Task TestSyncIdentityResourceCache3Async()
    {
        var services = InitializingData();
        services.AddSingleton(_ => _identityResourceCache.Object);
        await using var serviceProvider = services.BuildServiceProvider();
        var customDbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        var oidcDbContext = new OidcDbContext(customDbContext);
        var syncCache = new SyncCache(oidcDbContext, serviceProvider);
        var resource = new IdentityResource("ceshi", "测试", "这是个测试", default, default, default, default, default);
        await customDbContext.IdentityResources.AddAsync(resource);
        await customDbContext.SaveChangesAsync();
        await syncCache.SyncIdentityResourceCacheAsync(resource.Id);
        _identityResourceCache.Verify(a => a.SetRangeAsync(It.IsAny<List<IdentityResource>>()), Times.Once);
    }

    static ServiceCollection InitializingData()
    {
        var services = new ServiceCollection();
        services.AddMasaDbContext<CustomDbContext>(dbContext => dbContext.UseInMemoryTestDatabase(Guid.NewGuid().ToString()));
        services.AddScoped(provider => new OidcDbContext(provider.GetRequiredService<CustomDbContext>()));


        var options = new RedisConfigurationOptions()
        {
            Servers = new List<RedisServerOptions>
            {
                new()
                {
                    Host = "127.0.0.1",
                    Port = 6379
                }
            }
        };
        services.AddOidcCache(options);
        services.AddScoped<SyncCache>();
        return services;
    }
}
