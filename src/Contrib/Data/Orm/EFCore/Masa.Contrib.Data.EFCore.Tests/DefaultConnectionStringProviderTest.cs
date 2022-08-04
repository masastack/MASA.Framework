// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Tests;

[TestClass]
public class DefaultConnectionStringProviderTest
{
    [TestMethod]
    public async Task TestGetConnectionStringAsyncReturnTest1()
    {
        IServiceCollection services = new ServiceCollection();
        services.Configure<MasaDbConnectionOptions>(options =>
        {
            options.ConnectionStrings = new ConnectionStrings()
            {
                DefaultConnection = "Test1"
            };
        });
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptionsMonitor<MasaDbConnectionOptions>>();
        var defaultConnectionStringProvider = new DefaultConnectionStringProvider(options);
        var connectionString = await defaultConnectionStringProvider.GetConnectionStringAsync();
        Assert.AreEqual("Test1", connectionString);
    }

    [TestMethod]
    public async Task TestGetConnectionStringAsyncAndNameIsEmptyReturnTest1()
    {
        IServiceCollection services = new ServiceCollection();
        services.Configure<MasaDbConnectionOptions>(options =>
        {
            options.ConnectionStrings = new ConnectionStrings()
            {
                DefaultConnection = "Test1"
            };
        });
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptionsMonitor<MasaDbConnectionOptions>>();
        var defaultConnectionStringProvider = new DefaultConnectionStringProvider(options);
        var connectionString = await defaultConnectionStringProvider.GetConnectionStringAsync(string.Empty);
        Assert.AreEqual("Test1", connectionString);
    }
}
