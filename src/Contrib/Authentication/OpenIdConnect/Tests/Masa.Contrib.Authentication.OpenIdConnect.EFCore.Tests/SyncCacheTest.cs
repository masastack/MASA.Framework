// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Tests;

[TestClass]
public class SyncCacheTest
{
    [TestMethod]
    public async Task TestSyncApiResourceCacheAsync()
    {
        var serviceCollection = InitializingData();
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var dbContext = (CustomDbContext)serviceProvider.GetRequiredService<OidcDbContext>().Dbcontext;
        await dbContext.ApiResources.AddAsync(new ApiResource("ceshi", "测试", "这是个测试", "", default, default, default, default));
        await dbContext.SaveChangesAsync();
        var sync = serviceProvider.GetRequiredService<SyncCache>();
        await sync.SyncApiResourceCacheAsync(1);
        var cache = serviceProvider.GetRequiredService<IApiResourceCache>();
        var resources = await cache.GetListAsync();
        Assert.IsTrue(resources.Count == 1);
    }

    [TestMethod]
    public async Task TestSyncApiScopeCacheAsync()
    {
        var serviceCollection = InitializingData();
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var dbContext = (CustomDbContext)serviceProvider.GetRequiredService<OidcDbContext>().Dbcontext;
        await dbContext.ApiScopes.AddAsync(new ApiScope("ceshi"));
        await dbContext.SaveChangesAsync();
        var sync = serviceProvider.GetRequiredService<SyncCache>();
        await sync.SyncApiScopeCacheAsync(1);
        var cache = serviceProvider.GetRequiredService<IApiScopeCache>();
        var scopes = await cache.GetListAsync();
        Assert.IsTrue(scopes.Count == 1);
    }

    [TestMethod]
    public async Task TestIdentityResourceCacheAsync()
    {
        var serviceCollection = InitializingData();
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var dbContext = (CustomDbContext)serviceProvider.GetRequiredService<OidcDbContext>().Dbcontext;
        await dbContext.IdentityResources.AddAsync(new IdentityResource("ceshi", "测试", "这是个测试", default, default, default, default, default));
        await dbContext.SaveChangesAsync();
        var sync = serviceProvider.GetRequiredService<SyncCache>();
        await sync.SyncIdentityResourceCacheAsync(1);
        var cache = serviceProvider.GetRequiredService<IIdentityResourceCache>();
        var resources = await cache.GetListAsync();
        Assert.IsTrue(resources.Count == 1);
    }

    [TestMethod]
    public async Task TestRemoveApiResourceCacheAsync()
    {
        var serviceCollection = InitializingData();
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var dbContext = (CustomDbContext)serviceProvider.GetRequiredService<OidcDbContext>().Dbcontext;
        var resource = new ApiResource("ceshi", "测试", "这是个测试", "", default, default, default, default);
        await dbContext.ApiResources.AddAsync(resource);
        await dbContext.SaveChangesAsync();
        var sync = serviceProvider.GetRequiredService<SyncCache>();
        await sync.SyncApiResourceCacheAsync(1);
        var cache = serviceProvider.GetRequiredService<IApiResourceCache>();
        var resources = await cache.GetListAsync();
        Assert.IsTrue(resources.Count == 1);
        await sync.RemoveApiResourceCacheAsync(resource);      
        resources = await cache.GetListAsync();
        Assert.IsTrue(resources.Count == 0);
    }

    [TestMethod]
    public async Task TestRemoveApiScopeCacheAsync()
    {
        var serviceCollection = InitializingData();
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var dbContext = (CustomDbContext)serviceProvider.GetRequiredService<OidcDbContext>().Dbcontext;
        var scope = new ApiScope("ceshi");
        await dbContext.ApiScopes.AddAsync(scope);
        await dbContext.SaveChangesAsync();
        var sync = serviceProvider.GetRequiredService<SyncCache>();
        await sync.SyncApiScopeCacheAsync(1);
        var cache = serviceProvider.GetRequiredService<IApiScopeCache>();
        var scopes = await cache.GetListAsync();
        Assert.IsTrue(scopes.Count == 1);
        await sync.RemoveApiScopeCacheAsync(scope);       
        scopes = await cache.GetListAsync();
        Assert.IsTrue(scopes.Count == 0);
    }

