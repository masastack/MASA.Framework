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
        var resource = new ApiResource("ceshi", "测试", "这是个测试", "", default, default, default, default);
        await dbContext.ApiResources.AddAsync(resource);
        await dbContext.SaveChangesAsync();
        var sync = serviceProvider.GetRequiredService<SyncCache>();
        await sync.SyncApiResourceCacheAsync(resource.Id);
        var cache = serviceProvider.GetRequiredService<IApiResourceCache>();
        var resources = await cache.GetListAsync();
        Assert.IsTrue(resources.Any(item => item.Name == resource.Name));
    }

    [TestMethod]
    public async Task TestSyncApiScopeCacheAsync()
    {
        var serviceCollection = InitializingData();
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var dbContext = (CustomDbContext)serviceProvider.GetRequiredService<OidcDbContext>().Dbcontext;
        var scope = new ApiScope("ceshi");
        await dbContext.ApiScopes.AddAsync(scope);
        await dbContext.SaveChangesAsync();
        var sync = serviceProvider.GetRequiredService<SyncCache>();
        await sync.SyncApiScopeCacheAsync(scope.Id);
        var cache = serviceProvider.GetRequiredService<IApiScopeCache>();
        var scopes = await cache.GetListAsync();
        Assert.IsTrue(scopes.Any(item => item.Name == scope.Name));
    }

    [TestMethod]
    public async Task TestIdentityResourceCacheAsync()
    {
        var serviceCollection = InitializingData();
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var dbContext = (CustomDbContext)serviceProvider.GetRequiredService<OidcDbContext>().Dbcontext;
        var resource = new IdentityResource("ceshi", "测试", "这是个测试", default, default, default, default, default);
        await dbContext.IdentityResources.AddAsync(resource);
        await dbContext.SaveChangesAsync();
        var sync = serviceProvider.GetRequiredService<SyncCache>();
        await sync.SyncIdentityResourceCacheAsync(resource.Id);
        var cache = serviceProvider.GetRequiredService<IIdentityResourceCache>();
        var resources = await cache.GetListAsync();
        Assert.IsTrue(resources.Any(item => item.Name == resource.Name));
    }

    [TestMethod]
    public async Task TestRemoveApiResourceCacheAsync()
    {
        var serviceCollection = InitializingData();
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var dbContext = (CustomDbContext)serviceProvider.GetRequiredService<OidcDbContext>().Dbcontext;
        var resource = new ApiResource("ceshi2", "测试", "这是个测试", "", default, default, default, default);
        await dbContext.ApiResources.AddAsync(resource);
        await dbContext.SaveChangesAsync();
        var sync = serviceProvider.GetRequiredService<SyncCache>();
        await sync.SyncApiResourceCacheAsync(resource.Id);
        var cache = serviceProvider.GetRequiredService<IApiResourceCache>();
        var resources = await cache.GetListAsync();
        Assert.IsTrue(resources.Any(item => item.Name == resource.Name));
        await sync.RemoveApiResourceCacheAsync(resource);
        resources = await cache.GetListAsync();
        Assert.IsTrue(resources.Any(item => item.Name == resource.Name) is false);
    }

    [TestMethod]
    public async Task TestRemoveApiScopeCacheAsync()
    {
        var serviceCollection = InitializingData();
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var dbContext = (CustomDbContext)serviceProvider.GetRequiredService<OidcDbContext>().Dbcontext;
        var scope = new ApiScope("ceshi2");
        await dbContext.ApiScopes.AddAsync(scope);
        await dbContext.SaveChangesAsync();
        var sync = serviceProvider.GetRequiredService<SyncCache>();
        await sync.SyncApiScopeCacheAsync(scope.Id);
        var cache = serviceProvider.GetRequiredService<IApiScopeCache>();
        var scopes = await cache.GetListAsync();
        Assert.IsTrue(scopes.Any(item => item.Name == scope.Name));
        await sync.RemoveApiScopeCacheAsync(scope);
        scopes = await cache.GetListAsync();
        Assert.IsTrue(scopes.Any(item => item.Name == scope.Name) is false);
    }

    [TestMethod]
    public async Task TestRemoveIdentityResourceCacheAsync()
    {
        var serviceCollection = InitializingData();
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var dbContext = (CustomDbContext)serviceProvider.GetRequiredService<OidcDbContext>().Dbcontext;
        var resource = new IdentityResource("ceshi2", "测试", "这是个测试", default, default, default, default, default);
        await dbContext.IdentityResources.AddAsync(resource);
        await dbContext.SaveChangesAsync();
        var sync = serviceProvider.GetRequiredService<SyncCache>();
        await sync.SyncIdentityResourceCacheAsync(resource.Id);
        var cache = serviceProvider.GetRequiredService<IIdentityResourceCache>();
        var resources = await cache.GetListAsync();
        Assert.IsTrue(resources.Any(item => item.Name == resource.Name));
        await sync.RemoveIdentityResourceCacheAsync(resource);
        resources = await cache.GetListAsync();
        Assert.IsTrue(resources.Any(item => item.Name == resource.Name) is false);
    }

    [TestMethod]
    public async Task TestResetAsync()
    {
        var serviceCollection = InitializingData();
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var dbContext = (CustomDbContext)serviceProvider.GetRequiredService<OidcDbContext>().Dbcontext;
        await dbContext.ApiResources.AddAsync(new ApiResource("ceshi3", "测试", "这是个测试", "", default, default, default, default));
        await dbContext.ApiScopes.AddAsync(new ApiScope("ceshi3"));
        await dbContext.IdentityResources.AddAsync(new IdentityResource("ceshi3", "测试", "这是个测试", default, default, default, default, default));
        await dbContext.SaveChangesAsync();
        var sync = serviceProvider.GetRequiredService<SyncCache>();
        await sync.ResetAsync();
        var apiResourceCache = serviceProvider.GetRequiredService<IApiResourceCache>();
        var apiResources = await apiResourceCache.GetListAsync();
        Assert.IsTrue(apiResources.Count > 0);
        var apiScopeCache = serviceProvider.GetRequiredService<IApiScopeCache>();
        var apiScopes = await apiScopeCache.GetListAsync();
        Assert.IsTrue(apiScopes.Count > 0);
        var identityResourceCache = serviceProvider.GetRequiredService<IIdentityResourceCache>();
        var identityResources = await identityResourceCache.GetListAsync();
        Assert.IsTrue(identityResources.Count > 0);
    }

    static ServiceCollection InitializingData()
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
            .UseUoW<CustomDbContext>(dbOptions => dbOptions.UseInMemoryTestDatabase("TestSeedStandardResources1"), false)
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
