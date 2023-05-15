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
    public async Task TestConnectionStringAsync()
    {
        _services.AddMasaDbContext<CustomDbContext>(dbContextBuilder => dbContextBuilder.UseSqlite());
        _services.AddIsolation(isolationBuilder =>
        {
            isolationBuilder.UseMultiEnvironment();
        });

        var rootServiceProvider = _services.BuildServiceProvider();
        using var scope = rootServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetService<CustomDbContext>();
        Assert.IsNotNull(dbContext);

        await VerifyConnectionStringAsync(scope.ServiceProvider, ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME, "data source=test");
        using var scope2 = rootServiceProvider.CreateScope();
        scope2.ServiceProvider.GetRequiredService<IMultiEnvironmentSetter>().SetEnvironment("dev");
        await VerifyConnectionStringAsync(scope2.ServiceProvider, ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME, "Data Source=test2.db");

        scope2.ServiceProvider.GetRequiredService<IMultiEnvironmentSetter>().SetEnvironment("pro");
        await VerifyConnectionStringAsync(scope2.ServiceProvider, ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME, "Data Source=test2.db");

        using var scope3 = rootServiceProvider.CreateScope();
        scope3.ServiceProvider.GetRequiredService<IMultiEnvironmentSetter>().SetEnvironment("pro");

        await VerifyConnectionStringAsync(scope3.ServiceProvider, ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME, "Data Source=test3.db");
    }

    [TestMethod]
    public async Task TestConnectionStringAsyncByOptions()
    {
        _services.AddMasaDbContext<CustomDbContext>(dbContextBuilder => dbContextBuilder.UseSqlite());
        _services.AddIsolation(isolationBuilder =>
        {
            isolationBuilder.UseMultiEnvironment();
        });
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
        await VerifyConnectionStringAsync(scope.ServiceProvider, ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME, "data source=test");

        using var scope2 = rootServiceProvider.CreateScope();
        var multiEnvironmentSetter = scope2.ServiceProvider.GetService<IMultiEnvironmentSetter>();
        Assert.IsNotNull(multiEnvironmentSetter);
        multiEnvironmentSetter.SetEnvironment("dev");

        await VerifyConnectionStringAsync(scope2.ServiceProvider, ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME, "data source=test-manual");
    }

    private static async Task VerifyConnectionStringAsync(
        IServiceProvider serviceProvider,
        string name,
        string exceptedConnectionString)
    {
        var isolationConnectionStringProvider = new DefaultIsolationConnectionStringProvider(
            serviceProvider.GetRequiredService<IConnectionStringProviderWrapper>(),
            serviceProvider.GetRequiredService<IIsolationConfigProvider>()
        );
        var actualConnectionString = await isolationConnectionStringProvider.GetConnectionStringAsync(name);
        Assert.AreEqual(exceptedConnectionString, actualConnectionString);

        var currentDbContext = serviceProvider.GetService<CustomDbContext>();
        Assert.IsNotNull(currentDbContext);

        Assert.AreEqual(exceptedConnectionString, currentDbContext.Database.GetConnectionString());
    }
}