    [TestMethod]
    public async Task TestRemoveIdentityResourceCacheAsync()
    {
        var serviceCollection = InitializingData();
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var dbContext = (CustomDbContext)serviceProvider.GetRequiredService<OidcDbContext>().Dbcontext;
        var resource = new IdentityResource("ceshi", "测试", "这是个测试", default, default, default, default, default);
        await dbContext.IdentityResources.AddAsync(resource);
        await dbContext.SaveChangesAsync();
        var sync = serviceProvider.GetRequiredService<SyncCache>();
        await sync.SyncIdentityResourceCacheAsync(1);
        var cache = serviceProvider.GetRequiredService<IIdentityResourceCache>();
        var resources = await cache.GetListAsync();
        Assert.IsTrue(resources.Count == 1);
        await sync.RemoveIdentityResourceCacheAsync(resource);
        resources = await cache.GetListAsync();
        Assert.IsTrue(resources.Count == 0);
    }

    [TestMethod]
    public async Task TestResetAsync()
    {
        var serviceCollection = InitializingData();
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var dbContext = (CustomDbContext)serviceProvider.GetRequiredService<OidcDbContext>().Dbcontext;
        await dbContext.ApiResources.AddAsync(new ApiResource("ceshi", "测试", "这是个测试", "", default, default, default, default));
        await dbContext.ApiScopes.AddAsync(new ApiScope("ceshi"));
        await dbContext.IdentityResources.AddAsync(new IdentityResource("ceshi", "测试", "这是个测试", default, default, default, default, default));
        await dbContext.SaveChangesAsync();
        var sync = serviceProvider.GetRequiredService<SyncCache>();
        await sync.ResetAsync();
        var apiResourceCache = serviceProvider.GetRequiredService<IApiResourceCache>();
        var apiResources = await apiResourceCache.GetListAsync();
        Assert.IsTrue(apiResources.Count == 1);
        var apiScopeCache = serviceProvider.GetRequiredService<IApiScopeCache>();
        var apiScopes = await apiScopeCache.GetListAsync();
        Assert.IsTrue(apiScopes.Count == 1);
        var identityResourceCache = serviceProvider.GetRequiredService<IIdentityResourceCache>();
        var identityResources = await identityResourceCache.GetListAsync();
        Assert.IsTrue(identityResources.Count == 1);
    }

    ServiceCollection InitializingData()
    {
        var serviceCollection = new ServiceCollection();
        var publisher = new Mock<IPublisher>();
        serviceCollection.TryAddSingleton(serviceProvider => publisher.Object);
        serviceCollection.AddDomainEventBus(dispatcherOptions =>
        {
            dispatcherOptions
            .UseIntegrationEventBus<IntegrationEventLogService>(options => options.UseEventLog<CustomDbContext>())
            .UseEventBus(eventBusBuilder =>
            {
            })
            .UseUoW<CustomDbContext>(
                dbOptions => dbOptions.UseInMemoryTestDatabase("TestSeedStandardResources1"), false, false
            )
            .UseRepository<CustomDbContext>();
        });
        serviceCollection.AddScoped(provider => new OidcDbContext(provider.GetRequiredService<CustomDbContext>()));
        var options = new RedisConfigurationOptions()
        {
            Servers = new List<RedisServerOptions>
            {
                new RedisServerOptions
                {
                    Host = "127.0.0.1",
                    Port = 6379
                }
            }
        };
        serviceCollection.AddOidcCache(options);
        serviceCollection.AddScoped<SyncCache>();
        return serviceCollection;
    }
}
