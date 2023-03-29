// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Moq;

namespace Masa.Contrib.Data.EFCore.Tests;

[TestClass]
public class DefaultConnectionStringProviderTest
{
    private readonly DefaultConnectionStringProvider _defaultConnectionStringProvider;
    public DefaultConnectionStringProviderTest()
    {
        Mock<IConnectionStringConfigProvider> connectionStringConfigProvider = new();
        connectionStringConfigProvider.Setup(provider => provider.GetConnectionStrings()).Returns(new Dictionary<string, string>()
        {
            {
                ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME, "Test1"
            }
        });

        _defaultConnectionStringProvider = new DefaultConnectionStringProvider(connectionStringConfigProvider.Object);
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
