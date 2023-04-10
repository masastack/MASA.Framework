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
    }

    [TestMethod]
    public async Task TestGetConnectionStringAsync()
    {
        _services.AddMasaDbContext<CustomDbContext>(dbContextBuilder => dbContextBuilder.UseSqlite());
        _services.AddIsolation(isolationBuilder => { isolationBuilder.UseMultiEnvironment(); });

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
    public async Task TestGetConnectionStringAsyncByAddIsolationConfiguration()
    {
        _services.AddMasaDbContext<CustomDbContext>(dbContextBuilder => dbContextBuilder.UseSqlite());
        _services.AddIsolation(isolationBuilder => { isolationBuilder.UseMultiEnvironment(); });
        _services.Configure<IsolationOptions<ConnectionStrings>>(options =>
        {
            options.Data.Add(new IsolationConfigurationOptions<ConnectionStrings>()
            {
                Environment = "dev",
                Data = new ConnectionStrings(new List<KeyValuePair<string, string>>()
                {
                    new(ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME, "data source=test-manual"),
                })
            });
        });
        var rootServiceProvider = _services.BuildServiceProvider();
        using var scope = rootServiceProvider.CreateScope();
        var connectionString = await GetConnectionString(scope.ServiceProvider);
        Assert.AreEqual("data source=test", connectionString);

        using var scope2 = rootServiceProvider.CreateScope();
        var multiEnvironmentSetter = scope2.ServiceProvider.GetService<IMultiEnvironmentSetter>();
        Assert.IsNotNull(multiEnvironmentSetter);
        multiEnvironmentSetter.SetEnvironment("dev");
        var connectionString2 = await GetConnectionString(scope2.ServiceProvider);
        Assert.AreEqual("data source=test-manual", connectionString2);

        Task<string> GetConnectionString(IServiceProvider serviceProvider, string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME)
        {
            var isolationConnectionStringProvider = new DefaultIsolationConnectionStringProvider(
                serviceProvider.GetRequiredService<IConnectionStringProviderWrapper>(),
                serviceProvider.GetRequiredService<IIsolationConfigProvider>()
            );
            return isolationConnectionStringProvider.GetConnectionStringAsync(name);
        }
    }
}
