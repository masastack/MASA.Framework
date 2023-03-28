// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.EFCore.Tests.Scenes.Isolation;

[TestClass]
public class DefaultIsolationConnectionStringProviderTest
{
    private IServiceCollection _services;

    [TestInitialize]
    public void Initialize()
    {
        _services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .Build();
        _services.AddSingleton<IConfiguration>(configuration);
        _services.AddMasaDbContext<CustomDbContext>(dbContextBuilder => dbContextBuilder.UseSqlite());
    }

    [TestMethod]
    public async Task TestGetConnectionStringAsync()
    {
        _services.AddIsolation(isolationBuilder => isolationBuilder.UseMultiEnvironment());

        var rootServiceProvider = _services.BuildServiceProvider();
        using var scope = rootServiceProvider.CreateScope();
        var connectionString = await GetConnectionString(scope.ServiceProvider);
        Assert.AreEqual("data source=test", connectionString);

        using var scope2 = rootServiceProvider.CreateScope();
        scope2.ServiceProvider.GetRequiredService<IMultiEnvironmentSetter>().SetEnvironment("dev");
        var connectionString2 = await GetConnectionString(scope2.ServiceProvider);
        Assert.AreEqual("Data Source=test2.db", connectionString2);

        scope2.ServiceProvider.GetRequiredService<IMultiEnvironmentSetter>().SetEnvironment("pro");
        connectionString2 = await GetConnectionString(scope2.ServiceProvider);
        Assert.AreEqual("Data Source=test2.db", connectionString2);

        using var scope3 = rootServiceProvider.CreateScope();
        scope3.ServiceProvider.GetRequiredService<IMultiEnvironmentSetter>().SetEnvironment("pro");
        var connectionString3 = await GetConnectionString(scope3.ServiceProvider);
        Assert.AreEqual("Data Source=test3.db", connectionString3);

        Task<string> GetConnectionString(IServiceProvider serviceProvider, string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME)
        {
            var isolationConnectionStringProvider = new DefaultIsolationConnectionStringProvider(
                serviceProvider.GetRequiredService<IConnectionStringProviderWrapper>(),
                serviceProvider.GetRequiredService<IIsolationConfigProvider>()
            );
            return isolationConnectionStringProvider.GetConnectionStringAsync(name);
        }
    }

    [TestMethod]
    public async Task TestTenantIdByAddEntityAsync()
    {
        _services.Configure<IsolationOptions>(options => options.MultiTenantIdType = typeof(string));
        _services.AddIsolation(isolationBuilder => isolationBuilder.UseMultiTenant());
        var rootServiceProvider = _services.BuildServiceProvider();
        var dbContext = rootServiceProvider.GetRequiredService<CustomDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        using var scope = rootServiceProvider.CreateScope();
        var multiTenantSetter = scope.ServiceProvider.GetRequiredService<IMultiTenantSetter>();
        var tenantId = "masa";
        multiTenantSetter.SetTenant(new Tenant(tenantId));
        var customDbContext = scope.ServiceProvider.GetRequiredService<CustomDbContext>();
        var user = new User()
        {
            Name = "masa",
        };
        await customDbContext.Set<User>().AddAsync(user);
        await customDbContext.SaveChangesAsync();

        var userTemp = await customDbContext.User.FirstOrDefaultAsync();
        Assert.IsNotNull(userTemp);
        Assert.AreEqual(tenantId, userTemp.TenantId);
    }
}
