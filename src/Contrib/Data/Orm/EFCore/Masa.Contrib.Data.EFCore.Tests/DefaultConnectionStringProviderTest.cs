// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Tests;

[TestClass]
public class DefaultConnectionStringProviderTest
{
    private readonly DefaultConnectionStringProvider _defaultConnectionStringProvider;

    public DefaultConnectionStringProviderTest()
    {
        IServiceCollection services = new ServiceCollection();
        services.Configure<ConnectionStrings>(options =>
        {
            options.DefaultConnection = "Test1";
        });
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptionsSnapshot<ConnectionStrings>>();
        _defaultConnectionStringProvider = new DefaultConnectionStringProvider(options);
    }

    [TestMethod]
    public async Task TestGetConnectionStringAsyncReturnTest1()
    {
        var connectionString = await _defaultConnectionStringProvider.GetConnectionStringAsync();
        Assert.AreEqual("Test1", connectionString);
    }

    [TestMethod]
    public async Task TestGetConnectionStringAsyncAndNameIsEmptyReturnTest1()
    {
        var connectionString = await _defaultConnectionStringProvider.GetConnectionStringAsync(string.Empty);
        Assert.AreEqual("Test1", connectionString);
    }
}
