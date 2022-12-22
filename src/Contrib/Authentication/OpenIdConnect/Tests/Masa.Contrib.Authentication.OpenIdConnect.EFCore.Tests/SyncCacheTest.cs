// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;
using Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Enums;

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Tests;

[TestClass]
public class SyncCacheTest
{
    [TestMethod]
    public async Task TestSyncApiResourceCacheAsync()
    {
        var serviceCollection = InitializingData();
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var sync = serviceProvider.GetRequiredService<SyncCache>();
        var resource = new ApiResource("不存入数据库", "不存入数据库", "不存入数据库", "", default, default, default, default);
        await sync.SyncApiResourceCacheAsync(resource.Id);
        var cache = serviceProvider.GetRequiredService<IApiResourceCache>();
        var resources = await cache.GetListAsync();
        Assert.IsFalse(resources.Any(item => item.Name == resource.Name));

        var dbContext = (CustomDbContext)serviceProvider.GetRequiredService<OidcDbContext>().Dbcontext;
        resource = new ApiResource("ceshi", "测试", "这是个测试", "", default, default, default, default);
        await dbContext.ApiResources.AddAsync(resource);
        await dbContext.SaveChangesAsync();
        await sync.SyncApiResourceCacheAsync(resource.Id);
        resources = await cache.GetListAsync();
        Assert.IsTrue(resources.Any(item => item.Name == resource.Name));
    }

    [TestMethod]
    public async Task TestSyncApiScopeCacheAsync()
    {
        var serviceCollection = InitializingData();
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var sync = serviceProvider.GetRequiredService<SyncCache>();
        var scope = new ApiScope("不存入数据库");
        await sync.SyncApiScopeCacheAsync(scope.Id);
        var cache = serviceProvider.GetRequiredService<IApiScopeCache>();
        var scopes = await cache.GetListAsync();
        Assert.IsFalse(scopes.Any(item => item.Name == scope.Name));

        var dbContext = (CustomDbContext)serviceProvider.GetRequiredService<OidcDbContext>().Dbcontext;
        scope = new ApiScope("ceshi");
        await dbContext.ApiScopes.AddAsync(scope);
        await dbContext.SaveChangesAsync();
        await sync.SyncApiScopeCacheAsync(scope.Id);
        scopes = await cache.GetListAsync();
        Assert.IsTrue(scopes.Any(item => item.Name == scope.Name));
    }

    [TestMethod]
    public async Task TestIdentityResourceCacheAsync()
    {
        var serviceCollection = InitializingData();
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var sync = serviceProvider.GetRequiredService<SyncCache>();
        var resource = new IdentityResource("不存入数据库", "不存入数据库", "不存入数据库", default, default, default, default, default);
        await sync.SyncIdentityResourceCacheAsync(resource.Id);
        var cache = serviceProvider.GetRequiredService<IIdentityResourceCache>();
        var resources = await cache.GetListAsync();
        Assert.IsFalse(resources.Any(item => item.Name == resource.Name));

        var dbContext = (CustomDbContext)serviceProvider.GetRequiredService<OidcDbContext>().Dbcontext;
        resource = new IdentityResource("ceshi", "测试", "这是个测试", default, default, default, default, default);
        await dbContext.IdentityResources.AddAsync(resource);
        await dbContext.SaveChangesAsync();
        await sync.SyncIdentityResourceCacheAsync(resource.Id);
        resources = await cache.GetListAsync();
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

    [TestMethod]
    public async Task TestClientQueryAsync()
    {
        var serviceCollection = InitializingData();
        using var serviceProvider = serviceCollection.BuildServiceProvider();
        var dbContext = (CustomDbContext)serviceProvider.GetRequiredService<OidcDbContext>().Dbcontext;
        var client = new Client(ClientTypes.Web, Guid.NewGuid().ToString(), "测试");
        var clientPropertys = new List<ClientProperty> { new ClientProperty("key","value")};
        var clientClaims = new List<ClientClaim> { new ClientClaim("type","value") };
        var clientIdPRestrictions = new List<ClientIdPRestriction> { new ClientIdPRestriction("provider") };
        var clientCorsOrigins = new List<ClientCorsOrigin> { new ClientCorsOrigin("origin") };
        var clientSecrets = new List<ClientSecret> { new ClientSecret("description", "value", DateTime.Now.AddYears(1), "type") };
        var clientGrantTypes = new List<ClientGrantType> { new ClientGrantType("grantType") };
        var clientRedirectUris = new List<ClientRedirectUri> { new ClientRedirectUri("redirectUri") };
        var clientPostLogoutRedirectUris = new List<ClientPostLogoutRedirectUri> { new ClientPostLogoutRedirectUri("postLogoutRedirectUri") };
        var clientScopes = new List<ClientScope> { new ClientScope("scope") };
        client.AllowedGrantTypes.AddRange(clientGrantTypes);
        client.RedirectUris.AddRange(clientRedirectUris);
        client.PostLogoutRedirectUris.AddRange(clientPostLogoutRedirectUris);
        client.Properties.AddRange(clientPropertys);
        client.Claims.AddRange(clientClaims);
        client.IdentityProviderRestrictions.AddRange(clientIdPRestrictions);
        client.AllowedCorsOrigins.AddRange(clientCorsOrigins);
        client.ClientSecrets.AddRange(clientSecrets);
        client.AllowedScopes.AddRange(clientScopes);
        await dbContext.Clients.AddAsync(client);
        await dbContext.SaveChangesAsync();
        var clients = await dbContext.Clients.ToListAsync();
        client = clients.FirstOrDefault(item => item.ClientId == client.ClientId);
        Assert.IsTrue(client is not null);
        Assert.IsTrue(client!.AllowedGrantTypes.Any(item => item.GrantType == "grantType"));
        Assert.IsTrue(client!.RedirectUris.Any(item => item.RedirectUri == "redirectUri"));
        Assert.IsTrue(client!.PostLogoutRedirectUris.Any(item => item.PostLogoutRedirectUri == "postLogoutRedirectUri"));
        Assert.IsTrue(client!.Properties.Any(item => item.Key == "key"));
        Assert.IsTrue(client!.Claims.Any(item => item.Type == "type"));
        Assert.IsTrue(client!.IdentityProviderRestrictions.Any(item => item.Provider == "provider"));
        Assert.IsTrue(client!.AllowedCorsOrigins.Any(item => item.Origin == "origin"));
        Assert.IsTrue(client!.ClientSecrets.Any(item => item.Value == "value"));
        Assert.IsTrue(client!.AllowedScopes.Any(item => item.Scope == "scope"));
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
